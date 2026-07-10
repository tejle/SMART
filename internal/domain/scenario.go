package domain

import (
	"time"

	"github.com/google/uuid"
)

type GenerationConfig struct {
	StateCoverageThreshold float64 `json:"stateCoverageThreshold,omitempty"`
	MaxSteps               int     `json:"maxSteps,omitempty"`
}

type Scenario struct {
	ID               uuid.UUID        `json:"id"`
	OrgID            uuid.UUID        `json:"orgId"`
	ProjectID        uuid.UUID        `json:"projectId"`
	Name             string           `json:"name"`
	ModelIDs         []uuid.UUID      `json:"modelIds"`
	Algorithm        string           `json:"algorithm"`
	GenerationConfig GenerationConfig `json:"generationConfig"`
	AdapterConfig    AdapterConfig    `json:"adapterConfig"`
	CreatedAt        time.Time        `json:"createdAt"`
	UpdatedAt        time.Time        `json:"updatedAt"`
}

type CreateScenarioInput struct {
	Name             string           `json:"name"`
	ModelIDs         []uuid.UUID      `json:"modelIds"`
	Algorithm        string           `json:"algorithm,omitempty"`
	GenerationConfig GenerationConfig `json:"generationConfig,omitempty"`
	AdapterConfig    AdapterConfig    `json:"adapterConfig,omitempty"`
}

type UpdateScenarioInput struct {
	Name             *string           `json:"name,omitempty"`
	ModelIDs         []uuid.UUID       `json:"modelIds,omitempty"`
	Algorithm        *string           `json:"algorithm,omitempty"`
	GenerationConfig *GenerationConfig `json:"generationConfig,omitempty"`
}

type RunKind string

const (
	RunKindGenerate RunKind = "generate"
	RunKindExecute  RunKind = "execute"
)

type RunStatus string

const (
	RunStatusPending   RunStatus = "pending"
	RunStatusRunning   RunStatus = "running"
	RunStatusCompleted RunStatus = "completed"
	RunStatusFailed    RunStatus = "failed"
)

type TestStep struct {
	StateID    string `json:"stateId"`
	StateLabel string `json:"stateLabel"`
	Action     string `json:"action,omitempty"`
}

type TestPath struct {
	ID    string     `json:"id"`
	Steps []TestStep `json:"steps"`
}

type GenerationResult struct {
	Paths              []TestPath         `json:"paths"`
	VisitedStateIDs    []string           `json:"visitedStateIds"`
	StateCoverageRatio float64            `json:"stateCoverageRatio"`
	Statistics         map[string]float64 `json:"statistics"`
}

type RunResult struct {
	Generation *GenerationResult `json:"generation,omitempty"`
	Execution  *ExecutionResult  `json:"execution,omitempty"`
}

type Run struct {
	ID           uuid.UUID  `json:"id"`
	OrgID        uuid.UUID  `json:"orgId"`
	ProjectID    uuid.UUID  `json:"projectId"`
	ScenarioID   uuid.UUID  `json:"scenarioId"`
	Kind         RunKind    `json:"kind"`
	Status       RunStatus  `json:"status"`
	Result       *RunResult `json:"result,omitempty"`
	ErrorMessage string     `json:"errorMessage,omitempty"`
	CreatedAt    time.Time  `json:"createdAt"`
	CompletedAt  *time.Time `json:"completedAt,omitempty"`
}