import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';

const settings = ['User Management', 'Role Permissions', 'Factory Areas', 'Production Lines', 'Alert Thresholds', 'AI Recommendation Rules', 'Notification Preferences'];

export function SettingsPage() {
  return (
    <div className="page-stack">
      <PageHeader eyebrow="Administration" title="Settings" description="Configure users, roles, factory areas, production lines, alert thresholds, and AI rules. MVP keeps these settings read-only." />
      <Panel title="Configuration areas" eyebrow="Read-only MVP" wide>
        <div className="settings-grid">
          {settings.map((item) => <div className="settings-card" key={item}><strong>{item}</strong><p>Configuration placeholder for later backend integration.</p></div>)}
        </div>
      </Panel>
    </div>
  );
}