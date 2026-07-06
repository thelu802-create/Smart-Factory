import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { StatusBadge } from '../components/ui/StatusBadge';
import { factoryApi } from '../api/factoryApi';
import { notifications as mockNotifications } from '../data/mockData';
import { useApiData } from '../hooks/useApiData';

export function NotificationsPage() {
  const { data: notifications } = useApiData(mockNotifications, factoryApi.getNotifications);

  return (
    <div className="page-stack">
      <PageHeader eyebrow="Notification center" title="Notifications" description="Centralize safety alerts, production warnings, staffing reminders, and form approval updates." actionLabel="Mark all read" />
      <Panel title="All notifications" eyebrow="Inbox" wide>
        <div className="table-list">
          {notifications.map((item) => <div className="data-row three-col" key={item.id}><span><strong>{item.title}</strong><small>{item.type} - {item.time}</small></span><StatusBadge value={item.severity} /><StatusBadge value={item.status} /></div>)}
        </div>
      </Panel>
    </div>
  );
}