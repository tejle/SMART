import type { SequenceItem } from "./testSequence";

const badge = (label: string, color: string) => (
  <span
    style={{
      fontSize: 10,
      fontWeight: 600,
      textTransform: "uppercase",
      letterSpacing: "0.04em",
      color,
      minWidth: 72,
    }}
  >
    {label}
  </span>
);

type Props = {
  items: SequenceItem[];
  activeIndex?: number;
  compact?: boolean;
};

export default function SequenceList({ items, activeIndex, compact }: Props) {
  return (
    <ol
      style={{
        listStyle: "none",
        padding: 0,
        margin: 0,
        display: "grid",
        gap: compact ? "0.35rem" : "0.5rem",
      }}
    >
      {items.map((item, index) => {
        const active = activeIndex === index;
        const baseStyle = {
          padding: compact ? "0.4rem 0.55rem" : "0.55rem 0.65rem",
          borderRadius: 6,
          border: active ? "1px solid #60a5fa" : "1px solid #2a3558",
          background: active ? "#172554" : "transparent",
          fontSize: compact ? 11 : 12,
        };

        if (item.kind === "validate") {
          return (
            <li key={`${index}-validate`} style={baseStyle}>
              <div style={{ display: "flex", gap: "0.5rem", alignItems: "baseline" }}>
                {badge("Validate", "#4ade80")}
                <span>
                  State <strong>{item.stateLabel}</strong>
                </span>
              </div>
            </li>
          );
        }

        return (
          <li key={`${index}-traverse`} style={baseStyle}>
            <div style={{ display: "flex", gap: "0.5rem", alignItems: "baseline", flexWrap: "wrap" }}>
              {badge("Traverse", "#60a5fa")}
              <span>
                {item.guard ? (
                  <>
                    If <em>{item.guard}</em>, then{" "}
                  </>
                ) : null}
                action <strong>{item.action}</strong>
              </span>
            </div>
            {!compact && item.guard && (
              <div style={{ marginTop: 4, opacity: 0.7, fontSize: 11 }}>
                Condition evaluated before leaving prior state
              </div>
            )}
          </li>
        );
      })}
    </ol>
  );
}