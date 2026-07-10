package queue

import (
	"context"
	"fmt"

	"github.com/hibiken/asynq"
	"github.com/tejle/SMART/internal/runner"
	"github.com/tejle/SMART/internal/store"
)

type Server struct {
	server *asynq.Server
	mux    *asynq.ServeMux
}

func NewServer(redisURL string, store store.Store, runRunner *runner.Runner) (*Server, error) {
	opt, err := asynq.ParseRedisURI(redisURL)
	if err != nil {
		return nil, fmt.Errorf("parse redis url: %w", err)
	}

	srv := asynq.NewServer(opt, asynq.Config{Concurrency: 4})
	mux := asynq.NewServeMux()
	mux.HandleFunc(TypeProcessRun, func(ctx context.Context, task *asynq.Task) error {
		payload, err := UnmarshalProcessRun(task.Payload())
		if err != nil {
			return err
		}

		run, err := store.GetRun(ctx, payload.OrgID, payload.RunID)
		if err != nil {
			return err
		}
		scenario, err := store.GetScenario(ctx, payload.OrgID, payload.ProjectID, payload.ScenarioID)
		if err != nil {
			return err
		}
		return runRunner.ProcessRun(ctx, run, scenario, payload.OrgID, payload.ProjectID)
	})

	return &Server{server: srv, mux: mux}, nil
}

func (s *Server) Run() error {
	return s.server.Run(s.mux)
}

func (s *Server) Shutdown() {
	s.server.Shutdown()
}