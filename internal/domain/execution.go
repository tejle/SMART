package domain

type AdapterConfig struct {
	BaseURL string `json:"baseUrl"`
	Timeout int    `json:"timeoutSeconds,omitempty"`
}

type StepResult struct {
	Step       TestStep `json:"step"`
	Success    bool     `json:"success"`
	StatusCode int      `json:"statusCode,omitempty"`
	Error      string   `json:"error,omitempty"`
}

type ExecutionResult struct {
	Paths           []TestPath   `json:"paths"`
	StepResults     []StepResult `json:"stepResults"`
	DefectFlows     []TestPath   `json:"defectFlows"`
	CompletedRounds int          `json:"completedRounds"`
}