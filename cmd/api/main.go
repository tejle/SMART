package main

import (
	"context"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/tejle/SMART/internal/api"
	"github.com/tejle/SMART/internal/config"
	"github.com/tejle/SMART/internal/queue"
	"github.com/tejle/SMART/internal/runevents"
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

	queueClient, err := queue.NewClient(cfg.RedisURL)
	if err != nil {
		log.Fatalf("queue client: %v", err)
	}
	defer queueClient.Close()

	handler := api.NewHandler(db, hub, redisBridge, queueClient)
	router := api.NewRouter(handler, cfg.DevAuth)

	server := &http.Server{
		Addr:              cfg.HTTPAddr,
		Handler:           router,
		ReadHeaderTimeout: 5 * time.Second,
	}

	go func() {
		log.Printf("api listening on %s", cfg.HTTPAddr)
		if err := server.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			log.Fatalf("server: %v", err)
		}
	}()

	stop := make(chan os.Signal, 1)
	signal.Notify(stop, syscall.SIGINT, syscall.SIGTERM)
	<-stop

	shutdownCtx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()
	if err := server.Shutdown(shutdownCtx); err != nil {
		log.Printf("shutdown: %v", err)
	}
}