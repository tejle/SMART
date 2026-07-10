package api

import (
	"encoding/json"
	"errors"
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/google/uuid"
	"github.com/jackc/pgx/v5"
	"github.com/tejle/SMART/internal/auth"
	"github.com/tejle/SMART/internal/domain"
)

func (h *Handler) CreateModel(w http.ResponseWriter, r *http.Request) {
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

	var input domain.CreateModelInput
	if err := json.NewDecoder(r.Body).Decode(&input); err != nil {
		writeError(w, http.StatusBadRequest, "invalid JSON body")
		return
	}
	if input.Name == "" {
		writeError(w, http.StatusBadRequest, "name is required")
		return
	}

	if _, err := h.store.GetProject(r.Context(), orgID, projectID); err != nil {
		if errors.Is(err, pgx.ErrNoRows) {
			writeError(w, http.StatusNotFound, "project not found")
			return
		}
		writeError(w, http.StatusInternalServerError, "failed to verify project")
		return
	}

	model, err := h.store.CreateModel(r.Context(), orgID, projectID, input.Name, domain.DefaultModelGraph())
	if err != nil {
		writeError(w, http.StatusInternalServerError, "failed to create model")
		return
	}
	writeJSON(w, http.StatusCreated, model)
}

func (h *Handler) ListModels(w http.ResponseWriter, r *http.Request) {
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

	models, err := h.store.ListModels(r.Context(), orgID, projectID)
	if err != nil {
		writeError(w, http.StatusInternalServerError, "failed to list models")
		return
	}
	if models == nil {
		models = []domain.Model{}
	}
	writeJSON(w, http.StatusOK, models)
}

func (h *Handler) GetModel(w http.ResponseWriter, r *http.Request) {
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
	modelID, err := uuid.Parse(chi.URLParam(r, "modelID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid model id")
		return
	}

	model, err := h.store.GetModel(r.Context(), orgID, projectID, modelID)
	if err != nil {
		if errors.Is(err, pgx.ErrNoRows) {
			writeError(w, http.StatusNotFound, "model not found")
			return
		}
		writeError(w, http.StatusInternalServerError, "failed to get model")
		return
	}
	writeJSON(w, http.StatusOK, model)
}

func (h *Handler) UpdateModel(w http.ResponseWriter, r *http.Request) {
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
	modelID, err := uuid.Parse(chi.URLParam(r, "modelID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid model id")
		return
	}

	var input domain.UpdateModelInput
	if err := json.NewDecoder(r.Body).Decode(&input); err != nil {
		writeError(w, http.StatusBadRequest, "invalid JSON body")
		return
	}
	if input.Name == nil && input.Graph == nil {
		writeError(w, http.StatusBadRequest, "nothing to update")
		return
	}

	model, err := h.store.UpdateModel(r.Context(), orgID, projectID, modelID, input.Name, input.Graph)
	if err != nil {
		if errors.Is(err, pgx.ErrNoRows) {
			writeError(w, http.StatusNotFound, "model not found")
			return
		}
		writeError(w, http.StatusInternalServerError, "failed to update model")
		return
	}
	writeJSON(w, http.StatusOK, model)
}

func (h *Handler) DeleteModel(w http.ResponseWriter, r *http.Request) {
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
	modelID, err := uuid.Parse(chi.URLParam(r, "modelID"))
	if err != nil {
		writeError(w, http.StatusBadRequest, "invalid model id")
		return
	}

	if err := h.store.DeleteModel(r.Context(), orgID, projectID, modelID); err != nil {
		if errors.Is(err, pgx.ErrNoRows) {
			writeError(w, http.StatusNotFound, "model not found")
			return
		}
		writeError(w, http.StatusInternalServerError, "failed to delete model")
		return
	}
	w.WriteHeader(http.StatusNoContent)
}