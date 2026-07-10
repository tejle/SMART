package postgres

import (
	"context"
	"encoding/json"
	"time"

	"github.com/google/uuid"
	"github.com/jackc/pgx/v5"
	"github.com/tejle/SMART/internal/domain"
)

func (s *Store) CreateScenario(ctx context.Context, orgID, projectID uuid.UUID, input domain.CreateScenarioInput) (domain.Scenario, error) {
	algorithm := input.Algorithm
	if algorithm == "" {
		algorithm = "breadth-first"
	}
	cfgPayload, err := json.Marshal(input.GenerationConfig)
	if err != nil {
		return domain.Scenario{}, err
	}
	adapterPayload, err := json.Marshal(input.AdapterConfig)
	if err != nil {
		return domain.Scenario{}, err
	}

	var scenario domain.Scenario
	var cfgBytes []byte
	var adapterBytes []byte
	err = s.pool.QueryRow(ctx, `
		INSERT INTO scenarios (org_id, project_id, name, model_ids, algorithm, generation_config, adapter_config)
		VALUES ($1, $2, $3, $4, $5, $6::jsonb, $7::jsonb)
		RETURNING id, org_id, project_id, name, model_ids, algorithm, generation_config, adapter_config, created_at, updated_at
	`, orgID, projectID, input.Name, input.ModelIDs, algorithm, cfgPayload, adapterPayload).Scan(
		&scenario.ID, &scenario.OrgID, &scenario.ProjectID, &scenario.Name, &scenario.ModelIDs,
		&scenario.Algorithm, &cfgBytes, &adapterBytes, &scenario.CreatedAt, &scenario.UpdatedAt,
	)
	if err != nil {
		return domain.Scenario{}, err
	}
	if err := json.Unmarshal(cfgBytes, &scenario.GenerationConfig); err != nil {
		return domain.Scenario{}, err
	}
	if err := json.Unmarshal(adapterBytes, &scenario.AdapterConfig); err != nil {
		return domain.Scenario{}, err
	}
	return scenario, nil
}

func (s *Store) ListScenarios(ctx context.Context, orgID, projectID uuid.UUID) ([]domain.Scenario, error) {
	rows, err := s.pool.Query(ctx, `
		SELECT id, org_id, project_id, name, model_ids, algorithm, generation_config, adapter_config, created_at, updated_at
		FROM scenarios
		WHERE org_id = $1 AND project_id = $2
		ORDER BY created_at DESC
	`, orgID, projectID)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var scenarios []domain.Scenario
	for rows.Next() {
		scenario, err := scanScenario(rows.Scan)
		if err != nil {
			return nil, err
		}
		scenarios = append(scenarios, scenario)
	}
	return scenarios, rows.Err()
}

func (s *Store) GetScenario(ctx context.Context, orgID, projectID, scenarioID uuid.UUID) (domain.Scenario, error) {
	row := s.pool.QueryRow(ctx, `
		SELECT id, org_id, project_id, name, model_ids, algorithm, generation_config, adapter_config, created_at, updated_at
		FROM scenarios
		WHERE org_id = $1 AND project_id = $2 AND id = $3
	`, orgID, projectID, scenarioID)
	return scanScenario(row.Scan)
}

