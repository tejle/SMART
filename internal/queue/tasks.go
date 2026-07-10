package queue

import (
	"encoding/json"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/domain"
)

const (
	TypeProcessRun = "run:process"
)

type ProcessRunPayload struct {
	RunID      uuid.UUID      `json:"runId"`
	OrgID      uuid.UUID      `json:"orgId"`
	ProjectID  uuid.UUID      `json:"projectId"`
	ScenarioID uuid.UUID      `json:"scenarioId"`
	Kind       domain.RunKind `json:"kind"`
}

func MarshalProcessRun(payload ProcessRunPayload) ([]byte, error) {
	return json.Marshal(payload)
}

func UnmarshalProcessRun(data []byte) (ProcessRunPayload, error) {
	var payload ProcessRunPayload
	err := json.Unmarshal(data, &payload)
	return payload, err
}