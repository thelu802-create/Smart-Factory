import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { aiRecommendations as mockRecommendations, shiftPlans as mockShifts } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function WorkforcePage() {
  const { data: shiftPlans } = useApiData(mockShifts, factoryApi.getShiftPlans);
  const { data: aiRecommendations } = useApiData(mockRecommendations, factoryApi.getRecommendations);
  const requiredWorkers = shiftPlans.reduce((total, shift) => total + shift.requiredWorkers, 0);
  const assignedWorkers = shiftPlans.reduce((total, shift) => total + shift.assignedWorkers, 0);
  const coverage = requiredWorkers ? Math.round((assignedWorkers / requiredWorkers) * 100) : 0;
  const workerGap = Math.max(0, requiredWorkers - assignedWorkers);
  const suggestedOvertime = Math.max(...shiftPlans.map((shift) => shift.overtimeHours));

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Workforce planning" title="Shift Planning" description="Generate AI-supported shift plans based on production target, employee availability, absences, skills, and previous performance." actionLabel="Generate recommendation" />
      <section className="summary-strip">
        <div><span>Coverage</span><strong>{coverage}%</strong></div>
        <div><span>Worker gap</span><strong>{workerGap}</strong></div>
        <div><span>Suggested overtime</span><strong>{suggestedOvertime}h</strong></div>
      </section>
      <section className="dashboard-grid">
        <Panel title="Shift allocation" eyebrow="Schedule" wide>
          <div className="data-table">
            <div className="table-head workforce-grid"><span>Shift</span><span>Line</span><span>Required</span><span>Assigned</span><span>Overtime</span><span>Status</span></div>
            {shiftPlans.map((shift) => (
              <div className="table-row workforce-grid" key={shift.id}>
                <span>{shift.shiftName}</span><span>{shift.line}</span><strong>{shift.requiredWorkers}</strong><strong>{shift.assignedWorkers}</strong><span>{shift.overtimeHours}h</span><StatusBadge value={shift.status} />
              </div>
            ))}
          </div>
        </Panel>
        <Panel title="AI recommendations" eyebrow="Planner">
          <div className="recommendation-list">
            {aiRecommendations.map((item) => <div className="recommendation-card" key={item}>{item}</div>)}
          </div>
        </Panel>
      </section>
    </div>
  );
}