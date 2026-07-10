package api

import (
	"bytes"
	"context"
	"encoding/json"
	"net/http"
	"net/http/httptest"
	"testing"
	"time"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/auth"
	"github.com/tejle/SMART/internal/domain"
)

type mockStore struct {
	orgs     map[uuid.UUID]domain.Organization
	projects map[uuid.UUID]domain.Project
}

func newMockStore() *mockStore {
	return &mockStore{
		orgs:     make(map[uuid.UUID]domain.Organization),
		projects: make(map[uuid.UUID]domain.Project),
	}
}

func (m *mockStore) CreateOrganization(_ context.Context, name string) (domain.Organization, error) {
	org := domain.Organization{
		ID:        uuid.New(),
		Name:      name,
		CreatedAt: time.Now().UTC(),
	}
	m.orgs[org.ID] = org
	return org, nil
}

func (m *mockStore) GetOrganization(_ context.Context, id uuid.UUID) (domain.Organization, error) {
	return m.orgs[id], nil
}

func (m *mockStore) CreateProject(_ context.Context, orgID uuid.UUID, name string) (domain.Project, error) {
	project := domain.Project{
		ID:        uuid.New(),
		OrgID:     orgID,
		Name:      name,
		CreatedAt: time.Now().UTC(),
		UpdatedAt: time.Now().UTC(),
	}
	m.projects[project.ID] = project
	return project, nil
}

func (m *mockStore) ListProjects(_ context.Context, orgID uuid.UUID) ([]domain.Project, error) {
	var out []domain.Project
	for _, p := range m.projects {
		if p.OrgID == orgID {
			out = append(out, p)
		}
	}
	return out, nil
}

func (m *mockStore) GetProject(_ context.Context, orgID, projectID uuid.UUID) (domain.Project, error) {
	p, ok := m.projects[projectID]
	if !ok || p.OrgID != orgID {
		return domain.Project{}, errNotFound
	}
	return p, nil
}

func (m *mockStore) CreateModel(_ context.Context, orgID, projectID uuid.UUID, name string, graph domain.ModelGraph) (domain.Model, error) {
	model := domain.Model{
		ID:        uuid.New(),
		OrgID:     orgID,
		ProjectID: projectID,
		Name:      name,
		Graph:     graph,
		CreatedAt: time.Now().UTC(),
		UpdatedAt: time.Now().UTC(),
	}
	return model, nil
}

func (m *mockStore) ListModels(_ context.Context, _, _ uuid.UUID) ([]domain.Model, error) {
	return []domain.Model{}, nil
}

func (m *mockStore) GetModel(_ context.Context, _, _, _ uuid.UUID) (domain.Model, error) {
	return domain.Model{}, errNotFound
}

func (m *mockStore) UpdateModel(_ context.Context, _, _, _ uuid.UUID, _ *string, _ *domain.ModelGraph) (domain.Model, error) {
	return domain.Model{}, errNotFound
}

func (m *mockStore) DeleteModel(_ context.Context, _, _, _ uuid.UUID) error {
	return errNotFound
}

func (m *mockStore) CreateScenario(_ context.Context, orgID, projectID uuid.UUID, input domain.CreateScenarioInput) (domain.Scenario, error) {
	return domain.Scenario{
		ID:        uuid.New(),
		OrgID:     orgID,
		ProjectID: projectID,
		Name:      input.Name,
		ModelIDs:  input.ModelIDs,
		Algorithm: "breadth-first",
		CreatedAt: time.Now().UTC(),
		UpdatedAt: time.Now().UTC(),
	}, nil
}

func (m *mockStore) ListScenarios(_ context.Context, _, _ uuid.UUID) ([]domain.Scenario, error) {
	return []domain.Scenario{}, nil
}

func (m *mockStore) GetScenario(_ context.Context, _, _, _ uuid.UUID) (domain.Scenario, error) {
	return domain.Scenario{}, errNotFound
}

func (m *mockStore) UpdateScenario(_ context.Context, _, _, _ uuid.UUID, _ domain.UpdateScenarioInput) (domain.Scenario, error) {
	return domain.Scenario{}, errNotFound
}

func (m *mockStore) CreateRun(_ context.Context, orgID, projectID, scenarioID uuid.UUID, kind domain.RunKind) (domain.Run, error) {
	return domain.Run{
		ID:         uuid.New(),
		OrgID:      orgID,
		ProjectID:  projectID,
		ScenarioID: scenarioID,
		Kind:       kind,
		Status:     domain.RunStatusPending,
		CreatedAt:  time.Now().UTC(),
	}, nil
}

func (m *mockStore) UpdateRun(_ context.Context, _ domain.Run) error { return nil }

func (m *mockStore) GetRun(_ context.Context, _, _ uuid.UUID) (domain.Run, error) {
	return domain.Run{}, errNotFound
}

var errNotFound = &notFoundError{}

type notFoundError struct{}

func (e *notFoundError) Error() string { return "not found" }

func TestHealth(t *testing.T) {
	h := NewHandler(newMockStore())
	req := httptest.NewRequest(http.MethodGet, "/healthz", nil)
	rec := httptest.NewRecorder()
	NewRouter(h, true).ServeHTTP(rec, req)

	if rec.Code != http.StatusOK {
		t.Fatalf("expected 200, got %d", rec.Code)
	}
}

func TestCreateProjectRequiresOrgHeader(t *testing.T) {
	h := NewHandler(newMockStore())
	body, _ := json.Marshal(domain.CreateProjectInput{Name: "Demo"})
	req := httptest.NewRequest(http.MethodPost, "/v1/projects", bytes.NewReader(body))
	rec := httptest.NewRecorder()
	NewRouter(h, true).ServeHTTP(rec, req)

	if rec.Code != http.StatusUnauthorized {
		t.Fatalf("expected 401, got %d", rec.Code)
	}
}

func TestCreateAndListProjects(t *testing.T) {
	store := newMockStore()
	h := NewHandler(store)
	router := NewRouter(h, true)
	orgID := uuid.New()

	createBody, _ := json.Marshal(domain.CreateProjectInput{Name: "Checkout flow"})
	req := httptest.NewRequest(http.MethodPost, "/v1/projects", bytes.NewReader(createBody))
	req.Header.Set("X-Org-ID", orgID.String())
	rec := httptest.NewRecorder()
	router.ServeHTTP(rec, req)
	if rec.Code != http.StatusCreated {
		t.Fatalf("create: expected 201, got %d body=%s", rec.Code, rec.Body.String())
	}

	listReq := httptest.NewRequest(http.MethodGet, "/v1/projects", nil)
	listReq = listReq.WithContext(auth.WithOrgID(listReq.Context(), orgID))
	listReq.Header.Set("X-Org-ID", orgID.String())
	listRec := httptest.NewRecorder()
	router.ServeHTTP(listRec, listReq)
	if listRec.Code != http.StatusOK {
		t.Fatalf("list: expected 200, got %d", listRec.Code)
	}
}