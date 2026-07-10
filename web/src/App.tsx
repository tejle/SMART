import { FormEvent, useEffect, useState } from "react";
import {
  createModel,
  createOrganization,
  createProject,
  listModels,
  listProjects,
} from "./api";
import ModelEditor from "./ModelEditor";
import type { Model, Project } from "./types";

export default function App() {
  const [orgId, setOrgId] = useState(localStorage.getItem("smart.orgId") ?? "");
  const [orgName, setOrgName] = useState("");
  const [projectName, setProjectName] = useState("");
  const [modelName, setModelName] = useState("");
  const [projects, setProjects] = useState<Project[]>([]);
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [models, setModels] = useState<Model[]>([]);
  const [activeModel, setActiveModel] = useState<Model | null>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!orgId) return;
    listProjects()
      .then(setProjects)
      .catch((err: Error) => setError(err.message));
  }, [orgId]);

  useEffect(() => {
    if (!selectedProject) return;
    listModels(selectedProject.id)
      .then(setModels)
      .catch((err: Error) => setError(err.message));
  }, [selectedProject?.id]);

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
      setSelectedProject(project);
      setProjectName("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error");
    } finally {
      setLoading(false);
    }
  }

  async function onCreateModel(e: FormEvent) {
    e.preventDefault();
    if (!selectedProject) return;
    setLoading(true);
    setError("");
    try {
      const model = await createModel(selectedProject.id, modelName);
      setModels((prev) => [model, ...prev]);
      setModelName("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error");
    } finally {
      setLoading(false);
    }
  }

  if (activeModel) {
    return (
      <ModelEditor
        model={activeModel}
        onBack={() => setActiveModel(null)}
        onModelUpdated={(model) => {
          setActiveModel(model);
          setModels((prev) => prev.map((m) => (m.id === model.id ? model : m)));
        }}
      />
    );
  }

  return (
    <main style={{ maxWidth: 960, margin: "0 auto", padding: "2rem 1rem" }}>
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
        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1.5rem" }}>
          <section>
            <h2>Projects</h2>
            <form onSubmit={onCreateProject} style={{ display: "grid", gap: "0.75rem", marginBottom: "1rem" }}>
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
                <li key={p.id} style={{ marginBottom: "0.5rem" }}>
                  <button
                    onClick={() => setSelectedProject(p)}
                    style={{
                      width: "100%",
                      textAlign: "left",
                      padding: "0.75rem 1rem",
                      border: selectedProject?.id === p.id ? "1px solid #60a5fa" : "1px solid #2a3558",
                      borderRadius: 8,
                      background: selectedProject?.id === p.id ? "#172554" : "transparent",
                      color: "inherit",
                      cursor: "pointer",
                    }}
                  >
                    {p.name}
                  </button>
                </li>
              ))}
            </ul>
          </section>

          <section>
            <h2>Models</h2>
            {!selectedProject ? (
              <p style={{ opacity: 0.7 }}>Select a project to manage models.</p>
            ) : (
              <>
                <form onSubmit={onCreateModel} style={{ display: "grid", gap: "0.75rem", marginBottom: "1rem" }}>
                  <input
                    value={modelName}
                    onChange={(e) => setModelName(e.target.value)}
                    placeholder="New model name"
                    required
                  />
                  <button type="submit" disabled={loading}>
                    Create model
                  </button>
                </form>
                <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
                  {models.map((m) => (
                    <li key={m.id} style={{ marginBottom: "0.5rem" }}>
                      <button
                        onClick={() => setActiveModel(m)}
                        style={{
                          width: "100%",
                          textAlign: "left",
                          padding: "0.75rem 1rem",
                          border: "1px solid #2a3558",
                          borderRadius: 8,
                          background: "transparent",
                          color: "inherit",
                          cursor: "pointer",
                        }}
                      >
                        {m.name}
                        <span style={{ opacity: 0.6, marginLeft: 8, fontSize: 12 }}>
                          {m.graph.states.length} states
                        </span>
                      </button>
                    </li>
                  ))}
                </ul>
              </>
            )}
          </section>
        </div>
      )}
    </main>
  );
}