-- +goose Up
CREATE TABLE scenarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    org_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    project_id UUID NOT NULL REFERENCES projects(id) ON DELETE CASCADE,
    name TEXT NOT NULL,
    model_ids UUID[] NOT NULL DEFAULT '{}',
    algorithm TEXT NOT NULL DEFAULT 'breadth-first',
    generation_config JSONB NOT NULL DEFAULT '{}'::jsonb,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE runs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    org_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    project_id UUID NOT NULL REFERENCES projects(id) ON DELETE CASCADE,
    scenario_id UUID NOT NULL REFERENCES scenarios(id) ON DELETE CASCADE,
    kind TEXT NOT NULL CHECK (kind IN ('generate', 'execute')),
    status TEXT NOT NULL CHECK (status IN ('pending', 'running', 'completed', 'failed')),
    result JSONB,
    error_message TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMPTZ
);

CREATE INDEX idx_scenarios_project_id ON scenarios(project_id);
CREATE INDEX idx_runs_scenario_id ON runs(scenario_id);

-- +goose Down
DROP TABLE IF EXISTS runs;
DROP TABLE IF EXISTS scenarios;