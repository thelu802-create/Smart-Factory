import { useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { notifications as mockNotifications } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function NotificationsPage() {
  const { data: notifications, reload } = useApiData(mockNotifications, factoryApi.getNotifications);
  const [pendingId, setPendingId] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  async function markRead(notificationId: string) {
    setPendingId(notificationId);
    setActionError(null);
    try {
      await factoryApi.markNotificationRead(notificationId);
      reload();
    } catch (error) {
      setActionError(error instanceof Error ? error.message : 'Action failed');
    } finally {
      setPendingId(null);
    }
  }

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Notification center" title="Notifications" description="Centralize safety alerts, production warnings, staffing reminders, and form approval updates." actionLabel="Mark all read" />
      <Panel title="All notifications" eyebrow="Inbox" wide>
        {actionError && <p className="form-action-error">{actionError}</p>}
        <div className="table-list">
          {notifications.map((item) => (
            <div className="data-row four-col" key={item.id}>
              <span><strong>{item.title}</strong><small>{item.type} - {item.time}</small></span>
              <StatusBadge value={item.severity} />
              <StatusBadge value={item.status} />
              <span className="row-actions">
                {item.status === 'Unread' ? (
                  <button className="mini-button approve" disabled={pendingId === item.id} onClick={() => markRead(item.id)}>Mark read</button>
                ) : (
                  <small>—</small>
                )}
              </span>
            </div>
          ))}
        </div>
      </Panel>
    </div>
  );
}
