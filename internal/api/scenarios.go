package api

import (
	"encoding/json"
	"errors"
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
	if payload.Kind != domain.RunKindGenerate {
		writeError(w, http.StatusBadRequest, "only generate runs are supported in this phase")
		return
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

	result, runErr := h.generateForScenario(r, orgID, projectID, scenario)
	now := time.Now().UTC()
	run.CompletedAt = &now
	if runErr != nil {
		run.Status = domain.RunStatusFailed
		run.ErrorMessage = runErr.Error()
	} else {
		run.Status = domain.RunStatusCompleted
		run.Result = &result
	}
	_ = h.store.UpdateRun(r.Context(), run)

	if runErr != nil {
		writeError(w, http.StatusInternalServerError, run.ErrorMessage)
		return
	}
	writeJSON(w, http.StatusAccepted, run)
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
	var models []domain.Model
	for _, modelID := range scenario.ModelIDs {
		model, err := h.store.GetModel(r.Context(), orgID, projectID, modelID)
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