import { Background, Controls, MiniMap, ReactFlow } from "@xyflow/react";
import "@xyflow/react/dist/style.css";
import { useMemo, useState } from "react";
import { graphToFlow } from "./graph";
import { highlightEdges, highlightNodes, pathEdgeIdsForSteps } from "./graphStyles";
import type { GenerationResult, Model } from "./types";

type Props = {
  model: Model;
  result: GenerationResult;
  onBack: () => void;
  onViewReport?: () => void;
};

export default function GenerationView({ model, result, onBack, onViewReport }: Props) {
  const [selectedPathId, setSelectedPathId] = useState(result.paths[0]?.id ?? "");

  const { nodes: baseNodes, edges: baseEdges } = useMemo(
    () => graphToFlow(model.graph),
    [model.id, model.updatedAt],
  );

  const visitedIds = useMemo(() => new Set(result.visitedStateIds), [result.visitedStateIds]);

  const selectedPath = result.paths.find((p) => p.id === selectedPathId) ?? result.paths[0];
  const selectedStateIds = useMemo(
    () => new Set(selectedPath?.steps.map((s) => s.stateId.split(":").pop() ?? s.stateId) ?? []),
    [selectedPath],
  );
  const pathEdgeIds = useMemo(
    () =>
      pathEdgeIdsForSteps(
        baseEdges,
        selectedPath?.steps.map((s) => s.stateId.split(":").pop() ?? s.stateId) ?? [],
      ),
    [baseEdges, selectedPath],
  );

  const nodes = highlightNodes(baseNodes, { visitedIds, selectedPathIds: selectedStateIds });
  const edges = highlightEdges(baseEdges, { pathEdgeIds });

  const coveragePct = (result.stateCoverageRatio * 100).toFixed(0);
  const stats = result.statistics ?? {};

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
        <strong>Generation — {model.name}</strong>
        <span style={{ fontSize: 14, opacity: 0.85 }}>
          {result.paths.length} paths · {coveragePct}% state coverage
        </span>
        {onViewReport && (
          <button onClick={onViewReport} style={{ marginLeft: "auto" }}>
            View report
          </button>
        )}
      </header>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 320px", minHeight: 0 }}>
        <div style={{ minHeight: 0 }}>
          <ReactFlow nodes={nodes} edges={edges} fitView nodesDraggable={false} nodesConnectable={false}>
            <Background gap={16} color="#1f2937" />
            <MiniMap />
            <Controls showInteractive={false} />
          </ReactFlow>
        </div>
        <aside style={{ borderLeft: "1px solid #2a3558", overflow: "auto", padding: "1rem" }}>
          <h3 style={{ margin: "0 0 0.75rem", fontSize: 14 }}>Coverage</h3>
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
                width: `${result.stateCoverageRatio * 100}%`,
                height: "100%",
                background: result.stateCoverageRatio >= 1 ? "#4ade80" : "#60a5fa",
              }}
            />
          </div>
          <p style={{ fontSize: 12, opacity: 0.7, margin: "0 0 1rem" }}>
            {visitedIds.size} states visited · {stats.visitedStates ?? visitedIds.size} in statistics
          </p>

          <h3 style={{ margin: "0 0 0.75rem", fontSize: 14 }}>Generated paths</h3>
          <ul style={{ listStyle: "none", padding: 0, margin: 0, display: "grid", gap: "0.5rem" }}>
            {result.paths.map((path) => {
              const labels = path.steps.map((s) => s.stateLabel);
              const selected = path.id === selectedPathId;
              return (
                <li key={path.id}>
                  <button
                    type="button"
                    onClick={() => setSelectedPathId(path.id)}
                    style={{
                      width: "100%",
                      textAlign: "left",
                      padding: "0.65rem 0.75rem",
                      border: selected ? "1px solid #fbbf24" : "1px solid #2a3558",
                      borderRadius: 8,
                      background: selected ? "#422006" : "transparent",
                    }}
                  >
                    <div style={{ fontSize: 12, fontWeight: 600, marginBottom: 4 }}>{path.id}</div>
                    <div style={{ fontSize: 11, opacity: 0.85 }}>{labels.join(" → ")}</div>
                  </button>
                </li>
              );
            })}
          </ul>

          <div style={{ marginTop: "1rem", fontSize: 12, opacity: 0.7 }}>
            <div>Green = visited states</div>
            <div>Gold = selected path</div>
          </div>
        </aside>
      </div>
    </div>
  );
}