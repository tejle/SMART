import dagre from "dagre";
import { MarkerType, type Edge, type Node } from "@xyflow/react";
import { stateNodeStyle } from "./graphStyles";
import type { ModelGraph, State, StateType, Transition } from "./types";

export type FlowNodeData = {
  label: string;
  stateType: StateType;
};

export type FlowEdgeData = {
  action: string;
  guard: string;
};

export function edgeDisplayLabel(action?: string, guard?: string): string {
  const parts = [action, guard].filter(Boolean);
  return parts.join(" / ");
}

export function graphToFlow(graph: ModelGraph): {
  nodes: Node<FlowNodeData>[];
  edges: Edge<FlowEdgeData>[];
} {
  const nodes: Node<FlowNodeData>[] = graph.states.map((state) => ({
    id: state.id,
    type: "state",
    position: { x: state.location.x, y: state.location.y },
    data: { label: state.label, stateType: state.type },
    style: {
      ...stateNodeStyle(state.type),
      width: state.size.width,
      height: state.size.height,
    },
  }));

  const edges: Edge<FlowEdgeData>[] = graph.transitions.map((t) => ({
    id: t.id,
    source: t.sourceId,
    target: t.targetId,
    data: { action: t.action ?? "", guard: t.guard ?? "" },
    label: edgeDisplayLabel(t.action, t.guard),
    animated: false,
    style: { stroke: "#94a3b8" },
    labelStyle: { fill: "#e8edf7", fontSize: 11, fontWeight: 500 },
    labelBgStyle: { fill: "#1e293b", fillOpacity: 0.95 },
    labelBgPadding: [6, 8] as [number, number],
    labelBgBorderRadius: 4,
    markerEnd: { type: MarkerType.ArrowClosed, color: "#94a3b8", width: 18, height: 18 },
  }));

  return { nodes, edges };
}

export function flowToGraph(nodes: Node<FlowNodeData>[], edges: Edge<FlowEdgeData>[]): ModelGraph {
  const states: State[] = nodes.map((node) => ({
    id: node.id,
    label: node.data.label,
    type: node.data.stateType,
    location: { x: node.position.x, y: node.position.y },
    size: {
      width: Number(node.style?.width ?? 120),
      height: Number(node.style?.height ?? 48),
    },
  }));

  const transitions: Transition[] = edges.map((edge) => ({
    id: edge.id,
    sourceId: edge.source,
    targetId: edge.target,
    action: edge.data?.action ?? "",
    guard: edge.data?.guard ?? "",
  }));

  return { states, transitions };
}

export function autoLayout(nodes: Node<FlowNodeData>[], edges: Edge<FlowEdgeData>[]) {
  const g = new dagre.graphlib.Graph();
  g.setDefaultEdgeLabel(() => ({}));
  g.setGraph({ rankdir: "LR", nodesep: 60, ranksep: 100 });

  nodes.forEach((node) => {
    g.setNode(node.id, {
      width: Number(node.style?.width ?? 120),
      height: Number(node.style?.height ?? 48),
    });
  });
  edges.forEach((edge) => g.setEdge(edge.source, edge.target));

  dagre.layout(g);

  return nodes.map((node) => {
    const positioned = g.node(node.id);
    return {
      ...node,
      position: {
        x: positioned.x - Number(node.style?.width ?? 120) / 2,
        y: positioned.y - Number(node.style?.height ?? 48) / 2,
      },
    };
  });
}

export function newStateId() {
  return `state-${crypto.randomUUID().slice(0, 8)}`;
}

export function newTransitionId() {
  return `transition-${crypto.randomUUID().slice(0, 8)}`;
}