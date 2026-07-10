import {
  Background,
  Controls,
  Edge,
  MiniMap,
  Node,
  ReactFlow,
} from "@xyflow/react";
import "@xyflow/react/dist/style.css";
import { useMemo } from "react";
import { FlowNodeData, graphToFlow } from "./graph";
import type { Model } from "./types";

type Props = {
  model: Model;
  activeStateId?: string;
  log: string[];
  onBack: () => void;
};

export default function ExecutionView({ model, activeStateId, log, onBack }: Props) {
  const { nodes, edges } = useMemo(() => graphToFlow(model.graph), [model.id, model.updatedAt]);

  const highlightedNodes: Node<FlowNodeData>[] = nodes.map((node) => {
    if (node.id !== activeStateId) {
      return node;
    }
    return {
      ...node,
      style: {
        ...node.style,
        border: "2px solid #60a5fa",
        boxShadow: "0 0 12px rgba(96,165,250,0.8)",
      },
    };
  });

  return (
    <div style={{ display: "grid", gridTemplateRows: "auto 1fr auto", height: "100vh" }}>
      <header style={{ display: "flex", gap: "0.75rem", padding: "0.75rem 1rem", borderBottom: "1px solid #2a3558" }}>
        <button onClick={onBack}>Back</button>
        <strong>Executing {model.name}</strong>
      </header>
      <div style={{ minHeight: 0 }}>
        <ReactFlow nodes={highlightedNodes} edges={edges as Edge[]} fitView nodesDraggable={false} nodesConnectable={false}>
          <Background gap={16} color="#1f2937" />
          <MiniMap />
          <Controls showInteractive={false} />
        </ReactFlow>
      </div>
      <pre style={{ margin: 0, maxHeight: 180, overflow: "auto", padding: "0.75rem 1rem", background: "#111827", fontSize: 12 }}>
        {log.join("\n")}
      </pre>
    </div>
  );
}