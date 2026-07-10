import type { Edge, Node } from "@xyflow/react";
import type { FlowEdgeData, FlowNodeData } from "./graph";
import type { StateType } from "./types";

export function stateNodeStyle(stateType: StateType) {
  const isRef = stateType === "globalRef" || stateType === "localRef";
  return {
    width: 120,
    height: 48,
    borderRadius: stateType === "start" || stateType === "stop" ? 999 : 8,
    border:
      stateType === "start"
        ? "2px solid #4ade80"
        : stateType === "stop"
          ? "2px solid #f87171"
          : isRef
            ? "2px dashed #a78bfa"
            : "1px solid #64748b",
    background: isRef ? "#1e1b4b" : "#111827",
    color: "#e5e7eb",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 12,
    padding: 4,
    cursor: isRef ? "pointer" : "default",
  };
}

export function highlightNodes(
  nodes: Node<FlowNodeData>[],
  options: {
    visitedIds?: Set<string>;
    activeId?: string;
    failedIds?: Set<string>;
    selectedPathIds?: Set<string>;
  },
): Node<FlowNodeData>[] {
  return nodes.map((node) => {
    let border = node.style?.border as string | undefined;
    let boxShadow: string | undefined;
    let background = node.style?.background as string | undefined;

    if (options.failedIds?.has(node.id)) {
      border = "2px solid #f87171";
      boxShadow = "0 0 10px rgba(248,113,113,0.7)";
      background = "#450a0a";
    } else if (options.activeId === node.id) {
      border = "2px solid #60a5fa";
      boxShadow = "0 0 12px rgba(96,165,250,0.8)";
    } else if (options.selectedPathIds?.has(node.id)) {
      border = "2px solid #fbbf24";
      boxShadow = "0 0 8px rgba(251,191,36,0.5)";
    } else if (options.visitedIds?.has(node.id)) {
      border = "2px solid #4ade80";
      background = "#14532d";
    }

    return {
      ...node,
      style: {
        ...node.style,
        border,
        boxShadow,
        background,
      },
    };
  });
}

export function highlightEdges(
  edges: Edge<FlowEdgeData>[],
  options: {
    activeEdgeId?: string;
    activeTraverse?: { fromStateId: string; toStateId: string };
    pathEdgeIds?: Set<string>;
  },
): Edge<FlowEdgeData>[] {
  const resolvedActiveId =
    options.activeEdgeId ??
    (options.activeTraverse
      ? edges.find(
          (e) =>
            e.source === options.activeTraverse!.fromStateId &&
            e.target === options.activeTraverse!.toStateId,
        )?.id
      : undefined);

  return edges.map((edge) => {
    const onPath = options.pathEdgeIds?.has(edge.id);
    const active = resolvedActiveId === edge.id;
    return {
      ...edge,
      animated: active || onPath,
      style: {
        ...edge.style,
        stroke: active ? "#60a5fa" : onPath ? "#fbbf24" : "#94a3b8",
        strokeWidth: active || onPath ? 2.5 : 1.5,
      },
    };
  });
}

export function pathEdgeIdsForSteps(
  edges: Edge<FlowEdgeData>[],
  stepStateIds: string[],
): Set<string> {
  const ids = new Set<string>();
  for (let i = 0; i < stepStateIds.length - 1; i++) {
    const source = stepStateIds[i];
    const target = stepStateIds[i + 1];
    const edge = edges.find((e) => e.source === source && e.target === target);
    if (edge) ids.add(edge.id);
  }
  return ids;
}