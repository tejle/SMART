import { openRunReport } from "./api";
import type { Run, Scenario } from "./types";

type Props = {
  runs: Run[];
  scenarios: Scenario[];
  onOpenGeneration: (run: Run) => void;
  onOpenExecution: (run: Run) => void;
  onError: (message: string) => void;
};

function scenarioName(scenarios: Scenario[], scenarioId: string): string {
  return scenarios.find((s) => s.id === scenarioId)?.name ?? scenarioId.slice(0, 8);
}

function formatWhen(iso: string): string {
  return new Date(iso).toLocaleString();
}

function runSummary(run: Run): string {
  if (run.kind === "generate" && run.result?.generation) {
    const gen = run.result.generation;
    return `${gen.paths.length} paths · ${(gen.stateCoverageRatio * 100).toFixed(0)}% coverage`;
  }
  if (run.kind === "execute" && run.result?.execution) {
    const ex = run.result.execution;
    const passed = ex.stepResults.filter((s) => s.success).length;
    return `${passed}/${ex.stepResults.length} passed · ${ex.defectFlows.length} defects`;
  }
  if (run.errorMessage) return run.errorMessage;
  return run.status;
}

const linkButtonStyle = {
  background: "none",
  border: "none",
  color: "#60a5fa",
  cursor: "pointer",
  padding: 0,
  textDecoration: "underline",
  font: "inherit",
  fontSize: 12,
} as const;

export default function RunHistory({ runs, scenarios, onOpenGeneration, onOpenExecution, onError }: Props) {
  if (runs.length === 0) {
    return <p style={{ opacity: 0.7, fontSize: 14 }}>No runs yet for this project.</p>;
  }

  return (
    <ul style={{ listStyle: "none", padding: 0, margin: 0, display: "grid", gap: "0.5rem" }}>
      {runs.map((run) => (
        <li
          key={run.id}
          style={{
            padding: "0.75rem 1rem",
            border: "1px solid #2a3558",
            borderRadius: 8,
            fontSize: 13,
          }}
        >
          <div style={{ display: "flex", justifyContent: "space-between", gap: "0.75rem", flexWrap: "wrap" }}>
            <div>
              <strong style={{ textTransform: "capitalize" }}>{run.kind}</strong>
              <span style={{ opacity: 0.7 }}> · {scenarioName(scenarios, run.scenarioId)}</span>
            </div>
            <span
              style={{
                fontSize: 12,
                color:
                  run.status === "completed"
                    ? "#4ade80"
                    : run.status === "failed"
                      ? "#f87171"
                      : "#fbbf24",
              }}
            >
              {run.status}
            </span>
          </div>
          <div style={{ fontSize: 12, opacity: 0.8, marginTop: 4 }}>{runSummary(run)}</div>
          <div style={{ fontSize: 11, opacity: 0.55, marginTop: 2 }}>{formatWhen(run.createdAt)}</div>
          {run.status === "completed" && (
            <div style={{ display: "flex", gap: "0.75rem", marginTop: 8, flexWrap: "wrap" }}>
              {run.kind === "generate" && run.result?.generation && (
                <button type="button" style={linkButtonStyle} onClick={() => onOpenGeneration(run)}>
                  View generation
                </button>
              )}
              {run.kind === "execute" && run.result?.execution && (
                <button type="button" style={linkButtonStyle} onClick={() => onOpenExecution(run)}>
                  View execution
                </button>
              )}
              <button
                type="button"
                style={linkButtonStyle}
                onClick={() => openRunReport(run.id).catch((err: Error) => onError(err.message))}
              >
                Report
              </button>
            </div>
          )}
        </li>
      ))}
    </ul>
  );
}