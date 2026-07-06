import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { factoryApi } from '../api/factoryApi';
import { StatusBadge } from '../components/ui/StatusBadge';
import { useApiData } from '../hooks/useApiData';
import type { CameraEvent } from '../types';

const cameraAreas = ['Robot Cell 2', 'Warehouse Zone C', 'Line C', 'Storage Room B'];
const fallbackEvents: CameraEvent[] = [
  { id: 'cam-event-fallback-001', cameraCode: 'CAM-01', location: 'Robot Cell 2', type: 'Restricted Zone Entry', severity: 'Critical', confidence: 0.94, time: '09:42' },
  { id: 'cam-event-fallback-002', cameraCode: 'CAM-02', location: 'Warehouse Zone C', type: 'Traffic Congestion', severity: 'Medium', confidence: 0.82, time: '10:15' }
];

export function CamerasPage() {
  const { data: cameraEvents } = useApiData(fallbackEvents, factoryApi.getCameraEvents);

  return (
    <div className="page-stack">
      <PageHeader eyebrow="AI camera monitoring" title="AI Cameras" description="Simulate camera-based monitoring for restricted zones, obstacles, crowding, and safety incidents." actionLabel="Review events" />
      <section className="camera-grid">
        {cameraAreas.map((area, index) => <div className="camera-card" key={area}><span>CAM-0{index + 1}</span><strong>{area}</strong><p>Live simulation feed</p></div>)}
      </section>
      <Panel title="Incident detection log" eyebrow="Camera events" wide>
        <div className="table-list">
          {cameraEvents.map((event) => <div className="data-row" key={event.id}><span><strong>{event.type}</strong><small>{event.cameraCode} - {event.location} at {event.time} - {Math.round(event.confidence * 100)}% confidence</small></span><StatusBadge value={event.severity} /></div>)}
        </div>
      </Panel>
    </div>
  );
}