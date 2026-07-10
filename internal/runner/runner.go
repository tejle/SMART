package runner

import (
	"context"
	"math/rand"
	"time"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/domain"
	"github.com/tejle/SMART/internal/engine"
	"github.com/tejle/SMART/internal/store"
)

type EventPublisher interface {
	Publish(ctx context.Context, runID uuid.UUID, event engine.RunEvent) error
	PublishClose(ctx context.Context, runID uuid.UUID) error
}

type Runner struct {
	store   store.Store
	events  EventPublisher
}

func New(store store.Store, events EventPublisher) *Runner {
	return &Runner{store: store, events: events}
}

func (r *Runner) ProcessRun(ctx context.Context, run domain.Run, scenario domain.Scenario, orgID, projectID uuid.UUID) error {
	run.Status = domain.RunStatusRunning
	_ = r.store.UpdateRun(ctx, run)

	var runResult domain.RunResult
	var runErr error

	switch run.Kind {
	case domain.RunKindGenerate:
		generation, err := r.generate(ctx, orgID, projectID, scenario)
		if err != nil {
			runErr = err
		} else {
			runResult.Generation = &generation
		}
	case domain.RunKindExecute:
		generation, err := r.generate(ctx, orgID, projectID, scenario)
		if err != nil {
			runErr = err
		} else {
			emit := func(event engine.RunEvent) {
				_ = r.events.Publish(ctx, run.ID, event)
			}
			execution, err := engine.Execute(ctx, generation.Paths, scenario.AdapterConfig, emit)
			if err != nil {
				runErr = err
			} else {
				runResult.Generation = &generation
				runResult.Execution = &execution
			}
		}
	default:
		runErr = domain.ErrUnsupportedRunKind
	}

	now := time.Now().UTC()
	run.CompletedAt = &now
	if runErr != nil {
		run.Status = domain.RunStatusFailed
		run.ErrorMessage = runErr.Error()
	} else {
		run.Status = domain.RunStatusCompleted
		run.Result = &runResult
	}
	if err := r.store.UpdateRun(ctx, run); err != nil {
		return err
	}
	_ = r.events.PublishClose(ctx, run.ID)
	return runErr
}

func (r *Runner) generate(ctx context.Context, orgID, projectID uuid.UUID, scenario domain.Scenario) (domain.GenerationResult, error) {
	var models []domain.Model
	for _, modelID := range scenario.ModelIDs {
		model, err := r.store.GetModel(ctx, orgID, projectID, modelID)
		if err != nil {
			return domain.GenerationResult{}, err
		}
		models = append(models, model)
	}
	graph, err := engine.Compile(models)
	if err != nil {
		return domain.GenerationResult{}, err
	}
	return engine.Generate(graph, scenario.Algorithm, scenario.GenerationConfig, rand.New(rand.NewSource(time.Now().UnixNano())))
}