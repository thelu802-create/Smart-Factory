import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { formRequests as mockForms } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function FormsPage() {
  const { data: formRequests } = useApiData(mockForms, factoryApi.getFormRequests);

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Workflow automation" title="Electronic Forms" description="Submit, approve, reject, and track operational forms for leave, overtime, machine issues, and warehouse movement." actionLabel="Create form" />
      <Panel title="Approval queue" eyebrow="Pending work" action="Filter forms" wide>
        <div className="data-table">
          <div className="table-head forms-grid"><span>Form</span><span>Requester</span><span>Department</span><span>Status</span><span>Submitted</span></div>
          {formRequests.map((form) => (
            <div className="table-row forms-grid" key={form.id}>
              <span><strong>{form.formType}</strong><small>{form.summary}</small></span>
              <span>{form.requester}</span>
              <span>{form.department}</span>
              <StatusBadge value={form.status} />
              <span>{form.submittedAt}</span>
            </div>
          ))}
        </div>
      </Panel>
    </div>
  );
}