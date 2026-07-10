package domain

import (
	"time"

	"github.com/google/uuid"
)

type Point struct {
	X float64 `json:"x"`
	Y float64 `json:"y"`
}

type Size struct {
	Width  float64 `json:"width"`
	Height float64 `json:"height"`
}

type StateType string

const (
	StateTypeNormal        StateType = "normal"
	StateTypeStart         StateType = "start"
	StateTypeStop          StateType = "stop"
	StateTypeGlobalRef     StateType = "globalRef"
	StateTypeLocalRef      StateType = "localRef"
)

type State struct {
	ID       string    `json:"id"`
	Label    string    `json:"label"`
	Type     StateType `json:"type"`
	Location Point     `json:"location"`
	Size     Size      `json:"size"`
}

type Transition struct {
	ID          string            `json:"id"`
	SourceID    string            `json:"sourceId"`
	TargetID    string            `json:"targetId"`
	Guard       string            `json:"guard,omitempty"`
	Action      string            `json:"action,omitempty"`
	Parameters  map[string]string `json:"parameters,omitempty"`
}

type ModelGraph struct {
	States      []State      `json:"states"`
	Transitions []Transition `json:"transitions"`
}

type Model struct {
	ID        uuid.UUID  `json:"id"`
	OrgID     uuid.UUID  `json:"orgId"`
	ProjectID uuid.UUID  `json:"projectId"`
	Name      string     `json:"name"`
	Graph     ModelGraph `json:"graph"`
	CreatedAt time.Time  `json:"createdAt"`
	UpdatedAt time.Time  `json:"updatedAt"`
}

type CreateModelInput struct {
	Name string `json:"name"`
}

type UpdateModelInput struct {
	Name  *string     `json:"name,omitempty"`
	Graph *ModelGraph `json:"graph,omitempty"`
}

func DefaultModelGraph() ModelGraph {
	return ModelGraph{
		States: []State{
			{
				ID:    "start",
				Label: "Start",
				Type:  StateTypeStart,
				Location: Point{X: 80, Y: 120},
				Size:     Size{Width: 120, Height: 48},
			},
			{
				ID:    "stop",
				Label: "Stop",
				Type:  StateTypeStop,
				Location: Point{X: 420, Y: 120},
				Size:     Size{Width: 120, Height: 48},
			},
		},
		Transitions: []Transition{},
	}
}