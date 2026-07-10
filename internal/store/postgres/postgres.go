package postgres

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	"github.com/jackc/pgx/v5/pgxpool"
	"github.com/tejle/SMART/internal/domain"
	"github.com/tejle/SMART/internal/store"
)

type Store struct {
	pool *pgxpool.Pool
}

func New(ctx context.Context, databaseURL string) (*Store, error) {
	pool, err := pgxpool.New(ctx, databaseURL)
	if err != nil {
		return nil, fmt.Errorf("connect postgres: %w", err)
	}
	if err := pool.Ping(ctx); err != nil {
		pool.Close()
		return nil, fmt.Errorf("ping postgres: %w", err)
	}
	return &Store{pool: pool}, nil
}

func (s *Store) Close() {
	s.pool.Close()
}

func (s *Store) CreateOrganization(ctx context.Context, name string) (domain.Organization, error) {
	var org domain.Organization
	err := s.pool.QueryRow(ctx, `
		INSERT INTO organizations (name)
		VALUES ($1)
		RETURNING id, name, created_at
	`, name).Scan(&org.ID, &org.Name, &org.CreatedAt)
	return org, err
}

func (s *Store) GetOrganization(ctx context.Context, id uuid.UUID) (domain.Organization, error) {
	var org domain.Organization
	err := s.pool.QueryRow(ctx, `
		SELECT id, name, created_at
		FROM organizations
		WHERE id = $1
	`, id).Scan(&org.ID, &org.Name, &org.CreatedAt)
	return org, err
}

func (s *Store) CreateProject(ctx context.Context, orgID uuid.UUID, name string) (domain.Project, error) {
	var project domain.Project
	err := s.pool.QueryRow(ctx, `
		INSERT INTO projects (org_id, name)
		VALUES ($1, $2)
		RETURNING id, org_id, name, created_at, updated_at
	`, orgID, name).Scan(&project.ID, &project.OrgID, &project.Name, &project.CreatedAt, &project.UpdatedAt)
	return project, err
}

func (s *Store) ListProjects(ctx context.Context, orgID uuid.UUID) ([]domain.Project, error) {
	rows, err := s.pool.Query(ctx, `
		SELECT id, org_id, name, created_at, updated_at
		FROM projects
		WHERE org_id = $1
		ORDER BY created_at DESC
	`, orgID)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var projects []domain.Project
	for rows.Next() {
		var p domain.Project
		if err := rows.Scan(&p.ID, &p.OrgID, &p.Name, &p.CreatedAt, &p.UpdatedAt); err != nil {
			return nil, err
		}
		projects = append(projects, p)
	}
	return projects, rows.Err()
}

func (s *Store) GetProject(ctx context.Context, orgID, projectID uuid.UUID) (domain.Project, error) {
	var project domain.Project
	err := s.pool.QueryRow(ctx, `
		SELECT id, org_id, name, created_at, updated_at
		FROM projects
		WHERE org_id = $1 AND id = $2
	`, orgID, projectID).Scan(&project.ID, &project.OrgID, &project.Name, &project.CreatedAt, &project.UpdatedAt)
	return project, err
}

var _ store.Store = (*Store)(nil)