func (s *Store) UpdateScenario(ctx context.Context, orgID, projectID, scenarioID uuid.UUID, input domain.UpdateScenarioInput) (domain.Scenario, error) {
	current, err := s.GetScenario(ctx, orgID, projectID, scenarioID)
	if err != nil {
		return domain.Scenario{}, err
	}

	if input.Name != nil {
		current.Name = *input.Name
	}
	if input.ModelIDs != nil {
		current.ModelIDs = input.ModelIDs
	}
	if input.Algorithm != nil {
		current.Algorithm = *input.Algorithm
	}
	if input.GenerationConfig != nil {
		current.GenerationConfig = *input.GenerationConfig
	}

	cfgPayload, err := json.Marshal(current.GenerationConfig)
	if err != nil {
		return domain.Scenario{}, err
	}
	adapterPayload, err := json.Marshal(current.AdapterConfig)
	if err != nil {
		return domain.Scenario{}, err
	}

	var scenario domain.Scenario
	var cfgBytes []byte
	var adapterBytes []byte
	err = s.pool.QueryRow(ctx, `
		UPDATE scenarios
		SET name = $4, model_ids = $5, algorithm = $6, generation_config = $7::jsonb, adapter_config = $8::jsonb, updated_at = NOW()
		WHERE org_id = $1 AND project_id = $2 AND id = $3
		RETURNING id, org_id, project_id, name, model_ids, algorithm, generation_config, adapter_config, created_at, updated_at
	`, orgID, projectID, scenarioID, current.Name, current.ModelIDs, current.Algorithm, cfgPayload, adapterPayload).Scan(
		&scenario.ID, &scenario.OrgID, &scenario.ProjectID, &scenario.Name, &scenario.ModelIDs,
		&scenario.Algorithm, &cfgBytes, &adapterBytes, &scenario.CreatedAt, &scenario.UpdatedAt,
	)
	if err != nil {
		return domain.Scenario{}, err
	}
	if err := json.Unmarshal(cfgBytes, &scenario.GenerationConfig); err != nil {
		return domain.Scenario{}, err
	}
	if err := json.Unmarshal(adapterBytes, &scenario.AdapterConfig); err != nil {
		return domain.Scenario{}, err
	}
	return scenario, nil
}

func (s *Store) CreateRun(ctx context.Context, orgID, projectID, scenarioID uuid.UUID, kind domain.RunKind) (domain.Run, error) {
	var run domain.Run
	err := s.pool.QueryRow(ctx, `
		INSERT INTO runs (org_id, project_id, scenario_id, kind, status)
		VALUES ($1, $2, $3, $4, $5)
		RETURNING id, org_id, project_id, scenario_id, kind, status, created_at
	`, orgID, projectID, scenarioID, kind, domain.RunStatusPending).Scan(
		&run.ID, &run.OrgID, &run.ProjectID, &run.ScenarioID, &run.Kind, &run.Status, &run.CreatedAt,
	)
	return run, err
}

func (s *Store) UpdateRun(ctx context.Context, run domain.Run) error {
	var resultPayload []byte
	var err error
	if run.Result != nil {
		resultPayload, err = json.Marshal(run.Result)
		if err != nil {
			return err
		}
	}

	_, err = s.pool.Exec(ctx, `
		UPDATE runs
		SET status = $2,
		    result = $3::jsonb,
		    error_message = $4,
		    completed_at = $5
		WHERE id = $1 AND org_id = $6
	`, run.ID, run.Status, resultPayload, run.ErrorMessage, run.CompletedAt, run.OrgID)
	return err
}

func (s *Store) GetRun(ctx context.Context, orgID, runID uuid.UUID) (domain.Run, error) {
	var run domain.Run
	var resultPayload []byte
	var completedAt *time.Time
	err := s.pool.QueryRow(ctx, `
		SELECT id, org_id, project_id, scenario_id, kind, status, result, error_message, created_at, completed_at
		FROM runs
		WHERE org_id = $1 AND id = $2
	`, orgID, runID).Scan(
		&run.ID, &run.OrgID, &run.ProjectID, &run.ScenarioID, &run.Kind, &run.Status,
		&resultPayload, &run.ErrorMessage, &run.CreatedAt, &completedAt,
	)
	if err != nil {
		return domain.Run{}, err
	}
	run.CompletedAt = completedAt
	if len(resultPayload) > 0 {
		var result domain.RunResult
		if err := json.Unmarshal(resultPayload, &result); err != nil {
			return domain.Run{}, err
		}
		run.Result = &result
	}
	return run, nil
}

func scanScenario(scan func(dest ...any) error) (domain.Scenario, error) {
	var scenario domain.Scenario
	var cfgBytes []byte
	var adapterBytes []byte
	if err := scan(
		&scenario.ID, &scenario.OrgID, &scenario.ProjectID, &scenario.Name, &scenario.ModelIDs,
		&scenario.Algorithm, &cfgBytes, &adapterBytes, &scenario.CreatedAt, &scenario.UpdatedAt,
	); err != nil {
		return domain.Scenario{}, err
	}
	if err := json.Unmarshal(cfgBytes, &scenario.GenerationConfig); err != nil {
		return domain.Scenario{}, err
	}
	if err := json.Unmarshal(adapterBytes, &scenario.AdapterConfig); err != nil {
		return domain.Scenario{}, err
	}
	return scenario, nil
}

var _ = pgx.ErrNoRows