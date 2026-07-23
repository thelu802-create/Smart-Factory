import { useState } from 'react';
import { PageHeader } from '../components/ui/PageHeader';
import { Panel } from '../components/ui/Panel';
import { factoryApi } from '../api/factoryApi';
import { StatusBadge } from '../components/ui/StatusBadge';
import { useApiData } from '../hooks/useApiData';
import type { CameraEvent, Severity } from '../types';

const cameras = [
  { code: 'CAM-01', area: 'Robot Cell 2' },
  { code: 'CAM-02', area: 'Warehouse Zone C' },
  { code: 'CAM-03', area: 'Line C' },
  { code: 'CAM-04', area: 'Storage Room B' }
];
const eventTypes = ['Restricted Zone Entry', 'Traffic Congestion', 'Obstacle Detected', 'PPE Violation', 'Fire / Smoke'];
const severities: Severity[] = ['Low', 'Medium', 'High', 'Critical'];

const fallbackEvents: CameraEvent[] = [
  { id: 'cam-event-fallback-001', cameraCode: 'CAM-01', location: 'Robot Cell 2', type: 'Restricted Zone Entry', severity: 'Critical', confidence: 0.94, time: '09:42', alertId: 'safe-001' },
  { id: 'cam-event-fallback-002', cameraCode: 'CAM-02', location: 'Warehouse Zone C', type: 'Traffic Congestion', severity: 'Medium', confidence: 0.82, time: '10:15', alertId: null }
];

export function CamerasPage() {
  const { data: cameraEvents, reload } = useApiData(fallbackEvents, factoryApi.getCameraEvents);

  const [cameraCode, setCameraCode] = useState(cameras[0].code);
  const [eventType, setEventType] = useState(eventTypes[0]);
  const [severity, setSeverity] = useState<Severity>('Critical');
  const [confidence, setConfidence] = useState(90);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  // Mirrors the backend auto-alert rule so the operator sees, before running, whether
  // this detection will escalate into a linked safety alert.
  const willRaiseAlert = (severity === 'High' || severity === 'Critical') && confidence >= 80;

  async function runDetection() {
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await factoryApi.detectCameraEvent({ cameraCode, eventType, severity, confidence: confidence / 100 });
      setMessage(result.alertRaised
        ? `Detection logged and escalated: a linked safety alert + notification were raised for "${eventType}".`
        : `Detection logged for "${eventType}". Severity/confidence below threshold, so no safety alert was raised.`);
      reload();
    } catch (detectionError) {
      setError(detectionError instanceof Error ? detectionError.message : 'Detection failed');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="page-stack">
      <PageHeader eyebrow="AI camera monitoring" title="AI Cameras" description="Simulate camera-based monitoring for restricted zones, obstacles, crowding, and safety incidents." actionLabel="Review events" />
      <section className="camera-grid">
        {cameras.map((camera) => <div className="camera-card" key={camera.code}><span>{camera.code}</span><strong>{camera.area}</strong><p>Live simulation feed</p></div>)}
      </section>
      <Panel title="Simulate detection" eyebrow="Camera → Safety → Notification" wide>
        {error && <p className="form-action-error">{error}</p>}
        {message && <p className="form-action-ok">{message}</p>}
        <div className="move-form">
          <label>Camera
            <select value={cameraCode} onChange={(event) => setCameraCode(event.target.value)}>
              {cameras.map((camera) => <option key={camera.code} value={camera.code}>{camera.code} - {camera.area}</option>)}
            </select>
          </label>
          <label>Incident type
            <select value={eventType} onChange={(event) => setEventType(event.target.value)}>
              {eventTypes.map((type) => <option key={type} value={type}>{type}</option>)}
            </select>
          </label>
          <label>Severity
            <select value={severity} onChange={(event) => setSeverity(event.target.value as Severity)}>
              {severities.map((level) => <option key={level} value={level}>{level}</option>)}
            </select>
          </label>
          <label>Confidence ({confidence}%)
            <input className="qty" type="number" min={0} max={100} value={confidence} onChange={(event) => setConfidence(Number(event.target.value))} />
          </label>
          <button className="primary-button" type="button" disabled={busy} onClick={runDetection}>
            {busy ? 'Detecting…' : 'Run detection'}
          </button>
        </div>
        <p className="detection-hint">
          {willRaiseAlert
            ? '⚠ High severity + confidence ≥ 80% → this detection will auto-raise a linked safety alert and notify the safety officer.'
            : 'Low severity or confidence < 80% → this detection will only be logged, without raising a safety alert.'}
        </p>
      </Panel>
      <Panel title="Incident detection log" eyebrow="Camera events" wide>
        <div className="table-list">
          {cameraEvents.map((event) => (
            <div className="data-row" key={event.id}>
              <span>
                <strong>{event.type}</strong>
                <small>{event.cameraCode} - {event.location} at {event.time} - {Math.round(event.confidence * 100)}% confidence</small>
              </span>
              <span className="event-tags">
                {event.alertId && <span className="link-tag" title={`Linked safety alert ${event.alertId}`}>⚠ Safety alert</span>}
                <StatusBadge value={event.severity} />
              </span>
            </div>
          ))}
        </div>
      </Panel>
    </div>
  );
}
