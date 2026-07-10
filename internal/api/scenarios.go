package api

import (
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"math/rand"
	"net/http"
	"time"

	"github.com/go-chi/chi/v5"
	"github.com/google/uuid"
	"github.com/jackc/pgx/v5"
	"github.com/tejle/SMART/internal/auth"
	"github.com/tejle/SMART/internal/domain"
	"github.com/tejle/SMART/internal/engine"
	"github.com/tejle/SMART/internal/plugins"
)

func (h *Handler) PluginCatalog(w http.ResponseWriter, _ *http.Request) {
	writeJSON(w, http.StatusOK, plugins.Catalog())
}

func (h *Handler) CreateScenario(w http.ResponseWriter, r *http.Request) {
	orgID, ok := auth.OrgIDFromContext(r.Context())
	if !ok {
		writeError(w, http.StatusUnauthorized, "missing organization context")
		return
	}
	projectID, err := uuid.Parse(chi.URLParam(r, "projectID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid project id")
		return
	}

	var input domain.CreateScenarioInput
	if err := json.NewDecoder(r.Body).Decode(&input); err != nil {
		writeError(w, http.StatusBadRequest, "invalid JSON body")
		return
	}
	if input.Name == "" || len(input.ModelIDs) == 0 {
		writeError(w, http.StatusBadRequest, "name and modelIds are required")
		return
	}

	scenario, err := h.store.CreateScenario(r.Context(), orgID, projectID, input)
	if err != nil {
		writeError(w, http.StatusInternalServerError, "failed to create scenario")
		return
	}
	writeJSON(w, http.StatusCreated, scenario)
}

func (h *Handler) ListScenarios(w http.ResponseWriter, r *http.Request) {
	orgID, ok := auth.OrgIDFromContext(r.Context())
	if !ok {
		writeError(w, http.StatusUnauthorized, "missing organization context")
		return
	}
	projectID, err := uuid.Parse(chi.URLParam(r, "projectID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid project id")
		return
	}

	scenarios, err := h.store.ListScenarios(r.Context(), orgID, projectID)
	if err != nil {
		writeError(w, http.StatusInternalServerError, "failed to list scenarios")
		return
	}
	if scenarios == nil {
		scenarios = []domain.Scenario{}
	}
	writeJSON(w, http.StatusOK, scenarios)
}

func (h *Handler) StartScenarioRun(w http.ResponseWriter, r *http.Request) {
	orgID, ok := auth.OrgIDFromContext(r.Context())
	if !ok {
		writeError(w, http.StatusUnauthorized, "missing organization context")
		return
	}

	scenarioID, err := uuid.Parse(chi.URLParam(r, "scenarioID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid scenario id")
		return
	}

	var payload struct {
		Kind domain.RunKind `json:"kind"`
	}
	if err := json.NewDecoder(r.Body).Decode(&payload); err != nil || payload.Kind == "" {
		payload.Kind = domain.RunKindGenerate
	}

	scenario, projectID, err := h.findScenario(r, orgID, scenarioID)
	if err != nil {
		if errors.Is(err, pgx.ErrNoRows) {
			writeError(w, http.StatusNotFound, "scenario not found")
			return
		}
		writeError(w, http.StatusInternalServerError, "failed to load scenario")
		return
	}

	run, err := h.store.CreateRun(r.Context(), orgID, projectID, scenarioID, payload.Kind)
	if err != nil {
		writeError(w, http.StatusInternalServerError, "failed to create run")
		return
	}

	run.Status = domain.RunStatusRunning
	_ = h.store.UpdateRun(r.Context(), run)

	var runResult domain.RunResult
	var runErr error

	switch payload.Kind {
	case domain.RunKindGenerate:
		generation, err := h.generateForScenario(r, orgID, projectID, scenario)
		if err != nil {
			runErr = err
		} else {
			runResult.Generation = &generation
		}
	case domain.RunKindExecute:
		writeJSON(w, http.StatusAccepted, run)
		go h.executeScenarioAsync(run, scenario, orgID, projectID)
		return
	default:
		runErr = fmt.Errorf("unsupported run kind")
	}

	now := time.Now().UTC()
	run.CompletedAt = &now
	if runErr != nil {
		run.Status = domain.RunStatusFailed
		run.ErrorMessage = runErr.Error()
	} else {
		run.Status = domain.RunStatusCompleted
		run.Result = &runResult
	}
	_ = h.store.UpdateRun(r.Context(), run)

	if runErr != nil {
		writeError(w, http.StatusInternalServerError, run.ErrorMessage)
		return
	}
	writeJSON(w, http.StatusAccepted, run)
}

