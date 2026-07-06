import type { FormRequest, Kpi, NotificationItem, ProductionLine, SafetyAlert, ShiftPlan, WarehouseItem } from '../types';

export const kpis: Kpi[] = [
  { label: 'Daily Output', value: '12,480', detail: 'Units completed today', trend: '+6.4%', tone: 'good' },
  { label: 'Target Completion', value: '84%', detail: 'Against daily target', trend: 'Needs recovery', tone: 'warning' },
  { label: 'Active Lines', value: '18/20', detail: '2 lines require attention', trend: 'Stable', tone: 'neutral' },
  { label: 'Safety Alerts', value: '7', detail: '1 critical alert open', trend: 'Review now', tone: 'danger' },
  { label: 'Warehouse Occupancy', value: '82%', detail: 'Zone C nearing limit', trend: 'Watch', tone: 'warning' },
  { label: 'Workforce Coverage', value: '91%', detail: 'Night shift gap on Line 3', trend: '-2 workers', tone: 'warning' }
];

export const productionLines: ProductionLine[] = [
  { id: 'line-a', name: 'Line A', area: 'Assembly', status: 'Normal', targetOutput: 2200, actualOutput: 2140, efficiency: 97, defectRate: 1.8, downtimeMinutes: 4, assignedWorkers: 24, issue: 'Running normally' },
  { id: 'line-b', name: 'Line B', area: 'Packaging', status: 'Normal', targetOutput: 1900, actualOutput: 1810, efficiency: 94, defectRate: 2.1, downtimeMinutes: 9, assignedWorkers: 19, issue: 'Minor output gap' },
  { id: 'line-c', name: 'Line C', area: 'Final Assembly', status: 'Warning', targetOutput: 2000, actualOutput: 1560, efficiency: 78, defectRate: 4.6, downtimeMinutes: 18, assignedWorkers: 17, issue: 'Likely staffing shortage' },
  { id: 'line-d', name: 'Line D', area: 'Press', status: 'Stopped', targetOutput: 1600, actualOutput: 860, efficiency: 54, defectRate: 7.2, downtimeMinutes: 48, assignedWorkers: 12, issue: 'Press Unit P-14 vibration' },
  { id: 'line-e', name: 'Line E', area: 'Inspection', status: 'Maintenance', targetOutput: 1200, actualOutput: 0, efficiency: 0, defectRate: 0, downtimeMinutes: 120, assignedWorkers: 6, issue: 'Scheduled calibration' }
];

export const safetyAlerts: SafetyAlert[] = [
  { id: 'safe-001', title: 'Restricted zone entry', type: 'AI Camera', severity: 'Critical', location: 'Robot Cell 2', status: 'New', detectedAt: '09:42', description: 'Worker detected inside restricted area while Line B was active.' },
  { id: 'safe-002', title: 'Forklift congestion', type: 'Traffic', severity: 'Medium', location: 'Warehouse Zone C', status: 'In Review', detectedAt: '10:15', description: 'Traffic density increased during inbound peak.' },
  { id: 'safe-003', title: 'Temperature variance', type: 'Sensor', severity: 'High', location: 'Storage Room B', status: 'Escalated', detectedAt: '10:31', description: 'Room temperature exceeded safe threshold for 6 minutes.' }
];

