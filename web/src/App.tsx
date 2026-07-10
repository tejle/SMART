import { FormEvent, useEffect, useState } from "react";
import {
  createOrganization,
  createProject,
  listProjects,
  Project,
} from "./api";

export default function App() {
  const [orgId, setOrgId] = useState(localStorage.getItem("smart.orgId") ?? "");
  const [orgName, setOrgName] = useState("");
  const [projectName, setProjectName] = useState("");
  const [projects, setProjects] = useState<Project[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!orgId) return;
    listProjects()
      .then(setProjects)
      .catch((err: Error) => setError(err.message));
  }, [orgId]);

  async function onCreateOrg(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const org = await createOrganization(orgName);
      localStorage.setItem("smart.orgId", org.id);
      setOrgId(org.id);
      setOrgName("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error");
    } finally {
      setLoading(false);
    }
  }

  async function onCreateProject(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const project = await createProject(projectName);
      setProjects((prev) => [project, ...prev]);
      setProjectName("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main style={{ maxWidth: 720, margin: "0 auto", padding: "2rem 1rem" }}>
      <header style={{ marginBottom: "2rem" }}>
        <p style={{ opacity: 0.7, margin: 0 }}>Model-based testing SaaS</p>
        <h1 style={{ margin: "0.25rem 0 0" }}>SMART</h1>
      </header>

      {error && (
        <p role="alert" style={{ color: "#ff8b8b" }}>
          {error}
        </p>
      )}

      {!orgId ? (
        <section>
          <h2>Start your organization</h2>
          <form onSubmit={onCreateOrg} style={{ display: "grid", gap: "0.75rem" }}>
            <input
              value={orgName}
              onChange={(e) => setOrgName(e.target.value)}
              placeholder="Organization name"
              required
            />
            <button type="submit" disabled={loading}>
              Create organization
            </button>
          </form>
        </section>
      ) : (
        <>
          <p style={{ opacity: 0.8 }}>Org ID: {orgId}</p>
          <section style={{ marginTop: "1.5rem" }}>
            <h2>Projects</h2>
            <form
              onSubmit={onCreateProject}
              style={{ display: "grid", gap: "0.75rem", marginBottom: "1rem" }}
            >
              <input
                value={projectName}
                onChange={(e) => setProjectName(e.target.value)}
                placeholder="New project name"
                required
              />
              <button type="submit" disabled={loading}>
                Create project
              </button>
            </form>
            <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
              {projects.map((p) => (
                <li
                  key={p.id}
                  style={{
                    padding: "0.75rem 1rem",
                    border: "1px solid #2a3558",
                    borderRadius: 8,
                    marginBottom: "0.5rem",
                  }}
                >
                  {p.name}
                </li>
              ))}
            </ul>
          </section>
        </>
      )}
    </main>
  );
}