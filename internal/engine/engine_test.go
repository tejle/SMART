package engine

import (
	"math/rand"
	"testing"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/domain"
)

func demoModel() domain.Model {
	return domain.Model{
		ID: uuid.New(),
		Graph: domain.ModelGraph{
			States: []domain.State{
				{ID: "start", Label: "Start", Type: domain.StateTypeStart, Location: domain.Point{X: 0, Y: 0}, Size: domain.Size{Width: 100, Height: 40}},
				{ID: "a", Label: "Login", Type: domain.StateTypeNormal, Location: domain.Point{X: 100, Y: 0}, Size: domain.Size{Width: 100, Height: 40}},
				{ID: "stop", Label: "Stop", Type: domain.StateTypeStop, Location: domain.Point{X: 300, Y: 0}, Size: domain.Size{Width: 100, Height: 40}},
			},
			Transitions: []domain.Transition{
				{ID: "t1", SourceID: "start", TargetID: "a", Action: "open"},
				{ID: "t2", SourceID: "a", TargetID: "stop", Action: "submit"},
			},
		},
	}
}

func TestCompileAndGenerateBreadthFirst(t *testing.T) {
	graph, err := Compile([]domain.Model{demoModel()})
	if err != nil {
		t.Fatalf("compile: %v", err)
	}

	result, err := Generate(graph, "breadth-first", domain.GenerationConfig{MaxSteps: 50}, rand.New(rand.NewSource(1)))
	if err != nil {
		t.Fatalf("generate: %v", err)
	}
	if len(result.Paths) == 0 {
		t.Fatalf("expected at least one path")
	}
	if result.StateCoverageRatio <= 0 {
		t.Fatalf("expected positive coverage")
	}
}