export const warehouseItems: WarehouseItem[] = [
  { id: 'item-001', ioId: '4229', ioCode: 'TC2', bu: 'Ryobi', itemCode: 'TC2-RM-204', itemName: 'Ryobi TC2 Aluminum Frame', batchCode: 'B-7782', category: 'Raw Material', quantity: 480, zone: 'Finished Goods Zone C', shelf: 'C-18', status: 'Wrong Zone', lastMovementAt: '09:12' },
  { id: 'item-002', ioId: '4070', ioCode: 'TC5', bu: 'Ryobi', itemCode: 'TC5-FK-18', itemName: 'Ryobi TC5 Fastener Kit', batchCode: 'B-2221', category: 'Component', quantity: 56, zone: 'Raw Material Zone A', shelf: 'A-04', status: 'Low Stock', lastMovementAt: '08:26' },
  { id: 'item-003', ioId: '4506', ioCode: 'TN2', bu: 'Ryobi', itemCode: 'TN2-FG-887', itemName: 'Ryobi TN2 Completed Control Box', batchCode: 'B-9920', category: 'Finished Goods', quantity: 320, zone: 'Finished Goods Zone C', shelf: 'C-02', status: 'Correct', lastMovementAt: '10:05' },
  { id: 'item-004', ioId: '3615', ioCode: 'TN5', bu: 'Ryobi', itemCode: 'TN5-PK-441', itemName: 'Ryobi TN5 Packaging Foam', batchCode: 'B-3391', category: 'Packaging', quantity: 900, zone: 'Packaging Zone P', shelf: 'P-11', status: 'Correct', lastMovementAt: '07:44' },
  { id: 'item-005', ioId: '4073', ioCode: 'TCB', bu: 'AES', itemCode: 'TCB-CB-120', itemName: 'AES TCB Control Board', batchCode: 'B-4440', category: 'Component', quantity: 180, zone: 'Raw Material Zone A', shelf: 'A-12', status: 'Correct', lastMovementAt: '08:52' },
  { id: 'item-006', ioId: '4530', ioCode: 'TD3', bu: 'MIL', itemCode: 'TD3-HD-330', itemName: 'MIL TD3 Housing Door', batchCode: 'B-5530', category: 'Raw Material', quantity: 260, zone: 'Raw Material Zone A', shelf: 'A-21', status: 'Correct', lastMovementAt: '09:31' },
  { id: 'item-007', ioId: '4327', ioCode: 'TH3', bu: 'MIL', itemCode: 'TH3-LB-210', itemName: 'MIL TH3 Label Set', batchCode: 'B-6727', category: 'Packaging', quantity: 74, zone: 'Packaging Zone P', shelf: 'P-04', status: 'Low Stock', lastMovementAt: '09:48' },
  { id: 'item-008', ioId: '4226', ioCode: 'TP7', bu: 'MIL', itemCode: 'TP7-FG-510', itemName: 'MIL TP7 Finished Assembly', batchCode: 'B-7226', category: 'Finished Goods', quantity: 145, zone: 'Finished Goods Zone C', shelf: 'C-09', status: 'Correct', lastMovementAt: '10:18' }
];

export const shiftPlans: ShiftPlan[] = [
  { id: 'shift-001', shiftName: 'Morning', line: 'Line A', requiredWorkers: 24, assignedWorkers: 24, overtimeHours: 0, status: 'Published' },
  { id: 'shift-002', shiftName: 'Afternoon', line: 'Line C', requiredWorkers: 21, assignedWorkers: 18, overtimeHours: 1, status: 'Recommended' },
  { id: 'shift-003', shiftName: 'Night', line: 'Line 3', requiredWorkers: 20, assignedWorkers: 18, overtimeHours: 1.5, status: 'Recommended' }
];

export const formRequests: FormRequest[] = [
  { id: 'form-001', formType: 'Overtime Request', requester: 'Nguyen Minh', department: 'Packaging', status: 'Pending Approval', submittedAt: '10:22', summary: '1.5 overtime hours requested for night shift recovery.' },
  { id: 'form-002', formType: 'Machine Issue Report', requester: 'Tran Anh', department: 'Production', status: 'Pending Approval', submittedAt: '09:58', summary: 'Press Unit P-14 vibration detected on Line D.' },
  { id: 'form-003', formType: 'Warehouse Export', requester: 'Le Hoa', department: 'Warehouse', status: 'Approved', submittedAt: '09:35', summary: 'Export request for batch FG-887.' }
];

export const notifications: NotificationItem[] = [
  { id: 'noti-001', title: 'Critical restricted-zone entry', type: 'Safety', severity: 'Critical', status: 'Unread', time: '09:42' },
  { id: 'noti-002', title: 'Line C below target efficiency', type: 'Production', severity: 'High', status: 'Unread', time: '10:03' },
  { id: 'noti-003', title: 'Overtime request pending', type: 'Forms', severity: 'Medium', status: 'Read', time: '10:22' },
  { id: 'noti-004', title: 'Warehouse wrong placement alert', type: 'Warehouse', severity: 'High', status: 'Unread', time: '09:12' }
];

export const aiRecommendations = [
  'Move 2 certified operators from Line 5 to Line 3 for the night shift.',
  'Approve 1.5 hours overtime for packaging team to recover 320 units.',
  'Inspect Press Unit P-14 before restarting Line D.',
  'Relocate RM-204 from Finished Goods Zone C to Raw Material Zone A.'
];