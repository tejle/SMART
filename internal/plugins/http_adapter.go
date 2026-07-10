package plugins

import (
	"context"
	"fmt"
	"io"
	"net/http"
	"strings"
	"time"

	"github.com/tejle/SMART/internal/domain"
)

type HTTPAdapter struct {
	client  *http.Client
	baseURL string
}

func NewHTTPAdapter(cfg domain.AdapterConfig) (*HTTPAdapter, error) {
	if strings.TrimSpace(cfg.BaseURL) == "" {
		return nil, fmt.Errorf("adapter baseUrl is required")
	}
	timeout := 10 * time.Second
	if cfg.Timeout > 0 {
		timeout = time.Duration(cfg.Timeout) * time.Second
	}
	return &HTTPAdapter{
		client:  &http.Client{Timeout: timeout},
		baseURL: strings.TrimRight(cfg.BaseURL, "/"),
	}, nil
}

func (a *HTTPAdapter) Execute(ctx context.Context, step domain.TestStep) domain.StepResult {
	result := domain.StepResult{Step: step, Success: false}
	action := strings.TrimSpace(step.Action)
	if action == "" {
		action = strings.TrimSpace(step.StateLabel)
	}
	if action == "" {
		result.Error = "step has no action"
		return result
	}

	url := a.baseURL
	if strings.HasPrefix(action, "http://") || strings.HasPrefix(action, "https://") {
		url = action
	} else if strings.HasPrefix(action, "/") {
		url = a.baseURL + action
	} else {
		url = a.baseURL + "/" + action
	}

	req, err := http.NewRequestWithContext(ctx, http.MethodPost, url, http.NoBody)
	if err != nil {
		result.Error = err.Error()
		return result
	}

	resp, err := a.client.Do(req)
	if err != nil {
		result.Error = err.Error()
		return result
	}
	defer resp.Body.Close()
	_, _ = io.Copy(io.Discard, resp.Body)

	result.StatusCode = resp.StatusCode
	result.Success = resp.StatusCode >= 200 && resp.StatusCode < 300
	if !result.Success {
		result.Error = fmt.Sprintf("unexpected status %d", resp.StatusCode)
	}
	return result
}