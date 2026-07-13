import type { CameraEvent, FormRequest, Kpi, NotificationItem, ProductionLine, SafetyAlert, ShiftPlan, WarehouseItem } from '../types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8000';

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`);

  if (!response.ok) {
    throw new Error(`API request failed: ${response.status} ${response.statusText}`);
  }

  return response.json() as Promise<T>;
}

async function postJson<T>(path: string, body?: unknown): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body ?? {}),
  });

  if (!response.ok) {
    throw new Error(`API request failed: ${response.status} ${response.statusText}`);
  }

  return response.json() as Promise<T>;
}

export const factoryApi = {
  getKpis: () => getJson<Kpi[]>('/dashboard/kpis'),
  getProductionLines: () => getJson<ProductionLine[]>('/production/lines'),
  getSafetyAlerts: () => getJson<SafetyAlert[]>('/safety/alerts'),
  getWarehouseItems: () => getJson<WarehouseItem[]>('/warehouse/items'),
  getShiftPlans: () => getJson<ShiftPlan[]>('/workforce/shifts'),
  getRecommendations: () => getJson<string[]>('/workforce/recommendations'),
  getFormRequests: () => getJson<FormRequest[]>('/forms'),
  getNotifications: () => getJson<NotificationItem[]>('/notifications'),
  getCameraEvents: () => getJson<CameraEvent[]>('/cameras/events'),
  approveForm: (formId: string, note?: string) => postJson<FormRequest>(`/forms/${formId}/approve`, { note }),
  rejectForm: (formId: string, note?: string) => postJson<FormRequest>(`/forms/${formId}/reject`, { note }),
  resolveAlert: (alertId: string, note?: string) => postJson<SafetyAlert>(`/safety/alerts/${alertId}/resolve`, { note }),
  escalateAlert: (alertId: string, note?: string) => postJson<SafetyAlert>(`/safety/alerts/${alertId}/escalate`, { note }),
  markNotificationRead: (notificationId: string) => postJson<NotificationItem>(`/notifications/${notificationId}/read`),
  generateRecommendations: () => postJson<string[]>('/workforce/recommendations/generate')
};