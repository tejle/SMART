import type { ModelGraph, TestPath, Transition } from "./types";

export type SequenceItem =
  | { kind: "validate"; stateId: string; stateLabel: string }
  | {
      kind: "traverse";
      fromStateId: string;
      toStateId: string;
      edgeId?: string;
      action: string;
      guard?: string;
    };

export function normalizeStateId(stateId: string): string {
  return stateId.split(":").pop() ?? stateId;
}

export function findTransition(
  graph: ModelGraph,
  fromId: string,
  toId: string,
  action?: string,
): Transition | undefined {
  const from = normalizeStateId(fromId);
  const to = normalizeStateId(toId);
  const matches = graph.transitions.filter((t) => t.sourceId === from && t.targetId === to);
  if (action) {
    const byAction = matches.find((t) => t.action === action);
    if (byAction) return byAction;
  }
  return matches[0];
}

/** Parse engine path steps into validate → traverse → validate semantics. */
export function parseTestPath(path: TestPath, graph: ModelGraph): SequenceItem[] {
  if (path.steps.length === 0) return [];

  const items: SequenceItem[] = [];
  const first = path.steps[0];
  items.push({
    kind: "validate",
    stateId: normalizeStateId(first.stateId),
    stateLabel: first.stateLabel,
  });

  for (let i = 1; i < path.steps.length; i++) {
    const prev = path.steps[i - 1];
    const curr = path.steps[i];
    const fromId = normalizeStateId(prev.stateId);
    const toId = normalizeStateId(curr.stateId);
    const transition = findTransition(graph, fromId, toId, curr.action);

    items.push({
      kind: "traverse",
      fromStateId: fromId,
      toStateId: toId,
      edgeId: transition?.id,
      action: curr.action ?? transition?.action ?? "(unnamed action)",
      guard: transition?.guard,
    });
    items.push({
      kind: "validate",
      stateId: toId,
      stateLabel: curr.stateLabel,
    });
  }

  return items;
}

export function sequenceStateIds(items: SequenceItem[]): string[] {
  const ids: string[] = [];
  for (const item of items) {
    if (item.kind === "validate") {
      ids.push(item.stateId);
    }
  }
  return ids;
}

export function sequenceEdgeIds(items: SequenceItem[]): string[] {
  return items
    .filter((item): item is Extract<SequenceItem, { kind: "traverse" }> => item.kind === "traverse")
    .map((item) => item.edgeId)
    .filter((id): id is string => Boolean(id));
}

export function highlightsUpToIndex(
  items: SequenceItem[],
  index: number,
): { stateIds: Set<string>; edgeIds: Set<string>; activeStateId?: string; activeEdgeId?: string } {
  const stateIds = new Set<string>();
  const edgeIds = new Set<string>();
  let activeStateId: string | undefined;
  let activeEdgeId: string | undefined;

  for (let i = 0; i <= index && i < items.length; i++) {
    const item = items[i];
    if (item.kind === "validate") {
      stateIds.add(item.stateId);
      if (i === index) activeStateId = item.stateId;
    } else {
      if (item.edgeId) edgeIds.add(item.edgeId);
      if (i === index) activeEdgeId = item.edgeId;
    }
  }

  return { stateIds, edgeIds, activeStateId, activeEdgeId };
}