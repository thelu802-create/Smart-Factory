import { useState } from 'react';
import { factoryApi } from '../../api/factoryApi';
import { notifications as mockNotifications } from '../../data/mockData';
import { useApiData } from '../../hooks/useApiData';
import { StatusBadge } from '../ui/StatusBadge';

export function Topbar() {
  const { data: notifications, reload } = useApiData(mockNotifications, factoryApi.getNotifications);
  const [open, setOpen] = useState(false);
  const unread = notifications.filter((item) => item.status === 'Unread').length;

  function toggle() {
    const next = !open;
    setOpen(next);
    if (next) reload(); // refresh the list each time the bell is opened
  }

  async function markRead(id: string) {
    try {
      await factoryApi.markNotificationRead(id);
      reload();
    } catch {
      // ignore — backend offline; the mock list stays as-is
    }
  }

  return (
    <header className="topbar">
      <label className="global-search">
        <span>Search</span>
        <input placeholder="Line, item, alert, employee" />
      </label>

      <div className="topbar-actions">
        <div className="bell-wrap">
          <button className="bell-button" type="button" onClick={toggle} aria-label="Notifications">
            <span aria-hidden>🔔</span>
            {unread > 0 && <span className="bell-badge">{unread}</span>}
          </button>
          {open && (
            <div className="bell-dropdown">
              <div className="bell-head">
                <strong>Notifications</strong>
                <small>{unread} unread</small>
              </div>
              <div className="bell-list">
                {notifications.length === 0 ? (
                  <p className="bell-empty">No notifications.</p>
                ) : (
                  notifications.slice(0, 10).map((item) => (
                    <button
                      key={item.id}
                      type="button"
                      className={`bell-item${item.status === 'Unread' ? ' unread' : ''}`}
                      onClick={() => item.status === 'Unread' && markRead(item.id)}
                    >
                      <span>
                        <strong>{item.title}</strong>
                        <small>{item.type} · {item.time}</small>
                      </span>
                      <StatusBadge value={item.severity} />
                    </button>
                  ))
                )}
              </div>
            </div>
          )}
        </div>
        <div className="profile-chip">
          <strong>TL</strong>
          <div>
            <span>Operations Manager</span>
            <small>Factory A</small>
          </div>
        </div>
      </div>
    </header>
  );
}
