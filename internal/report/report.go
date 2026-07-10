package report

import (
	"fmt"
	"strings"

	"github.com/tejle/SMART/internal/domain"
)

func RenderHTML(run domain.Run) string {
	var b strings.Builder
	b.WriteString("<!doctype html><html><head><meta charset=\"utf-8\"><title>SMART Run Report</title>")
	b.WriteString("<style>body{font-family:system-ui,sans-serif;max-width:900px;margin:2rem auto;padding:0 1rem}")
	b.WriteString("table{border-collapse:collapse;width:100%}td,th{border:1px solid #ddd;padding:.5rem}</style></head><body>")
	fmt.Fprintf(&b, "<h1>Run %s</h1>", run.ID)
	fmt.Fprintf(&b, "<p>Status: <strong>%s</strong> · Kind: %s</p>", run.Status, run.Kind)

	if run.Result == nil {
		b.WriteString("<p>No result payload.</p></body></html>")
		return b.String()
	}

	if run.Result.Generation != nil {
		gen := run.Result.Generation
		fmt.Fprintf(&b, "<h2>Generation</h2><p>%d paths · coverage %.0f%%</p>", len(gen.Paths), gen.StateCoverageRatio*100)
		b.WriteString("<table><tr><th>Path</th><th>Steps</th></tr>")
		for _, path := range gen.Paths {
			labels := make([]string, 0, len(path.Steps))
			for _, step := range path.Steps {
				labels = append(labels, step.StateLabel)
			}
			fmt.Fprintf(&b, "<tr><td>%s</td><td>%s</td></tr>", path.ID, strings.Join(labels, " → "))
		}
		b.WriteString("</table>")
	}

	if run.Result.Execution != nil {
		ex := run.Result.Execution
		fmt.Fprintf(&b, "<h2>Execution</h2><p>%d steps · %d defect flows · %d rounds</p>",
			len(ex.StepResults), len(ex.DefectFlows), ex.CompletedRounds)
		b.WriteString("<table><tr><th>Step</th><th>Result</th></tr>")
		for _, step := range ex.StepResults {
			status := "pass"
			if !step.Success {
				status = "fail"
			}
			fmt.Fprintf(&b, "<tr><td>%s</td><td>%s</td></tr>", step.Step.StateLabel, status)
		}
		b.WriteString("</table>")
	}

	b.WriteString("</body></html>")
	return b.String()
}