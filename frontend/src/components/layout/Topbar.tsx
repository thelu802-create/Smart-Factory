import { notifications } from '../../data/mockData';

export function Topbar() {
  const unreadCount = notifications.filter((item) => item.status === 'Unread').length;

  return (
    <header className="topbar">
      <label className="global-search">
        <span>Search</span>
        <input placeholder="Line, item, alert, employee" />
      </label>

      <div className="topbar-actions">
        <button className="alert-button" type="button">
          Alerts <strong>{unreadCount}</strong>
        </button>
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