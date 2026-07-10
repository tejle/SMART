package runevents

import (
	"encoding/json"
	"sync"

	"github.com/google/uuid"
	"github.com/tejle/SMART/internal/engine"
)

type Hub struct {
	mu      sync.RWMutex
	streams map[uuid.UUID][]chan []byte
}

func NewHub() *Hub {
	return &Hub{streams: make(map[uuid.UUID][]chan []byte)}
}

func (h *Hub) Subscribe(runID uuid.UUID) <-chan []byte {
	ch := make(chan []byte, 16)
	h.mu.Lock()
	h.streams[runID] = append(h.streams[runID], ch)
	h.mu.Unlock()
	return ch
}

func (h *Hub) Publish(runID uuid.UUID, event engine.RunEvent) {
	payload, err := json.Marshal(event)
	if err != nil {
		return
	}
	h.mu.RLock()
	defer h.mu.RUnlock()
	for _, ch := range h.streams[runID] {
		select {
		case ch <- payload:
		default:
		}
	}
}

func (h *Hub) Close(runID uuid.UUID) {
	h.mu.Lock()
	defer h.mu.Unlock()
	for _, ch := range h.streams[runID] {
		close(ch)
	}
	delete(h.streams, runID)
}