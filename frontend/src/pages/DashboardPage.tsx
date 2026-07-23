import { Link } from 'react-router-dom';
import { KpiCard } from '../components/ui/KpiCard';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { useApiData } from '../hooks/useApiData';
import { formRequests as mockForms, kpis as mockKpis, productionLines as mockLines, safetyAlerts as mockAlerts, warehouseItems as mockItems } from '../data/mockData';
import type { Recommendation } from '../types';

export function DashboardPage() {
  const { data: kpis } = useApiData(mockKpis, factoryApi.getKpis);
  const { data: productionLines } = useApiData(mockLines, factoryApi.getProductionLines);
  const { data: safetyAlerts } = useApiData(mockAlerts, factoryApi.getSafetyAlerts);
  const { data: warehouseItems } = useApiData(mockItems, factoryApi.getWarehouseItems);
  const { data: aiRecommendations } = useApiData<Recommendation[]>([], factoryApi.getRecommendations);
  const { data: formRequests } = useApiData(mockForms, factoryApi.getFormRequests);

  return (
    <div className="page-stack">
      <PageHeader
        eyebrow="Factory control center"
        title="Overview Dashboard"
        description="Monitor production, warehouse, safety, workforce, forms, and AI recommendations from one operational view."
        actionLabel="Generate report"
      />

      <section className="command-panel">
        <div>
          <p className="eyebrow">Live command view</p>
          <h2>Production is stable, but safety and staffing need attention.</h2>
          <p>AI detected one restricted-zone entry, two warehouse risks, and a likely staffing gap on the night shift.</p>
        </div>
        <div className="command-metrics">
          <span>Target completion <strong>84%</strong></span>
          <span>Open incidents <strong>07</strong></span>
        </div>
      </section>

      <section className="kpi-grid">
        {kpis.map((item) => <KpiCard key={item.label} {...item} />)}
      </section>

      <section className="dashboard-grid">
        <Panel title="Line performance today" eyebrow="Production" action="View all lines" wide>
          <div className="table-list">
            {productionLines.slice(0, 4).map((line) => (
              <Link className="data-row three-col" to="/production" key={line.id}>
                <span><strong>{line.name}</strong><small>{line.issue}</small></span>
                <StatusBadge value={line.status} />
                <strong>{line.efficiency}%</strong>
              </Link>
            ))}
          </div>
        </Panel>

        <Panel title="Priority alerts" eyebrow="Safety" action="Open safety">
          <div className="table-list">
            {safetyAlerts.map((alert) => (
              <Link className="data-row" to="/safety" key={alert.id}>
                <span><strong>{alert.title}</strong><small>{alert.location} at {alert.detectedAt}</small></span>
                <StatusBadge value={alert.severity} />
              </Link>
            ))}
          </div>
        </Panel>

        <Panel title="Warehouse signals" eyebrow="Warehouse" action="Inspect inventory">
          <div className="table-list">
            {warehouseItems.slice(0, 3).map((item) => (
              <Link className="data-row" to="/warehouse" key={item.id}>
                <span><strong>{item.itemCode}</strong><small>{item.itemName} - {item.zone}</small></span>
                <StatusBadge value={item.status} />
              </Link>
            ))}
          </div>
        </Panel>

        <Panel title="AI shift planner" eyebrow="Workforce" action="Adjust schedule" wide>
          <div className="recommendation-list">
            {aiRecommendations.slice(0, 3).map((item, index) => (
              <div className="recommendation-card" key={`${item.title}-${index}`}>
                <strong>{item.title}</strong>
                <p>{item.detail}</p>
              </div>
            ))}
          </div>
        </Panel>

        <Panel title="Pending approvals" eyebrow="Forms" action="Review forms">
          <div className="table-list">
            {formRequests.map((form) => (
              <Link className="data-row" to="/forms" key={form.id}>
                <span><strong>{form.formType}</strong><small>{form.summary}</small></span>
                <StatusBadge value={form.status} />
              </Link>
            ))}
          </div>
        </Panel>
      </section>
    </div>
  );
}