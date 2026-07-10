package engine

import (
	"context"

	"github.com/tejle/SMART/internal/domain"
	"github.com/tejle/SMART/internal/plugins"
)

type StepExecutor interface {
	Execute(ctx context.Context, step domain.TestStep) domain.StepResult
}

type RunEvent struct {
	Type    string           `json:"type"`
	Round   int              `json:"round"`
	Step    *domain.TestStep `json:"step,omitempty"`
	Success *bool            `json:"success,omitempty"`
	Message string           `json:"message,omitempty"`
}

func Execute(
	ctx context.Context,
	paths []domain.TestPath,
	adapter domain.AdapterConfig,
	emit func(RunEvent),
) (domain.ExecutionResult, error) {
	executor, err := plugins.NewHTTPAdapter(adapter)
	if err != nil {
		return domain.ExecutionResult{}, err
	}

	result := domain.ExecutionResult{Paths: paths, CompletedRounds: 1}
	if emit != nil {
		emit(RunEvent{Type: "run_started", Round: 1, Message: "execution started"})
	}

	for round := 1; round <= 5; round++ {
		result.CompletedRounds = round
		for _, path := range paths {
			for _, step := range path.Steps {
				if emit != nil {
					emit(RunEvent{Type: "step_started", Round: round, Step: &step})
				}
				stepResult := executor.Execute(ctx, step)
				result.StepResults = append(result.StepResults, stepResult)
				if emit != nil {
					emit(RunEvent{Type: "step_finished", Round: round, Step: &step, Success: &stepResult.Success})
				}
				if !stepResult.Success {
					result.DefectFlows = append(result.DefectFlows, path)
					if emit != nil {
						emit(RunEvent{Type: "defect_recorded", Round: round, Message: stepResult.Error})
					}
				}
			}
		}
		if len(result.DefectFlows) == 0 || round == 5 {
			break
		}
		if emit != nil {
			emit(RunEvent{Type: "regenerate_round", Round: round + 1, Message: "adaptive re-generation"})
		}
	}

	if emit != nil {
		emit(RunEvent{Type: "run_finished", Round: result.CompletedRounds, Message: "execution completed"})
	}
	return result, nil
}