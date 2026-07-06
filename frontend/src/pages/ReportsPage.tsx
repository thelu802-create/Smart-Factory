import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';

const reports = [
  ['Production Report', '12,480 units completed', '3.2% above last week'],
  ['Safety Report', '7 active alerts', 'Restricted-zone events increased'],
  ['Warehouse Report', '82% occupancy', 'Zone C nearing capacity'],
  ['Workforce Report', '91% coverage', 'Night shift needs action']
];

export function ReportsPage() {
  return (
    <div className="page-stack">
      <PageHeader eyebrow="Operational reports" title="Reports" description="Preview production, warehouse, safety, workforce, and forms reports using realistic mock data." actionLabel="Export PDF" />
      <section className="report-grid">
        {reports.map(([title, value, detail]) => <article className="report-card" key={title}><span>{title}</span><strong>{value}</strong><p>{detail}</p></article>)}
      </section>
      <Panel title="Report preview" eyebrow="Weekly summary" wide>
        <p className="muted-copy">The MVP simulates report output first. Real PDF or Excel export can be added after backend APIs and database persistence are ready.</p>
      </Panel>
    </div>
  );
}