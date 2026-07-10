import type { Model, ModelGraph, Organization, Project } from "./types";

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