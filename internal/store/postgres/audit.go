package postgres

import (
	"context"
	"encoding/json"

	"github.com/tejle/SMART/internal/store"
)

func (s *Store) RecordAudit(ctx context.Context, entry store.AuditEntry) error {
	meta, err := json.Marshal(entry.Metadata)
	if err != nil {
		meta = []byte("{}")
	}
	_, err = s.pool.Exec(ctx, `
		INSERT INTO audit_log (org_id, actor, action, resource_type, resource_id, metadata)
		VALUES ($1, $2, $3, $4, $5, $6::jsonb)
	`, entry.OrgID, entry.Actor, entry.Action, entry.ResourceType, entry.ResourceID, meta)
	return err
}