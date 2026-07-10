package store

import (
	"context"

	"github.com/google/uuid"
)

type AuditEntry struct {
	OrgID        uuid.UUID
	Actor        string
	Action       string
	ResourceType string
	ResourceID   *uuid.UUID
	Metadata     map[string]any
}

type AuditStore interface {
	RecordAudit(ctx context.Context, entry AuditEntry) error
}