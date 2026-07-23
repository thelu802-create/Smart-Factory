import { useEffect, useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { Modal } from '../components/ui/Modal';
import { factoryApi } from '../api/factoryApi';
import { useApiData } from '../hooks/useApiData';
import type { Role, User } from '../types';

export function UsersPage() {
  const { data: users, reload } = useApiData<User[]>([], factoryApi.getUsers);
  const { data: roles } = useApiData<Role[]>([], factoryApi.getRoles);

  const [showModal, setShowModal] = useState(false);
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [roleId, setRoleId] = useState('');
  const [department, setDepartment] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [busyId, setBusyId] = useState<string | null>(null);

  const activeCount = users.filter((user) => user.status === 'Active').length;

  // Preselect the first role once the roles list has loaded (it may arrive after the
  // modal is opened), so the dropdown is never left blank when roles are available.
  useEffect(() => {
    if (showModal && !roleId && roles.length > 0) {
      setRoleId(roles[0].id);
    }
  }, [showModal, roles, roleId]);

  function openModal() {
    setFullName('');
    setEmail('');
    setRoleId(roles[0]?.id ?? '');
    setDepartment('');
    setError(null);
    setMessage(null);
    setShowModal(true);
  }

  async function submit() {
    if (!fullName.trim() || !email.trim() || !roleId) {
      setError('Please enter a full name, email and role.');
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      await factoryApi.createUser({ fullName, email, roleId, department });
      setMessage(`User ${fullName} added.`);
      reload();
      setShowModal(false);
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : 'Failed to add user');
    } finally {
      setSubmitting(false);
    }
  }

  async function remove(user: User) {
    if (!window.confirm(`Delete user "${user.fullName}"?`)) return;
    setBusyId(user.id);
    setError(null);
    setMessage(null);
    try {
      await factoryApi.deleteUser(user.id);
      setMessage(`${user.fullName} deleted.`);
      reload();
    } catch (deleteError) {
      setError(deleteError instanceof Error ? deleteError.message : 'Delete failed');
    } finally {
      setBusyId(null);
    }
  }

  return (
    <div className="page-stack">
      <PageHeader
        eyebrow="Administration"
        title="User Management"
        description="Manage user accounts by role and department. Add or remove users."
        actionLabel="Add user"
        onAction={openModal}
      />
      <section className="summary-strip">
        <div><span>Total users</span><strong>{users.length}</strong></div>
        <div><span>Active</span><strong>{activeCount}</strong></div>
        <div><span>Roles</span><strong>{roles.length}</strong></div>
      </section>
      <Panel title="User directory" eyebrow="Users" wide>
        {error && <p className="form-action-error">{error}</p>}
        {message && <p className="form-action-ok">{message}</p>}
        <div className="data-table">
          <div className="table-head users-grid">
            <span>Name</span><span>Email</span><span>Role</span><span>Department</span><span>Status</span><span>Created</span><span></span>
          </div>
          {users.map((user) => (
            <div className="table-row users-grid" key={user.id}>
              <span><strong>{user.fullName}</strong><small>{user.id}</small></span>
              <span>{user.email}</span>
              <span>{user.role}</span>
              <span>{user.department}</span>
              <StatusBadge value={user.status} />
              <span>{user.createdAt}</span>
              <span className="row-actions">
                <button className="mini-button reject" disabled={busyId === user.id} onClick={() => remove(user)}>Delete</button>
              </span>
            </div>
          ))}
        </div>
      </Panel>

      {showModal && (
        <Modal title="Add user" eyebrow="New account" onClose={() => setShowModal(false)}>
          <form className="create-form" onSubmit={(event) => { event.preventDefault(); submit(); }}>
            {error && <p className="form-action-error">{error}</p>}
            <div className="create-form-grid">
              <label>Full name *
                <input value={fullName} placeholder="Jane Doe" onChange={(event) => setFullName(event.target.value)} />
              </label>
              <label>Email *
                <input type="email" value={email} placeholder="jane@factory.local" onChange={(event) => setEmail(event.target.value)} />
              </label>
              <label>Role *
                <select value={roleId} onChange={(event) => setRoleId(event.target.value)}>
                  <option value="">{roles.length === 0 ? 'Loading roles…' : 'Select role…'}</option>
                  {roles.map((role) => <option key={role.id} value={role.id}>{role.name}</option>)}
                </select>
              </label>
              <label>Department
                <input value={department} placeholder="Operations" onChange={(event) => setDepartment(event.target.value)} />
              </label>
            </div>
            <div className="create-form-actions">
              <button type="button" className="ghost-button" onClick={() => setShowModal(false)}>Cancel</button>
              <button type="submit" className="primary-button" disabled={submitting}>{submitting ? 'Saving…' : 'Add user'}</button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
