import type { Kpi } from '../../types';

export function KpiCard({ label, value, detail, trend, tone }: Kpi) {
  return (
    <article className="kpi-card">
      <div className="kpi-topline">
        <span>{label}</span>
        <strong className={`tone-pill ${tone}`}>{trend}</strong>
      </div>
      <h2>{value}</h2>
      <p>{detail}</p>
    </article>
  );
}