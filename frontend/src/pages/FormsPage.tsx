import { useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { Modal } from '../components/ui/Modal';
import { factoryApi } from '../api/factoryApi';
import { formRequests as mockForms } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';
import type { FormRequest, FormStatus, WarehouseItem } from '../types';

const PENDING_STATUSES: FormStatus[] = ['Pending Approval', 'Draft'];

const FORM_TYPES = [
  'Leave Request',
  'Overtime Request',
  'Machine Issue Report',
  'Warehouse Import',
  'Warehouse Export',
  'Warehouse Borrow',
] as const;
type CreateFormType = (typeof FORM_TYPES)[number];

// Demo requesters mapped to seed users so the created form shows a real name/department.
const REQUESTERS = [
  { id: 'user-005', name: 'Nguyen Minh', department: 'Packaging' },
  { id: 'user-002', name: 'Tran Anh', department: 'Production' },
  { id: 'user-003', name: 'Le Hoa', department: 'Warehouse' },
  { id: 'user-004', name: 'Pham Linh', department: 'Safety' },
  { id: 'user-001', name: 'The Lu Nguyen', department: 'Operations' },
];

interface FieldDef {
  key: string;
  label: string;
  type?: 'text' | 'number' | 'date' | 'textarea' | 'select';
  options?: string[];
  required?: boolean;
  placeholder?: string;
}

// Field sets per form type (requirements 18.7).
const FIELD_DEFS: Record<CreateFormType, FieldDef[]> = {
  'Leave Request': [
    { key: 'leaveDate', label: 'Leave date', type: 'date', required: true },
    { key: 'leaveType', label: 'Leave type', type: 'select', options: ['Annual', 'Sick', 'Unpaid'], required: true },
    { key: 'shift', label: 'Shift', type: 'select', options: ['Morning', 'Afternoon', 'Night'] },
    { key: 'reason', label: 'Reason', type: 'textarea', required: true, placeholder: 'Why is the leave needed?' },
  ],
  'Overtime Request': [
    { key: 'line', label: 'Production line', type: 'text', required: true, placeholder: 'e.g. Line C' },
    { key: 'date', label: 'Date', type: 'date', required: true },
    { key: 'hours', label: 'Overtime hours', type: 'number', required: true, placeholder: 'e.g. 1.5' },
    { key: 'reason', label: 'Reason', type: 'textarea', required: true, placeholder: 'Reason for overtime' },
  ],
  'Machine Issue Report': [
    { key: 'machineCode', label: 'Machine code', type: 'text', required: true, placeholder: 'e.g. P-14' },
    { key: 'line', label: 'Production line', type: 'text', required: true, placeholder: 'e.g. Line D' },
    { key: 'severity', label: 'Severity', type: 'select', options: ['Low', 'Medium', 'High', 'Critical'], required: true },
    { key: 'description', label: 'Description', type: 'textarea', required: true, placeholder: 'Describe the fault' },
  ],
  'Warehouse Import': [
    { key: 'itemCode', label: 'Item code', type: 'text', required: true, placeholder: 'e.g. RM-204' },
    { key: 'quantity', label: 'Quantity', type: 'number', required: true, placeholder: 'e.g. 120' },
    { key: 'batchCode', label: 'Batch code', type: 'text', required: true, placeholder: 'e.g. B-2026-07' },
    { key: 'zone', label: 'Warehouse zone', type: 'text', required: true, placeholder: 'e.g. Raw Material Zone A' },
    { key: 'reason', label: 'Reason', type: 'textarea', placeholder: 'Optional note' },
  ],
  'Warehouse Export': [
    { key: 'itemCode', label: 'Item code', type: 'text', required: true, placeholder: 'e.g. FG-887' },
    { key: 'quantity', label: 'Quantity', type: 'number', required: true, placeholder: 'e.g. 60' },
    { key: 'batchCode', label: 'Batch code', type: 'text', required: true, placeholder: 'e.g. B-2026-07' },
    { key: 'zone', label: 'Warehouse zone', type: 'text', required: true, placeholder: 'e.g. Finished Goods Zone C' },
    { key: 'reason', label: 'Reason', type: 'textarea', placeholder: 'Optional note' },
  ],
  // Rendered with a custom item picker + quantity (submits to /forms/stock-borrow).
  'Warehouse Borrow': [],
};

function buildSummary(type: CreateFormType, f: Record<string, string>): string {
  switch (type) {
    case 'Leave Request':
      return `${f.leaveType} leave on ${f.leaveDate}${f.shift ? ` (${f.shift} shift)` : ''} — ${f.reason}`;
    case 'Overtime Request':
      return `${f.hours}h overtime on ${f.line} (${f.date}) — ${f.reason}`;
    case 'Machine Issue Report':
      return `${f.severity} issue on ${f.machineCode} / ${f.line} — ${f.description}`;
    case 'Warehouse Import':
      return `Import ${f.quantity} of ${f.itemCode} (batch ${f.batchCode}) to ${f.zone}${f.reason ? ` — ${f.reason}` : ''}`;
    case 'Warehouse Export':
      return `Export ${f.quantity} of ${f.itemCode} (batch ${f.batchCode}) from ${f.zone}${f.reason ? ` — ${f.reason}` : ''}`;
    default:
      return ''; // Warehouse Borrow builds its summary server-side
  }
}

export function FormsPage() {
  const { data: formRequests, reload } = useApiData(mockForms, factoryApi.getFormRequests);
  const { data: warehouseItems } = useApiData<WarehouseItem[]>([], factoryApi.getWarehouseItems);
  const [localForms, setLocalForms] = useState<FormRequest[]>([]);
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [bulkBusy, setBulkBusy] = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  // Create-form modal state
  const [showModal, setShowModal] = useState(false);
  const [formType, setFormType] = useState<CreateFormType>('Leave Request');
  const [requesterId, setRequesterId] = useState(REQUESTERS[0].id);
  const [fields, setFields] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const allForms = [...localForms, ...formRequests];
  const fieldDefs = FIELD_DEFS[formType];

  const selectedForms = allForms.filter((form) => selected.has(form.id));
  const pendingSelected = selectedForms.filter((form) => PENDING_STATUSES.includes(form.status));
  const exportableSelected = selectedForms.filter((form) => !form.id.startsWith('local-'));
  const allSelected = allForms.length > 0 && allForms.every((form) => selected.has(form.id));

  function openModal() {
    setFormType('Leave Request');
    setRequesterId(REQUESTERS[0].id);
    setFields({});
    setSubmitError(null);
    setSuccessMsg(null);
    setShowModal(true);
  }

  function onTypeChange(type: CreateFormType) {
    setFormType(type);
    setFields({});
    setSubmitError(null);
  }

  function setField(key: string, value: string) {
    setFields((prev) => ({ ...prev, [key]: value }));
  }

  function validate(): string | null {
    for (const def of fieldDefs) {
      const value = (fields[def.key] ?? '').trim();
      if (def.required && !value) return `${def.label} is required.`;
      if (def.type === 'number' && value && Number(value) <= 0) return `${def.label} must be greater than 0.`;
    }
    return null;
  }

  async function submitBorrow() {
    const itemId = (fields.itemId ?? '').trim();
    const quantity = Number(fields.quantity ?? 0);
    if (!itemId) { setSubmitError('Please select an item to borrow.'); return; }
    if (!(quantity > 0)) { setSubmitError('Borrow quantity must be greater than 0.'); return; }

    setSubmitting(true);
    setSubmitError(null);
    try {
      await factoryApi.createStockBorrow({ requesterId, itemId, quantity, note: fields.note });
      setSuccessMsg('Borrow request submitted for warehouse approval.');
      reload();
      setShowModal(false);
    } catch (error) {
      setSubmitError(error instanceof Error ? error.message : 'Submit failed');
    } finally {
      setSubmitting(false);
    }
  }

  async function submitForm() {
    if (formType === 'Warehouse Borrow') {
      await submitBorrow();
      return;
    }

    const validationError = validate();
    if (validationError) {
      setSubmitError(validationError);
      return;
    }
    const requester = REQUESTERS.find((r) => r.id === requesterId) ?? REQUESTERS[0];
    const summary = buildSummary(formType, fields);

    setSubmitting(true);
    setSubmitError(null);
    try {
      await factoryApi.createForm({ formType, requesterId, summary });
      setSuccessMsg(`${formType} submitted for approval.`);
      reload();
      setShowModal(false);
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Submit failed';
      // Genuine server responses (e.g. validation) keep the modal open with the message;
      // a network failure means the backend is offline, so add the form locally for the demo.
      if (message.startsWith('API request failed')) {
        setSubmitError(message);
        return;
      }
      const local: FormRequest = {
        id: `local-${Date.now()}`,
        formType,
        requester: requester.name,
        department: requester.department,
        status: 'Pending Approval',
        submittedAt: new Date().toTimeString().slice(0, 5),
        summary,
      };
      setLocalForms((prev) => [local, ...prev]);
      setSuccessMsg(`${formType} saved locally (offline demo).`);
      setShowModal(false);
    } finally {
      setSubmitting(false);
    }
  }

  function localDecision(formId: string, status: FormStatus) {
    setLocalForms((prev) => prev.map((form) => (form.id === formId ? { ...form, status } : form)));
  }

  function toggleOne(id: string) {
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  }

  function toggleAll() {
    setSelected(allSelected ? new Set() : new Set(allForms.map((form) => form.id)));
  }

  // Approve / reject every selected pending form. Server forms go through one batch
  // endpoint; offline-local forms are decided in-memory.
  async function bulkDecide(kind: 'Approved' | 'Rejected') {
    if (pendingSelected.length === 0) return;
    setBulkBusy(true);
    setActionError(null);
    setSuccessMsg(null);

    const localOnes = pendingSelected.filter((form) => form.id.startsWith('local-'));
    const serverIds = pendingSelected.filter((form) => !form.id.startsWith('local-')).map((form) => form.id);
    localOnes.forEach((form) => localDecision(form.id, kind));

    let succeeded = localOnes.length;
    let failed = 0;
    const errors: string[] = [];

    if (serverIds.length > 0) {
      try {
        const res = kind === 'Approved'
          ? await factoryApi.approveBatch(serverIds)
          : await factoryApi.rejectBatch(serverIds);
        succeeded += res.succeeded;
        failed += res.failed;
        res.results.filter((r) => r.status !== 'ok').forEach((r) => errors.push(`${r.id}: ${r.error ?? 'error'}`));
      } catch (error) {
        failed += serverIds.length;
        errors.push(error instanceof Error ? error.message : 'Batch failed');
      }
    }

    reload();
    setSelected(new Set());
    setBulkBusy(false);

    const verb = kind === 'Approved' ? 'Approved' : 'Rejected';
    if (failed > 0) {
      setActionError(`${verb} ${succeeded} form(s), ${failed} failed: ${errors.join('; ')}`);
    } else {
      setSuccessMsg(`${verb} ${succeeded} form(s).`);
    }
  }

  // Trigger a PDF download per selected (non-local) form.
  function bulkExport() {
    for (const form of exportableSelected) {
      const anchor = document.createElement('a');
      anchor.href = factoryApi.formExportUrl(form.id);
      document.body.appendChild(anchor);
      anchor.click();
      anchor.remove();
    }
  }

  return (
    <div className="page-stack">
      <PageHeader
        eyebrow="Workflow automation"
        title="Electronic Forms"
        description="Submit, approve, reject, and track operational forms for leave, overtime, machine issues, and warehouse movement."
        actionLabel="Create form"
        onAction={openModal}
      />
      <Panel title="Approval queue" eyebrow="Pending work" action="Filter forms" wide>
        {actionError && <p className="form-action-error">{actionError}</p>}
        {successMsg && <p className="form-action-ok">{successMsg}</p>}
        <div className="bulk-bar">
          <span className="bulk-count">{selected.size > 0 ? `${selected.size} selected` : 'Select forms for bulk actions'}</span>
          <div className="bulk-actions">
            <button className="mini-button approve" disabled={bulkBusy || pendingSelected.length === 0} onClick={() => bulkDecide('Approved')}>
              {bulkBusy ? 'Working…' : `Approve${pendingSelected.length > 0 ? ` (${pendingSelected.length})` : ''}`}
            </button>
            <button className="mini-button reject" disabled={bulkBusy || pendingSelected.length === 0} onClick={() => bulkDecide('Rejected')}>
              Reject{pendingSelected.length > 0 ? ` (${pendingSelected.length})` : ''}
            </button>
            <button className="mini-button export" disabled={bulkBusy || exportableSelected.length === 0} onClick={bulkExport}>
              Export PDF{exportableSelected.length > 0 ? ` (${exportableSelected.length})` : ''}
            </button>
          </div>
        </div>
        <div className="data-table">
          <div className="table-head forms-grid">
            <span><input type="checkbox" checked={allSelected} onChange={toggleAll} aria-label="Select all" /></span>
            <span>Form</span><span>Requester</span><span>Department</span><span>Status</span><span>Submitted</span>
          </div>
          {allForms.map((form) => (
            <div className={`table-row forms-grid${selected.has(form.id) ? ' selected' : ''}`} key={form.id}>
              <span><input type="checkbox" checked={selected.has(form.id)} onChange={() => toggleOne(form.id)} aria-label="Select form" /></span>
              <span><strong>{form.formType}</strong><small>{form.summary}</small></span>
              <span>{form.requester}</span>
              <span>{form.department}</span>
              <StatusBadge value={form.status} />
              <span>{form.submittedAt}</span>
            </div>
          ))}
        </div>
      </Panel>

      {showModal && (
        <Modal title="Create form" eyebrow="New request" onClose={() => setShowModal(false)}>
          <form
            className="create-form"
            onSubmit={(event) => {
              event.preventDefault();
              submitForm();
            }}
          >
            {submitError && <p className="form-action-error">{submitError}</p>}

            <div className="create-form-grid">
              <label>Form type
                <select value={formType} onChange={(event) => onTypeChange(event.target.value as CreateFormType)}>
                  {FORM_TYPES.map((type) => <option key={type} value={type}>{type}</option>)}
                </select>
              </label>
              <label>Requester
                <select value={requesterId} onChange={(event) => setRequesterId(event.target.value)}>
                  {REQUESTERS.map((r) => <option key={r.id} value={r.id}>{r.name} — {r.department}</option>)}
                </select>
              </label>
            </div>

            {formType === 'Warehouse Borrow' ? (
              <div className="create-form-grid">
                <label className="full">Item to borrow *
                  <select value={fields.itemId ?? ''} onChange={(event) => setField('itemId', event.target.value)}>
                    <option value="">Select item…</option>
                    {warehouseItems.map((item) => (
                      <option key={item.id} value={item.id}>{item.itemCode} - {item.itemName} (stock {item.quantity}, {item.zone})</option>
                    ))}
                  </select>
                </label>
                <label>Borrow quantity *
                  <input type="number" min={1} step={1} value={fields.quantity ?? ''} placeholder="e.g. 20" onChange={(event) => setField('quantity', event.target.value)} />
                </label>
                <label className="full">Note
                  <textarea rows={2} value={fields.note ?? ''} placeholder="Reason for borrowing (optional)" onChange={(event) => setField('note', event.target.value)} />
                </label>
              </div>
            ) : (
              <div className="create-form-grid">
                {fieldDefs.map((def) => (
                  <label key={def.key} className={def.type === 'textarea' ? 'full' : undefined}>
                    {def.label}{def.required ? ' *' : ''}
                    {def.type === 'select' ? (
                      <select value={fields[def.key] ?? ''} onChange={(event) => setField(def.key, event.target.value)}>
                        <option value="">Select…</option>
                        {def.options?.map((option) => <option key={option} value={option}>{option}</option>)}
                      </select>
                    ) : def.type === 'textarea' ? (
                      <textarea rows={2} value={fields[def.key] ?? ''} placeholder={def.placeholder} onChange={(event) => setField(def.key, event.target.value)} />
                    ) : (
                      <input
                        type={def.type ?? 'text'}
                        min={def.type === 'number' ? 0 : undefined}
                        step={def.type === 'number' ? 'any' : undefined}
                        value={fields[def.key] ?? ''}
                        placeholder={def.placeholder}
                        onChange={(event) => setField(def.key, event.target.value)}
                      />
                    )}
                  </label>
                ))}
              </div>
            )}

            <div className="create-form-actions">
              <button type="button" className="ghost-button" onClick={() => setShowModal(false)}>Cancel</button>
              <button type="submit" className="primary-button" disabled={submitting}>
                {submitting ? 'Submitting…' : 'Submit for approval'}
              </button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
