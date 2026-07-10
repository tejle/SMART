.PHONY: deps test lint build api worker migrate-up migrate-down web-dev docker-up

deps:
	go mod tidy

test:
	go test ./...

lint:
	go vet ./...

build:
	go build -o bin/api ./cmd/api
	go build -o bin/worker ./cmd/worker

api:
	go run ./cmd/api

worker:
	go run ./cmd/worker

migrate-up:
	go run ./cmd/migrate up

migrate-down:
	go run ./cmd/migrate down

web-dev:
	cd web && npm run dev

docker-up:
	docker compose up -d