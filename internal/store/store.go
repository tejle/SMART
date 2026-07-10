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
}