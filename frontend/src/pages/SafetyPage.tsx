import { useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { safetyAlerts as mockAlerts } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

const OPEN_STATUSES = ['New', 'In Review'];

export function SafetyPage() {
  const { data: safetyAlerts, reload } = useApiData(mockAlerts, factoryApi.getSafetyAlerts);
  const [pendingId, setPendingId] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  async function runAction(action: () => Promise<unknown>, alertId: string) {
    setPendingId(alertId);
    setActionError(null);
    try {
      await action();
      reload();
    } catch (error) {
      setActionError(error instanceof Error ? error.message : 'Action failed');
    } finally {
      setPendingId(null);
    }
  }

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
        {actionError && <p className="form-action-error">{actionError}</p>}
        <div className="data-table">
          <div className="table-head safety-grid"><span>Alert</span><span>Severity</span><span>Location</span><span>Status</span><span>Detected</span><span>Actions</span></div>
          {safetyAlerts.map((alert) => {
            const isOpen = OPEN_STATUSES.includes(alert.status);
            const isBusy = pendingId === alert.id;
            return (
              <div className="table-row safety-grid" key={alert.id}>
                <span><strong>{alert.title}</strong><small>{alert.description}</small></span>
                <StatusBadge value={alert.severity} />
                <span>{alert.location}</span>
                <StatusBadge value={alert.status} />
                <span>{alert.detectedAt}</span>
                <span className="row-actions">
                  {isOpen ? (
                    <>
                      <button className="mini-button approve" disabled={isBusy} onClick={() => runAction(() => factoryApi.resolveAlert(alert.id), alert.id)}>Resolve</button>
                      <button className="mini-button escalate" disabled={isBusy} onClick={() => runAction(() => factoryApi.escalateAlert(alert.id), alert.id)}>Escalate</button>
                    </>
                  ) : (
                    <small>Closed</small>
                  )}
                </span>
              </div>
            );
          })}
        </div>
      </Panel>
    </div>
  );
}
