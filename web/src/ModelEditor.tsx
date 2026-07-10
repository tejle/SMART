import {
  Background,
  Connection,
  Controls,
  Edge,
  MiniMap,
  Node,
  OnEdgesChange,
  OnNodesChange,
  ReactFlow,
  addEdge,
  applyEdgeChanges,
  applyNodeChanges,
} from "@xyflow/react";
import "@xyflow/react/dist/style.css";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { updateModel } from "./api";
import {
  FlowNodeData,
  autoLayout,
  flowToGraph,
  graphToFlow,
  newStateId,
  newTransitionId,
} from "./graph";
import type { Model } from "./types";

type Props = {
  model: Model;
  onBack: () => void;
  onModelUpdated: (model: Model) => void;
};

export default function ModelEditor({ model, onBack, onModelUpdated }: Props) {
  const initial = useMemo(() => graphToFlow(model.graph), [model.id]);
  const [nodes, setNodes] = useState<Node<FlowNodeData>[]>(initial.nodes);
  const [edges, setEdges] = useState<Edge[]>(initial.edges);
  const [saving, setSaving] = useState(false);
  const [savedAt, setSavedAt] = useState<string | null>(null);
  const [error, setError] = useState("");
  const saveTimer = useRef<number | null>(null);

  useEffect(() => {
    const next = graphToFlow(model.graph);
    setNodes(next.nodes);
    setEdges(next.edges);
  }, [model.id, model.updatedAt]);

  const persist = useCallback(
    async (nextNodes: Node<FlowNodeData>[], nextEdges: Edge[]) => {
      setSaving(true);
      setError("");
      try {
        const updated = await updateModel(model.projectId, model.id, {
          graph: flowToGraph(nextNodes, nextEdges),
        });
        onModelUpdated(updated);
        setSavedAt(new Date().toLocaleTimeString());
      } catch (err) {
        setError(err instanceof Error ? err.message : "Save failed");
      } finally {
        setSaving(false);
      }
    },
    [model.id, model.projectId, onModelUpdated],
  );

  const scheduleSave = useCallback(
    (nextNodes: Node<FlowNodeData>[], nextEdges: Edge[]) => {
      if (saveTimer.current) window.clearTimeout(saveTimer.current);
      saveTimer.current = window.setTimeout(() => {
        void persist(nextNodes, nextEdges);
      }, 800);
    },
    [persist],
  );

  const onNodesChange: OnNodesChange<Node<FlowNodeData>> = useCallback(
    (changes) => {
      setNodes((current) => {
        const next = applyNodeChanges(changes, current);
        scheduleSave(next, edges);
        return next;
      });
    },
    [edges, scheduleSave],
  );

  const onEdgesChange: OnEdgesChange = useCallback(
    (changes) => {
      setEdges((current) => {
        const next = applyEdgeChanges(changes, current);
        scheduleSave(nodes, next);
        return next;
      });
    },
    [nodes, scheduleSave],
  );

  const onConnect = useCallback(
    (connection: Connection) => {
      setEdges((current) => {
        const next = addEdge({ ...connection, id: newTransitionId() }, current);
        scheduleSave(nodes, next);
        return next;
      });
    },
    [nodes, scheduleSave],
  );

  const addNormalState = () => {
    const id = newStateId();
    const nextNodes: Node<FlowNodeData>[] = [
      ...nodes,
      {
        id,
        type: "state",
        position: { x: 200, y: 200 },
        data: { label: "New state", stateType: "normal" },
        style: {
          width: 120,
          height: 48,
          borderRadius: 8,
          border: "1px solid #64748b",
          background: "#111827",
          color: "#e5e7eb",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          fontSize: 12,
        },
      },
    ];
    setNodes(nextNodes);
    scheduleSave(nextNodes, edges);
  };

  const runLayout = () => {
    const laidOut = autoLayout(nodes, edges);
    setNodes(laidOut);
    scheduleSave(laidOut, edges);
  };

  return (
    <div style={{ display: "grid", gridTemplateRows: "auto 1fr", height: "100vh" }}>
      <header
        style={{
          display: "flex",
          gap: "0.75rem",
          alignItems: "center",
          padding: "0.75rem 1rem",
          borderBottom: "1px solid #2a3558",
        }}
      >
        <button onClick={onBack}>Back</button>
        <strong>{model.name}</strong>
        <button onClick={addNormalState}>Add state</button>
        <button onClick={runLayout}>Auto layout</button>
        <span style={{ marginLeft: "auto", opacity: 0.75, fontSize: 14 }}>
          {saving ? "Saving..." : savedAt ? `Saved ${savedAt}` : "Ready"}
        </span>
      </header>
      {error && (
        <p role="alert" style={{ color: "#ff8b8b", margin: "0.5rem 1rem" }}>
          {error}
        </p>
      )}
      <div style={{ minHeight: 0 }}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
          fitView
          deleteKeyCode={["Backspace", "Delete"]}
        >
          <Background gap={16} color="#1f2937" />
          <MiniMap />
          <Controls />
        </ReactFlow>
      </div>
    </div>
  );
}