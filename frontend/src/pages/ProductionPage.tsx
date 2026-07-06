import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { productionLines as mockLines } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function ProductionPage() {
  const { data: productionLines } = useApiData(mockLines, factoryApi.getProductionLines);
  const warningLines = productionLines.filter((line) => line.status !== 'Normal');
  const averageEfficiency = Math.round(productionLines.reduce((total, line) => total + line.efficiency, 0) / productionLines.length);

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Production monitoring" title="Production Lines" description="Track line output, efficiency, downtime, defects, staffing, and AI root-cause suggestions." actionLabel="Create issue report" />
      <section className="summary-strip">
        <div><span>Total lines</span><strong>{productionLines.length}</strong></div>
        <div><span>Need attention</span><strong>{warningLines.length}</strong></div>
        <div><span>Average efficiency</span><strong>{averageEfficiency}%</strong></div>
      </section>
      <Panel title="Line overview" eyebrow="Live status" action="Filter lines" wide>
        <div className="data-table">
          <div className="table-head production-grid"><span>Line</span><span>Status</span><span>Output</span><span>Efficiency</span><span>Defects</span><span>Downtime</span></div>
          {productionLines.map((line) => (
            <div className="table-row production-grid" key={line.id}>
              <span><strong>{line.name}</strong><small>{line.area} - {line.issue}</small></span>
              <StatusBadge value={line.status} />
              <span>{line.actualOutput} / {line.targetOutput}</span>
              <strong>{line.efficiency}%</strong>
              <span>{line.defectRate}%</span>
              <span>{line.downtimeMinutes} min</span>
            </div>
          ))}
        </div>
      </Panel>
    </div>
  );
}