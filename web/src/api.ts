import type {
  Model,
  ModelGraph,
  Organization,
  Project,
  Run,
  Scenario,
} from "./types";

const orgHeaders = (): Record<string, string> => {
  const orgId = localStorage.getItem("smart.orgId");
  return orgId ? { "X-Org-ID": orgId } : {};
};

async function parseError(res: Response, fallback: string) {
  try {
    const body = await res.json();
    if (body?.error) return body.error as string;
  } catch {
    // ignore
  }
  return fallback;
}

export async function createOrganization(name: string): Promise<Organization> {
  const res = await fetch("/v1/orgs", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name }),
  });
  if (!res.ok) throw new Error(await parseError(res, "Failed to create organization"));
  return res.json();
}

export async function listProjects(): Promise<Project[]> {
  const res = await fetch("/v1/projects", { headers: orgHeaders() });
  if (!res.ok) throw new Error(await parseError(res, "Failed to list projects"));
  return res.json();
}

export async function createProject(name: string): Promise<Project> {
  const res = await fetch("/v1/projects", {
    method: "POST",
    headers: { "Content-Type": "application/json", ...orgHeaders() },
    body: JSON.stringify({ name }),
  });
  if (!res.ok) throw new Error(await parseError(res, "Failed to create project"));
  return res.json();
}

export async function listModels(projectId: string): Promise<Model[]> {
  const res = await fetch(`/v1/projects/${projectId}/models`, { headers: orgHeaders() });
  if (!res.ok) throw new Error(await parseError(res, "Failed to list models"));
  return res.json();
}

export async function createModel(projectId: string, name: string): Promise<Model> {
  const res = await fetch(`/v1/projects/${projectId}/models`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...orgHeaders() },
    body: JSON.stringify({ name }),
  });
  if (!res.ok) throw new Error(await parseError(res, "Failed to create model"));
  return res.json();
}

export async function getModel(projectId: string, modelId: string): Promise<Model> {
  const res = await fetch(`/v1/projects/${projectId}/models/${modelId}`, {
    headers: orgHeaders(),
  });
  if (!res.ok) throw new Error(await parseError(res, "Failed to get model"));
  return res.json();
}

export async function updateModel(
  projectId: string,
  modelId: string,
  payload: { name?: string; graph?: ModelGraph },
): Promise<Model> {
  const res = await fetch(`/v1/projects/${projectId}/models/${modelId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json", ...orgHeaders() },
    body: JSON.stringify(payload),
  });
  if (!res.ok) throw new Error(await parseError(res, "Failed to update model"));
  return res.json();
}

export async function listScenarios(projectId: string): Promise<Scenario[]> {
  const res = await fetch(`/v1/projects/${projectId}/scenarios`, { headers: orgHeaders() });
  if (!res.ok) throw new Error(await parseError(res, "Failed to list scenarios"));
  return res.json();
}

export async function createScenario(
  projectId: string,
  payload: {
    name: string;
    modelIds: string[];
    algorithm?: string;
    generationConfig?: { stateCoverageThreshold?: number; maxSteps?: number };
    adapterConfig?: { baseUrl: string; timeoutSeconds?: number };
  },
): Promise<Scenario> {
  const res = await fetch(`/v1/projects/${projectId}/scenarios`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...orgHeaders() },
    body: JSON.stringify(payload),
  });
  if (!res.ok) throw new Error(await parseError(res, "Failed to create scenario"));
  return res.json();
}

export async function startRun(scenarioId: string, kind: "generate" | "execute"): Promise<Run> {
  const res = await fetch(`/v1/scenarios/${scenarioId}/runs`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...orgHeaders() },
    body: JSON.stringify({ kind }),
  });
  if (!res.ok) throw new Error(await parseError(res, `Failed to start ${kind} run`));
  return res.json();
}

export async function getRun(runId: string): Promise<Run> {
  const res = await fetch(`/v1/runs/${runId}`, { headers: orgHeaders() });
  if (!res.ok) throw new Error(await parseError(res, "Failed to get run"));
  return res.json();
}

export async function listProjectRuns(projectId: string): Promise<Run[]> {
  const res = await fetch(`/v1/projects/${projectId}/runs`, { headers: orgHeaders() });
  if (!res.ok) throw new Error(await parseError(res, "Failed to list runs"));
  return res.json();
}

export function runReportUrl(runId: string) {
  return `/v1/runs/${runId}/report`;
}

export async function openRunReport(runId: string): Promise<void> {
  const res = await fetch(runReportUrl(runId), { headers: orgHeaders() });
  if (!res.ok) throw new Error(await parseError(res, "Failed to load report"));
  const html = await res.text();
  const blob = new Blob([html], { type: "text/html" });
  const url = URL.createObjectURL(blob);
  window.open(url, "_blank", "noopener,noreferrer");
  window.setTimeout(() => URL.revokeObjectURL(url), 60_000);
}

export function subscribeRunEvents(runId: string, onEvent: (payload: unknown) => void) {
  const source = new EventSource(`/v1/runs/${runId}/events`);
  source.addEventListener("run", (event) => {
    onEvent(JSON.parse((event as MessageEvent).data));
  });
  source.addEventListener("close", () => source.close());
  return () => source.close();
}