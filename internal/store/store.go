package store

import (
	"context"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/domain"
)

type Store interface {
	CreateOrganization(ctx context.Context, name string) (domain.Organization, error)
	GetOrganization(ctx context.Context, id uuid.UUID) (domain.Organization, error)
	CreateProject(ctx context.Context, orgID uuid.UUID, name string) (domain.Project, error)
	ListProjects(ctx context.Context, orgID uuid.UUID) ([]domain.Project, error)
	GetProject(ctx context.Context, orgID, projectID uuid.UUID) (domain.Project, error)

	CreateModel(ctx context.Context, orgID, projectID uuid.UUID, name string, graph domain.ModelGraph) (domain.Model, error)
	ListModels(ctx context.Context, orgID, projectID uuid.UUID) ([]domain.Model, error)
	GetModel(ctx context.Context, orgID, projectID, modelID uuid.UUID) (domain.Model, error)
	UpdateModel(ctx context.Context, orgID, projectID, modelID uuid.UUID, name *string, graph *domain.ModelGraph) (domain.Model, error)
	DeleteModel(ctx context.Context, orgID, projectID, modelID uuid.UUID) error

	CreateScenario(ctx context.Context, orgID, projectID uuid.UUID, input domain.CreateScenarioInput) (domain.Scenario, error)
	ListScenarios(ctx context.Context, orgID, projectID uuid.UUID) ([]domain.Scenario, error)
	GetScenario(ctx context.Context, orgID, projectID, scenarioID uuid.UUID) (domain.Scenario, error)
	UpdateScenario(ctx context.Context, orgID, projectID, scenarioID uuid.UUID, input domain.UpdateScenarioInput) (domain.Scenario, error)

	CreateRun(ctx context.Context, orgID, projectID, scenarioID uuid.UUID, kind domain.RunKind) (domain.Run, error)
	UpdateRun(ctx context.Context, run domain.Run) error
	GetRun(ctx context.Context, orgID, runID uuid.UUID) (domain.Run, error)
	ListRunsByProject(ctx context.Context, orgID, projectID uuid.UUID) ([]domain.Run, error)

	RecordAudit(ctx context.Context, entry AuditEntry) error
}