func (h *Handler) executeScenarioAsync(run domain.Run, scenario domain.Scenario, orgID, projectID uuid.UUID) {
	ctx := context.Background()
	var runResult domain.RunResult
	var runErr error

	generation, err := h.generateForScenarioCtx(ctx, orgID, projectID, scenario)
	if err != nil {
		runErr = err
	} else {
		emit := func(event engine.RunEvent) {
			h.runEvents.Publish(run.ID, event)
		}
		execution, err := engine.Execute(ctx, generation.Paths, scenario.AdapterConfig, emit)
		if err != nil {
			runErr = err
		} else {
			runResult.Generation = &generation
			runResult.Execution = &execution
		}
	}

	now := time.Now().UTC()
	run.CompletedAt = &now
	if runErr != nil {
		run.Status = domain.RunStatusFailed
		run.ErrorMessage = runErr.Error()
	} else {
		run.Status = domain.RunStatusCompleted
		run.Result = &runResult
	}
	_ = h.store.UpdateRun(ctx, run)
	h.runEvents.Close(run.ID)
}

func (h *Handler) GetRun(w http.ResponseWriter, r *http.Request) {
	orgID, ok := auth.OrgIDFromContext(r.Context())
	if !ok {
		writeError(w, http.StatusUnauthorized, "missing organization context")
		return
	}
	runID, err := uuid.Parse(chi.URLParam(r, "runID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid run id")
		return
	}

	run, err := h.store.GetRun(r.Context(), orgID, runID)
	if err != nil {
		if errors.Is(err, pgx.ErrNoRows) {
			writeError(w, http.StatusNotFound, "run not found")
			return
		}
		writeError(w, http.StatusInternalServerError, "failed to get run")
		return
	}
	writeJSON(w, http.StatusOK, run)
}

func (h *Handler) StreamRunEvents(w http.ResponseWriter, r *http.Request) {
	runID, err := uuid.Parse(chi.URLParam(r, "runID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid run id")
		return
	}

	flusher, ok := w.(http.Flusher)
	if !ok {
		writeError(w, http.StatusInternalServerError, "streaming not supported")
		return
	}

	w.Header().Set("Content-Type", "text/event-stream")
	w.Header().Set("Cache-Control", "no-cache")
	w.Header().Set("Connection", "keep-alive")

	events := h.runEvents.Subscribe(runID)
	defer func() {
		// no-op; hub closes channels when run completes
	}()

	for {
		select {
		case <-r.Context().Done():
			return
		case payload, open := <-events:
			if !open {
				fmt.Fprintf(w, "event: close\ndata: {}\n\n")
				flusher.Flush()
				return
			}
			fmt.Fprintf(w, "event: run\ndata: %s\n\n", payload)
			flusher.Flush()
		}
	}
}

func (h *Handler) findScenario(r *http.Request, orgID, scenarioID uuid.UUID) (domain.Scenario, uuid.UUID, error) {
	projects, err := h.store.ListProjects(r.Context(), orgID)
	if err != nil {
		return domain.Scenario{}, uuid.Nil, err
	}
	for _, project := range projects {
		scenario, err := h.store.GetScenario(r.Context(), orgID, project.ID, scenarioID)
		if err == nil {
			return scenario, project.ID, nil
		}
		if !errors.Is(err, pgx.ErrNoRows) {
			return domain.Scenario{}, uuid.Nil, err
		}
	}
	return domain.Scenario{}, uuid.Nil, pgx.ErrNoRows
}

func (h *Handler) generateForScenario(r *http.Request, orgID, projectID uuid.UUID, scenario domain.Scenario) (domain.GenerationResult, error) {
	return h.generateForScenarioCtx(r.Context(), orgID, projectID, scenario)
}

func (h *Handler) generateForScenarioCtx(ctx context.Context, orgID, projectID uuid.UUID, scenario domain.Scenario) (domain.GenerationResult, error) {
	var models []domain.Model
	for _, modelID := range scenario.ModelIDs {
		model, err := h.store.GetModel(ctx, orgID, projectID, modelID)
		if err != nil {
			return domain.GenerationResult{}, err
		}
		models = append(models, model)
	}

	graph, err := engine.Compile(models)
	if err != nil {
		return domain.GenerationResult{}, err
	}
	return engine.Generate(graph, scenario.Algorithm, scenario.GenerationConfig, rand.New(rand.NewSource(time.Now().UnixNano())))
}