-- +goose Up
CREATE TABLE models (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    org_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    project_id UUID NOT NULL REFERENCES projects(id) ON DELETE CASCADE,
    name TEXT NOT NULL,
    graph JSONB NOT NULL DEFAULT '{"states":[],"transitions":[]}'::jsonb,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_models_project_id ON models(project_id);
CREATE INDEX idx_models_org_id ON models(org_id);

-- +goose Down
DROP TABLE IF EXISTS models;