package auth

import (
	"context"
	"net/http"

	"github.com/google/uuid"
)

type contextKey string

const orgIDKey contextKey = "orgID"

func WithOrgID(ctx context.Context, orgID uuid.UUID) context.Context {
	return context.WithValue(ctx, orgIDKey, orgID)
}

func OrgIDFromContext(ctx context.Context) (uuid.UUID, bool) {
	v, ok := ctx.Value(orgIDKey).(uuid.UUID)
	return v, ok
}

// DevMiddleware resolves tenant from X-Org-ID for local development.
// Production OIDC middleware replaces this in a later phase.
func DevMiddleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		raw := r.Header.Get("X-Org-ID")
		if raw == "" {
			http.Error(w, "missing X-Org-ID header", http.StatusUnauthorized)
			return
		}
		orgID, err := uuid.Parse(raw)
		if err != nil {
			http.Error(w, "invalid X-Org-ID header", http.StatusBadRequest)
			return
		}
		next.ServeHTTP(w, r.WithContext(WithOrgID(r.Context(), orgID)))
	})
}