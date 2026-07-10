package api

import (
	"encoding/json"
	"errors"
	"fmt"
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/google/uuid"
	"github.com/jackc/pgx/v5"
	"github.com/tejle/SMART/internal/auth"
	"github.com/tejle/SMART/internal/domain"
	"github.com/tejle/SMART/internal/plugins"
	"github.com/tejle/SMART/internal/queue"
	"github.com/tejle/SMART/internal/store"
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

	_ = h.store.RecordAudit(r.Context(), store.AuditEntry{
		OrgID:        orgID,
		Actor:        "api",
		Action:       "run.start",
		ResourceType: "run",
		ResourceID:   &run.ID,
		Metadata: map[string]any{
			"kind":       payload.Kind,
			"scenarioId": scenarioID.String(),
		},
	})

	if h.queue == nil {
		writeError(w, http.StatusServiceUnavailable, "job queue unavailable")
		return
	}

	if err := h.queue.EnqueueProcessRun(r.Context(), queue.ProcessRunPayload{
		RunID:      run.ID,
		OrgID:      orgID,
		ProjectID:  projectID,
		ScenarioID: scenario.ID,
		Kind:       payload.Kind,
	}); err != nil {
		writeError(w, http.StatusInternalServerError, "failed to enqueue run")
		return
	}

	run.Status = domain.RunStatusPending
	_ = h.store.UpdateRun(r.Context(), run)
	_ = scenario
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

	localEvents := h.runEvents.Subscribe(runID)
	redisEvents := make(chan string, 16)

	if h.redis != nil {
		pubsub, err := h.redis.Subscribe(r.Context(), runID)
		if err == nil {
			defer pubsub.Close()
			go func() {
				ch := pubsub.Channel()
				for {
					select {
					case <-r.Context().Done():
						return
					case msg, ok := <-ch:
						if !ok {
							return
						}
						redisEvents <- msg.Payload
					}
				}
			}()
		}
	}

	for {
		select {
		case <-r.Context().Done():
			return
		case payload, open := <-localEvents:
			if !open {
				fmt.Fprintf(w, "event: close\ndata: {}\n\n")
				flusher.Flush()
				return
			}
			fmt.Fprintf(w, "event: run\ndata: %s\n\n", payload)
			flusher.Flush()
		case payload := <-redisEvents:
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