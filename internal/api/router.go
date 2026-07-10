package api

import (
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/go-chi/cors"
	"github.com/tejle/SMART/internal/auth"
)

func NewRouter(h *Handler, devAuth bool) http.Handler {
	r := chi.NewRouter()
	r.Use(middleware.RequestID)
	r.Use(middleware.RealIP)
	r.Use(middleware.Logger)
	r.Use(middleware.Recoverer)
	r.Use(cors.Handler(cors.Options{
		AllowedOrigins:   []string{"http://localhost:5173", "http://127.0.0.1:5173"},
		AllowedMethods:   []string{"GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS"},
		AllowedHeaders:   []string{"Accept", "Authorization", "Content-Type", "X-Org-ID"},
		ExposedHeaders:   []string{"Link"},
		AllowCredentials: true,
		MaxAge:           300,
	}))

	r.Get("/healthz", h.Health)

	r.Route("/v1", func(r chi.Router) {
		r.Post("/orgs", h.CreateOrganization)
		r.Get("/catalog/plugins", h.PluginCatalog)

		r.Group(func(r chi.Router) {
			if devAuth {
				r.Use(auth.DevMiddleware)
			}
			r.Post("/projects", h.CreateProject)
			r.Get("/projects", h.ListProjects)
			r.Get("/projects/{projectID}", h.GetProject)

			r.Post("/projects/{projectID}/models", h.CreateModel)
			r.Get("/projects/{projectID}/models", h.ListModels)
			r.Get("/projects/{projectID}/models/{modelID}", h.GetModel)
			r.Put("/projects/{projectID}/models/{modelID}", h.UpdateModel)
			r.Delete("/projects/{projectID}/models/{modelID}", h.DeleteModel)

			r.Post("/projects/{projectID}/scenarios", h.CreateScenario)
			r.Get("/projects/{projectID}/scenarios", h.ListScenarios)
			r.Post("/scenarios/{scenarioID}/runs", h.StartScenarioRun)
			r.Get("/runs/{runID}", h.GetRun)
			r.Get("/runs/{runID}/events", h.StreamRunEvents)
			r.Get("/runs/{runID}/report", h.RunReportHTML)
		})
	})

	return r
}