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

const orgHeaders = (): Record<string, string> => {
  const orgId = localStorage.getItem("smart.orgId");
  return orgId ? { "X-Org-ID": orgId } : {};
};

export async function createOrganization(name: string): Promise<Organization> {
  const res = await fetch("/v1/orgs", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name }),
  });
  if (!res.ok) {
    throw new Error("Failed to create organization");
  }
  return res.json();
}

export async function listProjects(): Promise<Project[]> {
  const res = await fetch("/v1/projects", { headers: orgHeaders() });
  if (!res.ok) {
    throw new Error("Failed to list projects");
  }
  return res.json();
}

export async function createProject(name: string): Promise<Project> {
  const res = await fetch("/v1/projects", {
    method: "POST",
    headers: { "Content-Type": "application/json", ...orgHeaders() },
    body: JSON.stringify({ name }),
  });
  if (!res.ok) {
    throw new Error("Failed to create project");
  }
  return res.json();
}