export type Organization = {
  id: string;
  name: string;
  createdAt: string;
};

export type Project = {
  id: string;
  orgId: string;
  name: string;
  createdAt: string;
  updatedAt: string;
};

export type Point = { x: number; y: number };
export type Size = { width: number; height: number };

export type StateType = "normal" | "start" | "stop" | "globalRef" | "localRef";

export type State = {
  id: string;
  label: string;
  type: StateType;
  location: Point;
  size: Size;
};

export type Transition = {
  id: string;
  sourceId: string;
  targetId: string;
  guard?: string;
  action?: string;
};

export type ModelGraph = {
  states: State[];
  transitions: Transition[];
};

export type Model = {
  id: string;
  orgId: string;
  projectId: string;
  name: string;
  graph: ModelGraph;
  createdAt: string;
  updatedAt: string;
};

export type GenerationConfig = {
  stateCoverageThreshold?: number;
  maxSteps?: number;
};

export type Scenario = {
  id: string;
  orgId: string;
  projectId: string;
  name: string;
  modelIds: string[];
  algorithm: string;
  generationConfig: GenerationConfig;
  createdAt: string;
  updatedAt: string;
};

export type TestPath = {
  id: string;
  steps: { stateId: string; stateLabel: string; action?: string }[];
};

export type GenerationResult = {
  paths: TestPath[];
  visitedStateIds: string[];
  stateCoverageRatio: number;
  statistics: Record<string, number>;
};

export type Run = {
  id: string;
  orgId: string;
  projectId: string;
  scenarioId: string;
  kind: "generate" | "execute";
  status: "pending" | "running" | "completed" | "failed";
  result?: GenerationResult;
  errorMessage?: string;
  createdAt: string;
  completedAt?: string;
};