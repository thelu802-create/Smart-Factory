import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { safetyAlerts as mockAlerts } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function SafetyPage() {
  const { data: safetyAlerts } = useApiData(mockAlerts, factoryApi.getSafetyAlerts);

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Safety operations" title="Safety Management" description="Review safety alerts, monitor risk zones, record response actions, and track high-risk areas." actionLabel="Create safety report" />
      <section className="risk-map">
        <div className="risk-zone critical">Robot Cell 2</div>
        <div className="risk-zone warning">Warehouse Zone C</div>
        <div className="risk-zone normal">Assembly Zone A</div>
        <div className="risk-zone high">Storage Room B</div>
      </section>
      <Panel title="Active safety alerts" eyebrow="Incident response" action="Filter alerts" wide>
        <div className="data-table">
          <div className="table-head safety-grid"><span>Alert</span><span>Severity</span><span>Location</span><span>Status</span><span>Detected</span></div>
          {safetyAlerts.map((alert) => (
            <div className="table-row safety-grid" key={alert.id}>
              <span><strong>{alert.title}</strong><small>{alert.description}</small></span>
              <StatusBadge value={alert.severity} />
              <span>{alert.location}</span>
              <StatusBadge value={alert.status} />
              <span>{alert.detectedAt}</span>
            </div>
          ))}
        </div>
      </Panel>
    </div>
  );
}