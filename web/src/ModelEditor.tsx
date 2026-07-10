import {
  Background,
  Connection,
  Controls,
  Edge,
  MarkerType,
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
  FlowEdgeData,
  FlowNodeData,
  autoLayout,
  edgeDisplayLabel,
  flowToGraph,
  graphToFlow,
  newStateId,
  newTransitionId,
} from "./graph";
import type { Model, StateType } from "./types";

type Props = {
  model: Model;
  onBack: () => void;
  onModelUpdated: (model: Model) => void;
};

const stateTypeOptions: { value: StateType; label: string }[] = [
  { value: "normal", label: "Normal" },
  { value: "start", label: "Start" },
  { value: "stop", label: "Stop" },
  { value: "globalRef", label: "Global reference" },
  { value: "localRef", label: "Local reference" },
];

function stateNodeStyle(stateType: StateType) {
  return {
    width: 120,
    height: 48,
    borderRadius: stateType === "start" || stateType === "stop" ? 999 : 8,
    border:
      stateType === "start"
        ? "2px solid #4ade80"
        : stateType === "stop"
          ? "2px solid #f87171"
          : "1px solid #64748b",
    background: "#111827",
    color: "#e5e7eb",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 12,
    padding: 4,
  };
}

function PropertiesPanel({
  modelName,
  onModelNameChange,
  selectedNode,
  selectedEdge,
  onUpdateNode,
  onUpdateEdge,
  onDeleteNode,
  onDeleteEdge,
}: {
  modelName: string;
  onModelNameChange: (name: string) => void;
  selectedNode: Node<FlowNodeData> | null;
  selectedEdge: Edge<FlowEdgeData> | null;
  onUpdateNode: (id: string, patch: Partial<FlowNodeData>) => void;
  onUpdateEdge: (id: string, patch: Partial<FlowEdgeData>) => void;
  onDeleteNode: (id: string) => void;
  onDeleteEdge: (id: string) => void;
}) {
  const fieldStyle = { display: "grid", gap: "0.35rem", marginBottom: "1rem" };
  const labelStyle = { fontSize: 12, opacity: 0.75 };

  if (selectedNode) {
    return (
      <aside style={{ padding: "1rem", borderLeft: "1px solid #2a3558", overflow: "auto" }}>
        <h3 style={{ margin: "0 0 1rem", fontSize: 14 }}>State</h3>
        <label style={fieldStyle}>
          <span style={labelStyle}>Name</span>
          <input
            value={selectedNode.data.label}
            onChange={(e) => onUpdateNode(selectedNode.id, { label: e.target.value })}
            autoFocus
          />
        </label>
        <label style={fieldStyle}>
          <span style={labelStyle}>Type</span>
          <select
            value={selectedNode.data.stateType}
            onChange={(e) => onUpdateNode(selectedNode.id, { stateType: e.target.value as StateType })}
          >
            {stateTypeOptions.map((opt) => (
              <option key={opt.value} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
        </label>
        <button type="button" onClick={() => onDeleteNode(selectedNode.id)} style={{ color: "#f87171" }}>
          Delete state
        </button>
      </aside>
    );
  }

  if (selectedEdge) {
    return (
      <aside style={{ padding: "1rem", borderLeft: "1px solid #2a3558", overflow: "auto" }}>
        <h3 style={{ margin: "0 0 1rem", fontSize: 14 }}>Transition</h3>
        <label style={fieldStyle}>
          <span style={labelStyle}>Action</span>
          <input
            value={selectedEdge.data?.action ?? ""}
            onChange={(e) => onUpdateEdge(selectedEdge.id, { action: e.target.value })}
            placeholder="Action name"
            autoFocus
          />
        </label>
        <label style={fieldStyle}>
          <span style={labelStyle}>Guard</span>
          <input
            value={selectedEdge.data?.guard ?? ""}
            onChange={(e) => onUpdateEdge(selectedEdge.id, { guard: e.target.value })}
            placeholder="Guard condition"
          />
        </label>
        <button type="button" onClick={() => onDeleteEdge(selectedEdge.id)} style={{ color: "#f87171" }}>
          Delete transition
        </button>
      </aside>
    );
  }

  return (
    <aside style={{ padding: "1rem", borderLeft: "1px solid #2a3558", overflow: "auto" }}>
      <h3 style={{ margin: "0 0 1rem", fontSize: 14 }}>Model</h3>
      <label style={fieldStyle}>
        <span style={labelStyle}>Name</span>
        <input value={modelName} onChange={(e) => onModelNameChange(e.target.value)} />
      </label>
      <p style={{ fontSize: 12, opacity: 0.6, margin: 0 }}>
        Select a state or transition on the canvas to edit its properties.
      </p>
    </aside>
  );
}

export default function ModelEditor({ model, onBack, onModelUpdated }: Props) {
  const initial = useMemo(() => graphToFlow(model.graph), [model.id]);
  const [nodes, setNodes] = useState<Node<FlowNodeData>[]>(initial.nodes);
  const [edges, setEdges] = useState<Edge<FlowEdgeData>[]>(initial.edges);
  const [modelName, setModelName] = useState(model.name);
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null);
  const [selectedEdgeId, setSelectedEdgeId] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [savedAt, setSavedAt] = useState<string | null>(null);
  const [error, setError] = useState("");
  const saveTimer = useRef<number | null>(null);
  const closingRef = useRef(false);

  useEffect(() => {
    const next = graphToFlow(model.graph);
    setNodes(next.nodes);
    setEdges(next.edges);
    setModelName(model.name);
  }, [model.id, model.updatedAt]);

  useEffect(() => {
    return () => {
      closingRef.current = true;
      if (saveTimer.current) window.clearTimeout(saveTimer.current);
    };
  }, []);

  const persist = useCallback(
    async (
      nextNodes: Node<FlowNodeData>[],
      nextEdges: Edge<FlowEdgeData>[],
      nextName?: string,
    ) => {
      if (closingRef.current) return;
      setSaving(true);
      setError("");
      try {
        const payload: { name?: string; graph?: ReturnType<typeof flowToGraph> } = {
          graph: flowToGraph(nextNodes, nextEdges),
        };
        if (nextName !== undefined && nextName !== model.name) {
          payload.name = nextName;
        }
        const updated = await updateModel(model.projectId, model.id, payload);
        if (closingRef.current) return;
        onModelUpdated(updated);
        setSavedAt(new Date().toLocaleTimeString());
      } catch (err) {
        if (!closingRef.current) {
          setError(err instanceof Error ? err.message : "Save failed");
        }
      } finally {
        if (!closingRef.current) setSaving(false);
      }
    },
    [model.id, model.name, model.projectId, onModelUpdated],
  );

  const scheduleSave = useCallback(
    (nextNodes: Node<FlowNodeData>[], nextEdges: Edge<FlowEdgeData>[], nextName?: string) => {
      if (saveTimer.current) window.clearTimeout(saveTimer.current);
      saveTimer.current = window.setTimeout(() => {
        void persist(nextNodes, nextEdges, nextName);
      }, 800);
    },
    [persist],
  );

  const handleBack = () => {
    closingRef.current = true;
    if (saveTimer.current) window.clearTimeout(saveTimer.current);
    onBack();
  };

  const onNodesChange: OnNodesChange<Node<FlowNodeData>> = useCallback(
    (changes) => {
      setNodes((current) => {
        const next = applyNodeChanges(changes, current);
        scheduleSave(next, edges, modelName);
        return next;
      });
    },
    [edges, modelName, scheduleSave],
  );

  const onEdgesChange: OnEdgesChange<Edge<FlowEdgeData>> = useCallback(
    (changes) => {
      setEdges((current) => {
        const next = applyEdgeChanges(changes, current);
        scheduleSave(nodes, next, modelName);
        return next;
      });
    },
    [nodes, modelName, scheduleSave],
  );

  const onConnect = useCallback(
    (connection: Connection) => {
      setEdges((current) => {
        const next = addEdge(
          {
            ...connection,
            id: newTransitionId(),
            data: { action: "", guard: "" },
            markerEnd: { type: MarkerType.ArrowClosed, color: "#94a3b8", width: 18, height: 18 },
          },
          current,
        );
        scheduleSave(nodes, next, modelName);
        return next;
      });
    },
    [nodes, modelName, scheduleSave],
  );

  const updateNode = (id: string, patch: Partial<FlowNodeData>) => {
    const nextNodes = nodes.map((node) => {
      if (node.id !== id) return node;
      const data = { ...node.data, ...patch };
      return {
        ...node,
        data,
        style: stateNodeStyle(data.stateType),
      };
    });
    setNodes(nextNodes);
    scheduleSave(nextNodes, edges, modelName);
  };

  const updateEdge = (id: string, patch: Partial<FlowEdgeData>) => {
    const nextEdges = edges.map((edge) => {
      if (edge.id !== id) return edge;
      const data = { ...(edge.data ?? { action: "", guard: "" }), ...patch };
      return {
        ...edge,
        data,
        label: edgeDisplayLabel(data.action, data.guard),
      };
    });
    setEdges(nextEdges);
    scheduleSave(nodes, nextEdges, modelName);
  };

  const deleteNode = (id: string) => {
    const nextNodes = nodes.filter((n) => n.id !== id);
    const nextEdges = edges.filter((e) => e.source !== id && e.target !== id);
    setNodes(nextNodes);
    setEdges(nextEdges);
    setSelectedNodeId(null);
    scheduleSave(nextNodes, nextEdges, modelName);
  };

  const deleteEdge = (id: string) => {
    const nextEdges = edges.filter((e) => e.id !== id);
    setEdges(nextEdges);
    setSelectedEdgeId(null);
    scheduleSave(nodes, nextEdges, modelName);
  };

  const addState = (stateType: StateType, label: string) => {
    const id = newStateId();
    const nextNodes: Node<FlowNodeData>[] = [
      ...nodes,
      {
        id,
        type: "state",
        position: { x: 200, y: 200 },
        data: { label, stateType },
        style: stateNodeStyle(stateType),
      },
    ];
    setNodes(nextNodes);
    scheduleSave(nextNodes, edges, modelName);
  };

  const runLayout = () => {
    const laidOut = autoLayout(nodes, edges);
    setNodes(laidOut);
    scheduleSave(laidOut, edges, modelName);
  };

  const handleModelNameChange = (name: string) => {
    setModelName(name);
    scheduleSave(nodes, edges, name);
  };

  const selectedNode = nodes.find((n) => n.id === selectedNodeId) ?? null;
  const selectedEdge = edges.find((e) => e.id === selectedEdgeId) ?? null;

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
        <button onClick={handleBack}>Back</button>
        <strong>{modelName}</strong>
        <button onClick={() => addState("normal", "New state")}>Add state</button>
        <button onClick={() => addState("start", "Start")}>Add start</button>
        <button onClick={() => addState("stop", "Stop")}>Add stop</button>
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
      <div style={{ display: "grid", gridTemplateColumns: "1fr 280px", minHeight: 0 }}>
        <div style={{ minHeight: 0 }}>
          <ReactFlow
            nodes={nodes}
            edges={edges}
            onNodesChange={onNodesChange}
            onEdgesChange={onEdgesChange}
            onConnect={onConnect}
            onSelectionChange={({ nodes: selNodes, edges: selEdges }) => {
              setSelectedNodeId(selNodes[0]?.id ?? null);
              setSelectedEdgeId(selEdges[0]?.id ?? null);
            }}
            fitView
            deleteKeyCode={["Backspace", "Delete"]}
          >
            <Background gap={16} color="#1f2937" />
            <MiniMap />
            <Controls />
          </ReactFlow>
        </div>
        <PropertiesPanel
          modelName={modelName}
          onModelNameChange={handleModelNameChange}
          selectedNode={selectedNode}
          selectedEdge={selectedEdge}
          onUpdateNode={updateNode}
          onUpdateEdge={updateEdge}
          onDeleteNode={deleteNode}
          onDeleteEdge={deleteEdge}
        />
      </div>
    </div>
  );
}