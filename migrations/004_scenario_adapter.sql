-- +goose Up
ALTER TABLE scenarios
    ADD COLUMN adapter_config JSONB NOT NULL DEFAULT '{"baseUrl":""}'::jsonb;

-- +goose Down
ALTER TABLE scenarios DROP COLUMN IF EXISTS adapter_config;