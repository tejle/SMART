import { Background, Controls, MiniMap, ReactFlow } from "@xyflow/react";
import "@xyflow/react/dist/style.css";
import { useMemo } from "react";
import { graphToFlow } from "./graph";
import { highlightEdges, highlightNodes } from "./graphStyles";
import type { ExecutionResult, Model, Run } from "./types";

type Props = {
  model: Model;
  run: Run;
  activeStateId?: string;
  log: string[];
  onBack: () => void;
};

export default function ExecutionView({ model, run, activeStateId, log, onBack }: Props) {
  const execution = run.result?.execution;

  const { nodes: baseNodes, edges: baseEdges } = useMemo(
    () => graphToFlow(model.graph),
    [model.id, model.updatedAt],
  );

  const visitedIds = useMemo(() => {
    const ids = new Set<string>();
    execution?.stepResults.forEach((sr) => {
      const id = sr.step.stateId.split(":").pop() ?? sr.step.stateId;
      ids.add(id);
    });
    return ids;
  }, [execution]);

  const failedIds = useMemo(() => {
    const ids = new Set<string>();
    execution?.stepResults.forEach((sr) => {
      if (!sr.success) {
        const id = sr.step.stateId.split(":").pop() ?? sr.step.stateId;
        ids.add(id);
      }
    });
    return ids;
  }, [execution]);

  const nodes = highlightNodes(baseNodes, { visitedIds, activeId: activeStateId, failedIds });
  const edges = highlightEdges(baseEdges, {});

  const totalSteps = execution?.stepResults.length ?? 0;
  const passedSteps = execution?.stepResults.filter((s) => s.success).length ?? 0;
  const progress = totalSteps > 0 ? passedSteps / totalSteps : 0;
  const isRunning = run.status === "pending" || run.status === "running";

  return (
    <div style={{ display: "grid", gridTemplateRows: "auto 1fr", height: "100vh" }}>
      <header
        style={{
          display: "flex",
          gap: "0.75rem",
          alignItems: "center",
          padding: "0.75rem 1rem",
          borderBottom: "1px solid #2a3558",
          flexWrap: "wrap",
        }}
      >
        <button onClick={onBack}>Back</button>
        <strong>Executing {model.name}</strong>
        <span style={{ fontSize: 14, opacity: 0.85 }}>
          {isRunning ? "Running…" : `Completed — ${passedSteps}/${totalSteps} steps passed`}
        </span>
        {execution && execution.defectFlows.length > 0 && (
          <span style={{ fontSize: 14, color: "#f87171" }}>
            {execution.defectFlows.length} defect flow{execution.defectFlows.length === 1 ? "" : "s"}
          </span>
        )}
      </header>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 320px", minHeight: 0 }}>
        <div style={{ minHeight: 0, display: "grid", gridTemplateRows: "1fr auto" }}>
          <ReactFlow nodes={nodes} edges={edges} fitView nodesDraggable={false} nodesConnectable={false}>
            <Background gap={16} color="#1f2937" />
            <MiniMap />
            <Controls showInteractive={false} />
          </ReactFlow>
          <pre
            style={{
              margin: 0,
              maxHeight: 140,
              overflow: "auto",
              padding: "0.75rem 1rem",
              background: "#111827",
              fontSize: 12,
              borderTop: "1px solid #2a3558",
            }}
          >
            {log.length > 0 ? log.join("\n") : "Waiting for execution events…"}
          </pre>
        </div>
        <ExecutionSidebar execution={execution} progress={progress} isRunning={isRunning} />
      </div>
    </div>
  );
}

function ExecutionSidebar({
  execution,
  progress,
  isRunning,
}: {
  execution?: ExecutionResult;
  progress: number;
  isRunning: boolean;
}) {
  return (
    <aside style={{ borderLeft: "1px solid #2a3558", overflow: "auto", padding: "1rem" }}>
      <h3 style={{ margin: "0 0 0.75rem", fontSize: 14 }}>Progress</h3>
      <div
        style={{
          height: 8,
          borderRadius: 4,
          background: "#1e293b",
          overflow: "hidden",
          marginBottom: "0.5rem",
        }}
      >
        <div
          style={{
            width: `${progress * 100}%`,
            height: "100%",
            background: isRunning ? "#60a5fa" : progress >= 1 ? "#4ade80" : "#fbbf24",
            transition: "width 0.3s ease",
          }}
        />
      </div>

      {execution ? (
        <>
          <p style={{ fontSize: 12, opacity: 0.7, margin: "0 0 1rem" }}>
            {execution.completedRounds} round{execution.completedRounds === 1 ? "" : "s"} ·{" "}
            {execution.paths.length} path{execution.paths.length === 1 ? "" : "s"}
          </p>

          <h3 style={{ margin: "0 0 0.75rem", fontSize: 14 }}>Steps</h3>
          <ul style={{ listStyle: "none", padding: 0, margin: 0, display: "grid", gap: "0.35rem" }}>
            {execution.stepResults.map((sr, i) => (
              <li
                key={`${sr.step.stateId}-${i}`}
                style={{
                  padding: "0.5rem 0.65rem",
                  borderRadius: 6,
                  border: "1px solid #2a3558",
                  background: sr.success ? "#14532d33" : "#450a0a55",
                  fontSize: 12,
                }}
              >
                <div style={{ display: "flex", justifyContent: "space-between", gap: "0.5rem" }}>
                  <span>{sr.step.stateLabel}</span>
                  <span style={{ color: sr.success ? "#4ade80" : "#f87171" }}>
                    {sr.success ? "pass" : "fail"}
                  </span>
                </div>
                {sr.step.action && (
                  <div style={{ opacity: 0.7, marginTop: 2 }}>Action: {sr.step.action}</div>
                )}
                {sr.error && <div style={{ color: "#f87171", marginTop: 2 }}>{sr.error}</div>}
              </li>
            ))}
          </ul>

          {execution.defectFlows.length > 0 && (
            <>
              <h3 style={{ margin: "1rem 0 0.75rem", fontSize: 14, color: "#f87171" }}>Defect flows</h3>
              <ul style={{ listStyle: "none", padding: 0, margin: 0, display: "grid", gap: "0.5rem" }}>
                {execution.defectFlows.map((flow) => (
                  <li
                    key={flow.id}
                    style={{
                      padding: "0.5rem 0.65rem",
                      borderRadius: 6,
                      border: "1px solid #7f1d1d",
                      fontSize: 11,
                    }}
                  >
                    {flow.steps.map((s) => s.stateLabel).join(" → ")}
                  </li>
                ))}
              </ul>
            </>
          )}
        </>
      ) : (
        <p style={{ fontSize: 12, opacity: 0.6 }}>Execution results will appear here.</p>
      )}

      <div style={{ marginTop: "1rem", fontSize: 12, opacity: 0.7 }}>
        <div>Green = visited · Blue = active</div>
        <div>Red = failed step</div>
      </div>
    </aside>
  );
}