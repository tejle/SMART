package engine

import (
	"encoding/json"
	"math/rand"
	"os"
	"path/filepath"
	"testing"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/domain"
)

func TestGoldenLoginFlowBreadthFirst(t *testing.T) {
	path := filepath.Join("testdata", "login_flow.json")
	raw, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read fixture: %v", err)
	}

	var graph domain.ModelGraph
	if err := json.Unmarshal(raw, &graph); err != nil {
		t.Fatalf("parse fixture: %v", err)
	}

	compiled, err := Compile([]domain.Model{{
		ID:    uuid.New(),
		Graph: graph,
	}})
	if err != nil {
		t.Fatalf("compile: %v", err)
	}

	result, err := Generate(compiled, "breadth-first", domain.GenerationConfig{MaxSteps: 100}, rand.New(rand.NewSource(42)))
	if err != nil {
		t.Fatalf("generate: %v", err)
	}

	if len(result.Paths) == 0 {
		t.Fatal("expected at least one generated path")
	}
	last := result.Paths[0].Steps[len(result.Paths[0].Steps)-1]
	if last.StateLabel != "Stop" {
		t.Fatalf("expected terminal Stop state, got %q", last.StateLabel)
	}
	if result.StateCoverageRatio < 0.5 {
		t.Fatalf("expected reasonable coverage, got %.2f", result.StateCoverageRatio)
	}
}