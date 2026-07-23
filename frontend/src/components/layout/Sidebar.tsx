import { NavLink } from 'react-router-dom';

const mainLinks = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/warehouse', label: 'Warehouse' },
  { to: '/production', label: 'Production' },
  { to: '/cameras', label: 'AI Cameras' },
  { to: '/safety', label: 'Safety' },
  { to: '/analytics', label: 'Analytics' }
];

const operationLinks = [
  { to: '/workforce', label: 'Workforce' },
  { to: '/forms', label: 'Forms' },
  { to: '/notifications', label: 'Notifications' },
  { to: '/reports', label: 'Reports' },
  { to: '/users', label: 'User Management' },
  { to: '/settings', label: 'Settings' }
];

export function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="brand">
        <div className="brand-logo-card">
          <img
            src="https://www.ttigroup.com/themes/custom/tti_theme/assets/images/logo-2.png"
            alt="TTI"
            className="brand-logo"
          />
        </div>
        <div>
          <span>Industrial AI Suite</span>
          <strong>Smart Factory</strong>
        </div>
      </div>

      <nav className="nav-block" aria-label="Main navigation">
        <p>Main</p>
        {mainLinks.map((link) => (
          <NavLink key={link.to} to={link.to} className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}>
            {link.label}
          </NavLink>
        ))}
      </nav>

      <nav className="nav-block" aria-label="Operations navigation">
        <p>Operations</p>
        {operationLinks.map((link) => (
          <NavLink key={link.to} to={link.to} className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}>
            {link.label}
          </NavLink>
        ))}
      </nav>

      <section className="sidebar-insight">
        <span>AI Recommendation</span>
        <strong>Shift optimization ready</strong>
        <p>Line 3 night shift is short by 2 operators against target plan.</p>
      </section>
    </aside>
  );
}