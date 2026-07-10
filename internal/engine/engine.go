package engine

import (
	"fmt"
	"math/rand"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/domain"
)

type CompiledGraph struct {
	States      map[string]domain.State
	Transitions []domain.Transition
	StartID     string
	StopID      string
}

func Compile(models []domain.Model) (CompiledGraph, error) {
	if len(models) == 0 {
		return CompiledGraph{}, fmt.Errorf("at least one model is required")
	}

	compiled := CompiledGraph{States: make(map[string]domain.State)}
	for _, model := range models {
		for _, state := range model.Graph.States {
			key := fmt.Sprintf("%s:%s", model.ID.String(), state.ID)
			copy := state
			copy.ID = key
			compiled.States[key] = copy
			if state.Type == domain.StateTypeStart {
				compiled.StartID = key
			}
			if state.Type == domain.StateTypeStop {
				compiled.StopID = key
			}
		}
		for _, transition := range model.Graph.Transitions {
			compiled.Transitions = append(compiled.Transitions, domain.Transition{
				ID:       fmt.Sprintf("%s:%s", model.ID.String(), transition.ID),
				SourceID: fmt.Sprintf("%s:%s", model.ID.String(), transition.SourceID),
				TargetID: fmt.Sprintf("%s:%s", model.ID.String(), transition.TargetID),
				Guard:    transition.Guard,
				Action:   transition.Action,
			})
		}
	}

	if compiled.StartID == "" || compiled.StopID == "" {
		return CompiledGraph{}, fmt.Errorf("compiled graph must include start and stop states")
	}
	return compiled, nil
}

func Generate(graph CompiledGraph, algorithm string, cfg domain.GenerationConfig, rng *rand.Rand) (domain.GenerationResult, error) {
	if cfg.MaxSteps <= 0 {
		cfg.MaxSteps = 200
	}
	if cfg.StateCoverageThreshold <= 0 {
		cfg.StateCoverageThreshold = 1.0
	}

	var paths []domain.TestPath
	visited := map[string]bool{}

	switch algorithm {
	case "random":
		paths = generateRandom(graph, cfg, visited, rng)
	default:
		paths = generateBreadthFirst(graph, cfg, visited)
	}

	normalStates := 0
	for _, state := range graph.States {
		if state.Type == domain.StateTypeNormal {
			normalStates++
		}
	}
	covered := 0
	for id := range visited {
		state, ok := graph.States[id]
		if ok && state.Type == domain.StateTypeNormal {
			covered++
		}
	}
	ratio := 1.0
	if normalStates > 0 {
		ratio = float64(covered) / float64(normalStates)
	}

	visitedIDs := make([]string, 0, len(visited))
	for id := range visited {
		visitedIDs = append(visitedIDs, id)
	}

	return domain.GenerationResult{
		Paths:              paths,
		VisitedStateIDs:    visitedIDs,
		StateCoverageRatio: ratio,
		Statistics: map[string]float64{
			"paths":          float64(len(paths)),
			"visitedStates":  float64(len(visitedIDs)),
			"stateCoverage":  ratio,
		},
	}, nil
}

func generateBreadthFirst(graph CompiledGraph, cfg domain.GenerationConfig, visited map[string]bool) []domain.TestPath {
	type frame struct {
		stateID string
		steps   []domain.TestStep
	}

	queue := []frame{{stateID: graph.StartID, steps: nil}}
	var paths []domain.TestPath
	expanded := 0

	for len(queue) > 0 && expanded < cfg.MaxSteps {
		current := queue[0]
		queue = queue[1:]
		expanded++

		state, ok := graph.States[current.stateID]
		if !ok {
			continue
		}
		visited[current.stateID] = true
		nextSteps := append(append([]domain.TestStep{}, current.steps...), domain.TestStep{
			StateID: state.ID, StateLabel: state.Label,
		})

		if state.Type == domain.StateTypeStop {
			paths = append(paths, domain.TestPath{ID: uuid.NewString(), Steps: nextSteps})
			continue
		}

		for _, transition := range outgoing(graph, current.stateID) {
			target := graph.States[transition.TargetID]
			queue = append(queue, frame{
				stateID: transition.TargetID,
				steps: append(nextSteps, domain.TestStep{
					StateID: transition.TargetID, StateLabel: target.Label, Action: transition.Action,
				}),
			})
		}
	}

	if len(paths) == 0 {
		start := graph.States[graph.StartID]
		paths = append(paths, domain.TestPath{
			ID: uuid.NewString(),
			Steps: []domain.TestStep{{StateID: start.ID, StateLabel: start.Label}},
		})
	}
	return paths
}

func generateRandom(graph CompiledGraph, cfg domain.GenerationConfig, visited map[string]bool, rng *rand.Rand) []domain.TestPath {
	var paths []domain.TestPath
	currentID := graph.StartID
	steps := []domain.TestStep{}

	for stepCount := 0; stepCount < cfg.MaxSteps; stepCount++ {
		state := graph.States[currentID]
		visited[currentID] = true
		steps = append(steps, domain.TestStep{StateID: state.ID, StateLabel: state.Label})

		if state.Type == domain.StateTypeStop {
			paths = append(paths, domain.TestPath{ID: uuid.NewString(), Steps: append([]domain.TestStep{}, steps...)})
			break
		}

		options := outgoing(graph, currentID)
		if len(options) == 0 {
			break
		}
		pick := options[rng.Intn(len(options))]
		steps = append(steps, domain.TestStep{StateID: pick.TargetID, Action: pick.Action})
		currentID = pick.TargetID
	}

	return paths
}

func outgoing(graph CompiledGraph, stateID string) []domain.Transition {
	var out []domain.Transition
	for _, transition := range graph.Transitions {
		if transition.SourceID == stateID {
			out = append(out, transition)
		}
	}
	return out
}

