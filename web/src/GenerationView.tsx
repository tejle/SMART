import { Background, Controls, MiniMap, ReactFlow } from "@xyflow/react";
import "@xyflow/react/dist/style.css";
import { useEffect, useMemo, useState } from "react";
import { graphToFlow } from "./graph";
import { highlightEdges, highlightNodes } from "./graphStyles";
import SequenceList from "./SequenceList";
import { highlightsUpToIndex, parseTestPath } from "./testSequence";
import type { GenerationResult, Model } from "./types";

type Props = {
  model: Model;
  result: GenerationResult;
  onBack: () => void;
  onViewReport?: () => void;
};

const ANIM_MS = 1400;

export default function GenerationView({ model, result, onBack, onViewReport }: Props) {
  const [selectedPathId, setSelectedPathId] = useState(result.paths[0]?.id ?? "");
  const [animIndex, setAnimIndex] = useState(0);
  const [playing, setPlaying] = useState(true);

  const { nodes: baseNodes, edges: baseEdges } = useMemo(
    () => graphToFlow(model.graph),
    [model.id, model.updatedAt],
  );

  const visitedIds = useMemo(() => new Set(result.visitedStateIds.map((id) => id.split(":").pop() ?? id)), [
    result.visitedStateIds,
  ]);

  const selectedPath = result.paths.find((p) => p.id === selectedPathId) ?? result.paths[0];
  const sequence = useMemo(
    () => (selectedPath ? parseTestPath(selectedPath, model.graph) : []),
    [selectedPath, model.graph],
  );

  useEffect(() => {
    setAnimIndex(0);
    setPlaying(true);
  }, [selectedPathId]);

  useEffect(() => {
    if (!playing || sequence.length === 0) return;
    const timer = window.setInterval(() => {
      setAnimIndex((current) => (current + 1) % sequence.length);
    }, ANIM_MS);
    return () => window.clearInterval(timer);
  }, [playing, sequence.length]);

  const { stateIds, edgeIds, activeStateId, activeEdgeId } = highlightsUpToIndex(sequence, animIndex);
  const activeItem = sequence[animIndex];
  const activeTraverse =
    activeItem?.kind === "traverse"
      ? { fromStateId: activeItem.fromStateId, toStateId: activeItem.toStateId }
      : undefined;

  const nodes = highlightNodes(baseNodes, {
    visitedIds,
    selectedPathIds: stateIds,
    activeId: activeStateId,
  });
  const edges = highlightEdges(baseEdges, {
    pathEdgeIds: edgeIds,
    activeEdgeId,
    activeTraverse,
  });

  const coveragePct = (result.stateCoverageRatio * 100).toFixed(0);

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
        <div style={{ marginLeft: "auto", display: "flex", gap: "0.5rem" }}>
          <button type="button" onClick={() => setPlaying((p) => !p)}>
            {playing ? "Pause" : "Play"}
          </button>
          <button
            type="button"
            onClick={() => {
              setAnimIndex(0);
              setPlaying(true);
            }}
          >
            Restart
          </button>
          {onViewReport && <button onClick={onViewReport}>View report</button>}
        </div>
      </header>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 360px", minHeight: 0 }}>
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
            {visitedIds.size} states validated across all paths
          </p>

          <h3 style={{ margin: "0 0 0.75rem", fontSize: 14 }}>Generated paths</h3>
          <ul style={{ listStyle: "none", padding: 0, margin: "0 0 1rem", display: "grid", gap: "0.5rem" }}>
            {result.paths.map((path) => {
              const selected = path.id === selectedPathId;
              const seq = parseTestPath(path, model.graph);
              const stepCount = seq.filter((s) => s.kind === "validate").length;
              const actionCount = seq.filter((s) => s.kind === "traverse").length;
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
                    <div style={{ fontSize: 11, opacity: 0.85 }}>
                      {stepCount} validations · {actionCount} actions
                    </div>
                  </button>
                </li>
              );
            })}
          </ul>

          {selectedPath && (
            <>
              <h3 style={{ margin: "0 0 0.75rem", fontSize: 14 }}>Test sequence</h3>
              <p style={{ fontSize: 11, opacity: 0.65, margin: "0 0 0.75rem" }}>
                States are validated at rest. Guards are evaluated before traversal. Edges are the actions taken.
              </p>
              <SequenceList items={sequence} activeIndex={animIndex} />
            </>
          )}

          <div style={{ marginTop: "1rem", fontSize: 12, opacity: 0.7 }}>
            <div>Green = validated states</div>
            <div>Gold = path so far · Blue pulse = current step</div>
          </div>
        </aside>
      </div>
    </div>
  );
}