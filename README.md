# SMART — Model-Based Testing SaaS

SMART is being revived as a cloud-native, multi-tenant SaaS for model-based testing (MBT). Users model requirements as state machines, configure generation and execution strategies, and run adaptive test paths in the browser.

The legacy WPF desktop codebase lives under [`legacy/`](legacy/) as a read-only behavioral reference.

## Stack

- **API / worker:** Go (chi, pgx, goose)
- **Frontend:** React 18 + TypeScript + Vite
- **Database:** PostgreSQL
- **Queue (planned):** Redis + Asynq

## Quick start

```bash
# Start dependencies
make docker-up

# Run migrations
make migrate-up

# API + worker (worker processes queued runs via Redis/Asynq)
make api
make worker   # separate terminal — required for generate/execute jobs

# Web UI
make web-dev
```

Create an organization in the web UI, then create projects. The API expects `X-Org-ID` in development mode (`DEV_AUTH=true`).

### Environment

| Variable | Default |
|----------|---------|
| `HTTP_ADDR` | `:8080` |
| `DATABASE_URL` | `postgres://smart:smart@localhost:5432/smart?sslmode=disable` |
| `REDIS_URL` | `redis://localhost:6379` |
| `DEV_AUTH` | `true` |

## Development

```bash
make test
make build
```

OpenAPI contract: [`api/openapi.yaml`](api/openapi.yaml)

## Legacy desktop app

The original .NET Framework WPF tool enabled early MBT via graphical requirement models. See `src/` for the reference implementation.