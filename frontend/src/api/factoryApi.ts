import type { BatchDecisionResult, CameraDetectionResult, CameraEvent, FormRequest, GoodsMovement, Kpi, NotificationItem, ProductionLine, Recommendation, Role, SafetyAlert, ShiftPlan, User, WarehouseItem, WarehouseZone } from '../types';

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

// Like postJson but surfaces the server's { detail } message on error (used where the
// exact validation reason matters, e.g. creating a user). Also supports other methods.
async function sendJson<T>(method: string, path: string, body?: unknown): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
  const data = await response.json().catch(() => null);
  if (!response.ok) {
    throw new Error((data && (data as { detail?: string }).detail) || `${response.status} ${response.statusText}`);
  }
  return data as T;
}

async function del(path: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}${path}`, { method: 'DELETE' });
  if (!response.ok) {
    let detail = `${response.status} ${response.statusText}`;
    try {
      const body = await response.json();
      if (body?.detail) detail = body.detail;
    } catch {
      // no JSON body
    }
    throw new Error(detail);
  }
}

export const factoryApi = {
  getKpis: () => getJson<Kpi[]>('/dashboard/kpis'),
  getProductionLines: () => getJson<ProductionLine[]>('/production/lines'),
  getSafetyAlerts: () => getJson<SafetyAlert[]>('/safety/alerts'),
  getWarehouseItems: () => getJson<WarehouseItem[]>('/warehouse/items'),
  getShiftPlans: () => getJson<ShiftPlan[]>('/workforce/shifts'),
  getRecommendations: () => getJson<Recommendation[]>('/workforce/recommendations'),
  getFormRequests: () => getJson<FormRequest[]>('/forms'),
  createForm: (payload: { formType: string; requesterId: string; summary: string }) => postJson<FormRequest>('/forms', payload),
  formExportUrl: (formId: string) => `${API_BASE_URL}/forms/${formId}/export`,
  createStockBorrow: (payload: { requesterId: string; itemId: string; quantity: number; note?: string }) =>
    postJson<FormRequest>('/forms/stock-borrow', payload),
  getNotifications: () => getJson<NotificationItem[]>('/notifications'),
  getCameraEvents: () => getJson<CameraEvent[]>('/cameras/events'),
  detectCameraEvent: (payload: { cameraCode: string; eventType: string; severity: string; confidence: number }) =>
    postJson<CameraDetectionResult>('/cameras/detect', payload),
  approveForm: (formId: string, note?: string) => postJson<FormRequest>(`/forms/${formId}/approve`, { note }),
  rejectForm: (formId: string, note?: string) => postJson<FormRequest>(`/forms/${formId}/reject`, { note }),
  approveBatch: (ids: string[], note?: string) => postJson<BatchDecisionResult>('/forms/approve-batch', { ids, note }),
  rejectBatch: (ids: string[], note?: string) => postJson<BatchDecisionResult>('/forms/reject-batch', { ids, note }),
  getUsers: () => getJson<User[]>('/users'),
  getRoles: () => getJson<Role[]>('/users/roles'),
  createUser: (payload: { fullName: string; email: string; roleId: string; department: string }) =>
    sendJson<User>('POST', '/users', payload),
  deleteUser: (userId: string) => del(`/users/${userId}`),
  resolveAlert: (alertId: string, note?: string) => postJson<SafetyAlert>(`/safety/alerts/${alertId}/resolve`, { note }),
  escalateAlert: (alertId: string, note?: string) => postJson<SafetyAlert>(`/safety/alerts/${alertId}/escalate`, { note }),
  markNotificationRead: (notificationId: string) => postJson<NotificationItem>(`/notifications/${notificationId}/read`),
  generateRecommendations: () => postJson<Recommendation[]>('/workforce/recommendations/generate'),
  applyRecommendations: () => postJson<{ assigned: number; recommendations: Recommendation[] }>('/workforce/recommendations/apply'),
  getWarehouseZones: () => getJson<WarehouseZone[]>('/warehouse/zones'),
  getItemMovements: (itemId: string) => getJson<GoodsMovement[]>(`/warehouse/items/${itemId}/movements`),
  moveWarehouseItem: (itemId: string, movementType: 'Import' | 'Export' | 'Transfer', quantity: number, toZoneId?: string, note?: string) =>
    postJson<WarehouseItem>(`/warehouse/items/${itemId}/move`, { movementType, quantity, toZoneId, note })
};