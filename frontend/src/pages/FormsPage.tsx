import { useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { formRequests as mockForms } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

const PENDING_STATUSES = ['Pending Approval', 'Draft'];

export function FormsPage() {
  const { data: formRequests, reload } = useApiData(mockForms, factoryApi.getFormRequests);
  const [pendingId, setPendingId] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  async function runAction(action: () => Promise<unknown>, formId: string) {
    setPendingId(formId);
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
      <PageHeader eyebrow="Workflow automation" title="Electronic Forms" description="Submit, approve, reject, and track operational forms for leave, overtime, machine issues, and warehouse movement." actionLabel="Create form" />
      <Panel title="Approval queue" eyebrow="Pending work" action="Filter forms" wide>
        {actionError && <p className="form-action-error">{actionError}</p>}
        <div className="data-table">
          <div className="table-head forms-grid"><span>Form</span><span>Requester</span><span>Department</span><span>Status</span><span>Submitted</span><span>Actions</span></div>
          {formRequests.map((form) => {
            const isPending = PENDING_STATUSES.includes(form.status);
            const isBusy = pendingId === form.id;
            return (
              <div className="table-row forms-grid" key={form.id}>
                <span><strong>{form.formType}</strong><small>{form.summary}</small></span>
                <span>{form.requester}</span>
                <span>{form.department}</span>
                <StatusBadge value={form.status} />
                <span>{form.submittedAt}</span>
                <span className="row-actions">
                  {isPending ? (
                    <>
                      <button className="mini-button approve" disabled={isBusy} onClick={() => runAction(() => factoryApi.approveForm(form.id), form.id)}>Approve</button>
                      <button className="mini-button reject" disabled={isBusy} onClick={() => runAction(() => factoryApi.rejectForm(form.id), form.id)}>Reject</button>
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
