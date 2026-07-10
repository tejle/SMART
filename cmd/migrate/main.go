package main

import (
	"context"
	"database/sql"
	"flag"
	"log"
	"os"

	_ "github.com/jackc/pgx/v5/stdlib"
	"github.com/pressly/goose/v3"
	"github.com/tejle/SMART/internal/config"
)

func main() {
	flag.Parse()
	if flag.NArg() == 0 {
		log.Fatal("usage: migrate <up|down|status>")
	}

	cfg, err := config.Load()
	if err != nil {
		log.Fatalf("config: %v", err)
	}

	db, err := sql.Open("pgx", cfg.DatabaseURL)
	if err != nil {
		log.Fatalf("open db: %v", err)
	}
	defer db.Close()

	if err := db.PingContext(context.Background()); err != nil {
		log.Fatalf("ping db: %v", err)
	}

	if err := goose.SetDialect("postgres"); err != nil {
		log.Fatalf("dialect: %v", err)
	}

	command := flag.Arg(0)
	switch command {
	case "up":
		if err := goose.Up(db, "migrations"); err != nil {
			log.Fatalf("migrate up: %v", err)
		}
	case "down":
		if err := goose.Down(db, "migrations"); err != nil {
			log.Fatalf("migrate down: %v", err)
		}
	case "status":
		if err := goose.Status(db, "migrations"); err != nil {
			log.Fatalf("migrate status: %v", err)
		}
	default:
		log.Fatalf("unknown command: %s", command)
		os.Exit(1)
	}
}