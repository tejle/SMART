package runevents

import (
	"context"
	"encoding/json"
	"fmt"

	"github.com/google/uuid"
	"github.com/redis/go-redis/v9"
	"github.com/tejle/SMART/internal/engine"
)

type RedisBridge struct {
	client *redis.Client
	local  *Hub
}

func NewRedisBridge(redisURL string, local *Hub) (*RedisBridge, error) {
	opt, err := redis.ParseURL(redisURL)
	if err != nil {
		return nil, fmt.Errorf("parse redis url: %w", err)
	}
	return &RedisBridge{
		client: redis.NewClient(opt),
		local:  local,
	}, nil
}

func (b *RedisBridge) Close() error {
	return b.client.Close()
}

func runChannel(runID uuid.UUID) string {
	return fmt.Sprintf("smart:run:%s", runID.String())
}

func (b *RedisBridge) Publish(ctx context.Context, runID uuid.UUID, event engine.RunEvent) error {
	payload, err := json.Marshal(event)
	if err != nil {
		return err
	}
	if err := b.client.Publish(ctx, runChannel(runID), payload).Err(); err != nil {
		return err
	}
	b.local.Publish(runID, event)
	return nil
}

func (b *RedisBridge) Subscribe(ctx context.Context, runID uuid.UUID) (*redis.PubSub, error) {
	pubsub := b.client.Subscribe(ctx, runChannel(runID))
	if _, err := pubsub.Receive(ctx); err != nil {
		_ = pubsub.Close()
		return nil, err
	}
	return pubsub, nil
}

func (b *RedisBridge) PublishClose(ctx context.Context, runID uuid.UUID) error {
	return b.client.Publish(ctx, runChannel(runID), []byte(`{"type":"run_finished"}`)).Err()
}