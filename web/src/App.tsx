import { FormEvent, useEffect, useState } from "react";
import {
  createModel,
  createOrganization,
  createProject,
  createScenario,
  getRun,
  listModels,
  listProjects,
  listScenarios,
  startRun,
  runReportUrl,
  subscribeRunEvents,
} from "./api";
import ExecutionView from "./ExecutionView";
import ModelEditor from "./ModelEditor";
import type { Model, Project, Run, Scenario } from "./types";

export default function App() {
  const [orgId, setOrgId] = useState(localStorage.getItem("smart.orgId") ?? "");
  const [orgName, setOrgName] = useState("");
  const [projectName, setProjectName] = useState("");
  const [modelName, setModelName] = useState("");
  const [projects, setProjects] = useState<Project[]>([]);
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [models, setModels] = useState<Model[]>([]);
  const [scenarios, setScenarios] = useState<Scenario[]>([]);
  const [scenarioName, setScenarioName] = useState("");
  const [selectedModelIds, setSelectedModelIds] = useState<string[]>([]);
  const [adapterBaseUrl, setAdapterBaseUrl] = useState("http://localhost:9090");
  const [lastRun, setLastRun] = useState<Run | null>(null);
  const [runLog, setRunLog] = useState<string[]>([]);
  const [executionModel, setExecutionModel] = useState<Model | null>(null);
  const [activeStateId, setActiveStateId] = useState<string | undefined>();
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
    listScenarios(selectedProject.id)
      .then(setScenarios)
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

  async function onCreateScenario(e: FormEvent) {
    e.preventDefault();
    if (!selectedProject || selectedModelIds.length === 0) return;
    setLoading(true);
    setError("");
    try {
      const scenario = await createScenario(selectedProject.id, {
        name: scenarioName,
        modelIds: selectedModelIds,
        algorithm: "breadth-first",
        generationConfig: { stateCoverageThreshold: 1, maxSteps: 100 },
        adapterConfig: { baseUrl: adapterBaseUrl },
      });
      setScenarios((prev) => [scenario, ...prev]);
      setScenarioName("");
      setSelectedModelIds([]);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error");
    } finally {
      setLoading(false);
    }
  }

  async function waitForRun(runId: string): Promise<Run> {
    for (let attempt = 0; attempt < 90; attempt++) {
      const run = await getRun(runId);
      if (run.status === "completed" || run.status === "failed") {
        return run;
      }
      await new Promise((resolve) => setTimeout(resolve, 1000));
    }
    throw new Error("Run timed out");
  }

  async function onRun(scenarioId: string, kind: "generate" | "execute") {
    setLoading(true);
    setError("");
    setRunLog([]);
    setActiveStateId(undefined);
    try {
      const pending = await startRun(scenarioId, kind);
      setLastRun(pending);

      if (kind === "execute") {
        const scenario = scenarios.find((s) => s.id === scenarioId);
        const model = models.find((m) => scenario?.modelIds.includes(m.id));
        if (model) {
          setExecutionModel(model);
        }
        const unsubscribe = subscribeRunEvents(pending.id, (event) => {
          const e = event as {
            type?: string;
            message?: string;
            step?: { stateLabel?: string; stateId?: string };
          };
          if (e.step?.stateId) {
            setActiveStateId(e.step.stateId.split(":").pop());
          }
          const line = e.step?.stateLabel
            ? `${e.type}: ${e.step.stateLabel}`
            : `${e.type}${e.message ? ` — ${e.message}` : ""}`;
          setRunLog((prev) => [...prev, line]);
        });
        const completed = await waitForRun(pending.id);
        setLastRun(completed);
        unsubscribe();
        setExecutionModel(null);
      } else {
        const completed = await waitForRun(pending.id);
        setLastRun(completed);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error");
      setExecutionModel(null);
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

  if (executionModel) {
    return (
      <ExecutionView
        model={executionModel}
        activeStateId={activeStateId}
        log={runLog}
        onBack={() => setExecutionModel(null)}
      />
    );
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
        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: "1.5rem" }}>
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

          <section>
            <h2>Scenarios</h2>
            {!selectedProject ? (
              <p style={{ opacity: 0.7 }}>Select a project to configure scenarios.</p>
            ) : (
              <>
                <form onSubmit={onCreateScenario} style={{ display: "grid", gap: "0.75rem", marginBottom: "1rem" }}>
                  <input
                    value={scenarioName}
                    onChange={(e) => setScenarioName(e.target.value)}
                    placeholder="Scenario name"
                    required
                  />
                  <div style={{ display: "grid", gap: "0.35rem" }}>
                    {models.map((m) => (
                      <label key={m.id} style={{ fontSize: 14 }}>
                        <input
                          type="checkbox"
                          checked={selectedModelIds.includes(m.id)}
                          onChange={(e) => {
                            setSelectedModelIds((prev) =>
                              e.target.checked ? [...prev, m.id] : prev.filter((id) => id !== m.id),
                            );
                          }}
                        />{" "}
                        {m.name}
                      </label>
                    ))}
                  </div>
                  <input
                    value={adapterBaseUrl}
                    onChange={(e) => setAdapterBaseUrl(e.target.value)}
                    placeholder="HTTP adapter base URL"
                  />
                  <button type="submit" disabled={loading || selectedModelIds.length === 0}>
                    Create scenario
                  </button>
                </form>
                <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
                  {scenarios.map((s) => (
                    <li
                      key={s.id}
                      style={{
                        padding: "0.75rem 1rem",
                        border: "1px solid #2a3558",
                        borderRadius: 8,
                        marginBottom: "0.5rem",
                      }}
                    >
                      <div style={{ display: "flex", justifyContent: "space-between", gap: "0.5rem" }}>
                        <span>{s.name}</span>
                        <button onClick={() => onRun(s.id, "generate")} disabled={loading}>
                          Generate
                        </button>
                        <button onClick={() => onRun(s.id, "execute")} disabled={loading}>
                          Execute
                        </button>
                      </div>
                    </li>
                  ))}
                </ul>
                {lastRun?.result?.generation && (
                  <div style={{ marginTop: "1rem", fontSize: 14, opacity: 0.9 }}>
                    <strong>Last run:</strong> {lastRun.result.generation.paths.length} paths, coverage{" "}
                    {(lastRun.result.generation.stateCoverageRatio * 100).toFixed(0)}%
                    {" · "}
                    <a href={runReportUrl(lastRun.id)} target="_blank" rel="noreferrer">
                      View report
                    </a>
                  </div>
                )}
                {lastRun?.result?.execution && (
                  <div style={{ marginTop: "0.5rem", fontSize: 14, opacity: 0.9 }}>
                    Execution: {lastRun.result.execution.stepResults.length} steps,{" "}
                    {lastRun.result.execution.defectFlows.length} defect flows
                  </div>
                )}
                {runLog.length > 0 && (
                  <pre
                    style={{
                      marginTop: "0.75rem",
                      padding: "0.75rem",
                      background: "#111827",
                      borderRadius: 8,
                      fontSize: 12,
                      overflow: "auto",
                    }}
                  >
                    {runLog.join("\n")}
                  </pre>
                )}
              </>
            )}
          </section>
        </div>
      )}
    </main>
  );
}