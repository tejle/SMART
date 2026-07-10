package queue

import (
	"context"
	"fmt"

	"github.com/hibiken/asynq"
)

type Client struct {
	inner *asynq.Client
}

func NewClient(redisURL string) (*Client, error) {
	opt, err := asynq.ParseRedisURI(redisURL)
	if err != nil {
		return nil, fmt.Errorf("parse redis url: %w", err)
	}
	return &Client{inner: asynq.NewClient(opt)}, nil
}

func (c *Client) Close() error {
	return c.inner.Close()
}

func (c *Client) EnqueueProcessRun(ctx context.Context, payload ProcessRunPayload) error {
	body, err := MarshalProcessRun(payload)
	if err != nil {
		return err
	}
	task := asynq.NewTask(TypeProcessRun, body)
	_, err = c.inner.EnqueueContext(ctx, task)
	return err
}