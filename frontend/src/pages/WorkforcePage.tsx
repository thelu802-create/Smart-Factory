import { useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { shiftPlans as mockShifts } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';
import type { Recommendation } from '../types';

export function WorkforcePage() {
  const { data: shiftPlans, reload: reloadShifts } = useApiData(mockShifts, factoryApi.getShiftPlans);
  const { data: recommendations, reload: reloadRecommendations } = useApiData<Recommendation[]>([], factoryApi.getRecommendations);
  const [generating, setGenerating] = useState(false);
  const [applying, setApplying] = useState(false);
  const [applyMsg, setApplyMsg] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  const requiredWorkers = shiftPlans.reduce((total, shift) => total + shift.requiredWorkers, 0);
  const assignedWorkers = shiftPlans.reduce((total, shift) => total + shift.assignedWorkers, 0);
  const coverage = requiredWorkers ? Math.round((assignedWorkers / requiredWorkers) * 100) : 0;
  const workersShort = Math.max(0, requiredWorkers - assignedWorkers);
  const understaffed = shiftPlans.filter((shift) => shift.assignedWorkers < shift.requiredWorkers).length;

  async function generate() {
    setGenerating(true);
    setActionError(null);
    setApplyMsg(null);
    try {
      await factoryApi.generateRecommendations();
      reloadRecommendations();
    } catch (error) {
      setActionError(error instanceof Error ? error.message : 'Action failed');
    } finally {
      setGenerating(false);
    }
  }

  async function applyPlan() {
    setApplying(true);
    setActionError(null);
    setApplyMsg(null);
    try {
      const result = await factoryApi.applyRecommendations();
      setApplyMsg(result.assigned > 0
        ? `Assigned ${result.assigned} employee(s) to understaffed shifts.`
        : 'No suitable available employees left to assign.');
      reloadRecommendations();
      reloadShifts();
    } catch (error) {
      setActionError(error instanceof Error ? error.message : 'Apply failed');
    } finally {
      setApplying(false);
    }
  }

  return (
    <div className="page-stack">
      <PageHeader
        eyebrow="Workforce planning"
        title="Shift Planning"
        description="See which shifts are understaffed, then let the AI planner suggest and assign available, skilled employees."
      />
      <section className="summary-strip">
        <div><span>Coverage</span><strong>{coverage}%</strong></div>
        <div><span>Workers short</span><strong>{workersShort}</strong></div>
        <div><span>Understaffed shifts</span><strong>{understaffed}</strong></div>
      </section>
      <section className="dashboard-grid">
        <Panel title="Shift allocation" eyebrow="Schedule" wide>
          <div className="data-table">
            <div className="table-head workforce-grid"><span>Shift</span><span>Line</span><span>Staffing</span><span>Overtime</span><span>Status</span></div>
            {shiftPlans.map((shift) => {
              const short = shift.requiredWorkers - shift.assignedWorkers;
              return (
                <div className="table-row workforce-grid" key={shift.id}>
                  <span>{shift.shiftName}</span>
                  <span>{shift.line}</span>
                  <span className="staffing">
                    <strong>{shift.assignedWorkers}/{shift.requiredWorkers}</strong>
                    {short > 0
                      ? <span className="gap-pill short">Short {short}</span>
                      : <span className="gap-pill full">Full</span>}
                  </span>
                  <span>{shift.overtimeHours}h</span>
                  <StatusBadge value={shift.status} />
                </div>
              );
            })}
          </div>
        </Panel>
        <Panel title="AI Shift Planner" eyebrow="Planner">
          {actionError && <p className="form-action-error">{actionError}</p>}
          {applyMsg && <p className="form-action-ok">{applyMsg}</p>}
          <p className="planner-hint">
            Suggests available, skilled employees for understaffed shifts. <strong>Generate</strong> previews a plan; <strong>Apply</strong> assigns them.
          </p>
          <div className="planner-toolbar">
            <button className="ghost-button" onClick={generate} disabled={generating || applying}>
              {generating ? 'Generating…' : '1 · Generate'}
            </button>
            <button className="primary-button" onClick={applyPlan} disabled={applying || generating}>
              {applying ? 'Applying…' : '2 · Apply plan'}
            </button>
          </div>
          <div className="recommendation-list">
            {recommendations.length === 0 ? (
              <p className="planner-empty">No suggestions yet — click Generate to preview a plan.</p>
            ) : (
              recommendations.map((rec, index) => (
                <div className="recommendation-card" key={`${rec.title}-${index}`}>
                  <strong>{rec.title}</strong>
                  <p>{rec.detail}</p>
                </div>
              ))
            )}
          </div>
        </Panel>
      </section>
    </div>
  );
}
