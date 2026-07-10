package main

import (
	"context"
	"log"
	"os"
	"os/signal"
	"syscall"

	"github.com/tejle/SMART/internal/config"
	"github.com/tejle/SMART/internal/queue"
	"github.com/tejle/SMART/internal/runevents"
	"github.com/tejle/SMART/internal/runner"
	"github.com/tejle/SMART/internal/store/postgres"
)

func main() {
	cfg, err := config.Load()
	if err != nil {
		log.Fatalf("config: %v", err)
	}

	ctx := context.Background()
	db, err := postgres.New(ctx, cfg.DatabaseURL)
	if err != nil {
		log.Fatalf("database: %v", err)
	}
	defer db.Close()

	hub := runevents.NewHub()
	redisBridge, err := runevents.NewRedisBridge(cfg.RedisURL, hub)
	if err != nil {
		log.Fatalf("redis: %v", err)
	}
	defer redisBridge.Close()

	runRunner := runner.New(db, redisBridge)
	srv, err := queue.NewServer(cfg.RedisURL, db, runRunner)
	if err != nil {
		log.Fatalf("queue server: %v", err)
	}

	go func() {
		log.Printf("worker listening on redis=%s", cfg.RedisURL)
		if err := srv.Run(); err != nil {
			log.Fatalf("worker: %v", err)
		}
	}()

	stop := make(chan os.Signal, 1)
	signal.Notify(stop, syscall.SIGINT, syscall.SIGTERM)
	<-stop
	srv.Shutdown()
	log.Printf("worker stopped")
}