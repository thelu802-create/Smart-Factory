export type Severity = 'Low' | 'Medium' | 'High' | 'Critical';
export type LineStatus = 'Normal' | 'Warning' | 'Stopped' | 'Maintenance';
export type RecommendationStatus = 'New' | 'Reviewed' | 'Applied' | 'Modified' | 'Rejected';
export type FormStatus = 'Draft' | 'Pending Approval' | 'Approved' | 'Rejected' | 'Cancelled';

export interface Kpi {
  label: string;
  value: string;
  detail: string;
  trend: string;
  tone: 'good' | 'warning' | 'danger' | 'neutral';
}

export interface ProductionLine {
  id: string;
  name: string;
  area: string;
  status: LineStatus;
  targetOutput: number;
  actualOutput: number;
  efficiency: number;
  defectRate: number;
  downtimeMinutes: number;
  assignedWorkers: number;
  issue: string;
}

export interface SafetyAlert {
  id: string;
  title: string;
  type: string;
  severity: Severity;
  location: string;
  status: 'New' | 'In Review' | 'Resolved' | 'Escalated' | 'False Alert';
  detectedAt: string;
  description: string;
}

export interface WarehouseItem {
  id: string;
  ioId: string;
  ioCode: string;
  bu: string;
  itemCode: string;
  itemName: string;
  batchCode: string;
  category: string;
  quantity: number;
  zone: string;
  shelf: string;
  status: 'Correct' | 'Wrong Zone' | 'Low Stock' | 'Over Capacity';
  lastMovementAt: string;
}

export interface ShiftPlan {
  id: string;
  shiftName: string;
  line: string;
  requiredWorkers: number;
  assignedWorkers: number;
  overtimeHours: number;
  status: 'Draft' | 'Recommended' | 'Published';
}

export interface FormRequest {
  id: string;
  formType: string;
  requester: string;
  department: string;
  status: FormStatus;
  submittedAt: string;
  summary: string;
}

export interface NotificationItem {
  id: string;
  title: string;
  type: string;
  severity: Severity;
  status: 'Unread' | 'Read' | 'Resolved';
  time: string;
}

export interface CameraEvent {
  id: string;
  cameraCode: string;
  location: string;
  type: string;
  severity: Severity;
  confidence: number;
  time: string;
}