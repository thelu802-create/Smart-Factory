import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { factoryApi } from '../api/factoryApi';
import { productionLines as mockLines } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function AnalyticsPage() {
  const { data: productionLines } = useApiData(mockLines, factoryApi.getProductionLines);

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Productivity analytics" title="Analytics" description="Analyze output trends, target gaps, defect rates, downtime, and AI improvement opportunities." actionLabel="Export analytics" />
      <Panel title="Line efficiency comparison" eyebrow="Performance" wide>
        <div className="bar-list">
          {productionLines.map((line) => <div className="bar-row" key={line.id}><span>{line.name}</span><div><i style={{ width: `${line.efficiency}%` }} /></div><strong>{line.efficiency}%</strong></div>)}
        </div>
      </Panel>
    </div>
  );
}