package plugins

type PluginInfo struct {
	ID          string `json:"id"`
	Kind        string `json:"kind"`
	Name        string `json:"name"`
	Description string `json:"description"`
	Version     string `json:"version"`
}

func Catalog() []PluginInfo {
	return []PluginInfo{
		{
			ID:          "breadth-first",
			Kind:        "algorithm",
			Name:        "Breadth first",
			Description: "Explore the model breadth-first to build test paths",
			Version:     "1.0",
		},
		{
			ID:          "random",
			Kind:        "algorithm",
			Name:        "Random",
			Description: "Walk the model randomly until stop criteria are met",
			Version:     "1.0",
		},
		{
			ID:          "state-coverage",
			Kind:        "generation-stop-criteria",
			Name:        "State coverage",
			Description: "Stop when a target ratio of normal states has been visited",
			Version:     "1.0",
		},
		{
			ID:          "step-count",
			Kind:        "generation-stop-criteria",
			Name:        "Step count",
			Description: "Stop after a maximum number of generation steps",
			Version:     "1.0",
		},
		{
			ID:          "http",
			Kind:        "adapter",
			Name:        "HTTP adapter",
			Description: "Execute generated steps against HTTP endpoints",
			Version:     "1.0",
		},
	}
}