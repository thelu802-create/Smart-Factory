INSERT INTO roles (id, name, description) VALUES
('role-factory-manager', 'Factory Manager', 'Full operational visibility and approval authority'),
('role-production-manager', 'Production Manager', 'Manages production lines, downtime, and output issues'),
('role-warehouse-manager', 'Warehouse Manager', 'Manages inventory, item locations, and warehouse alerts'),
('role-safety-officer', 'Safety Officer', 'Handles safety alerts, camera incidents, and risk areas'),
('role-employee', 'Employee', 'Submits forms, receives notifications, and views assignments');

INSERT INTO users (id, full_name, email, role_id, department, status, created_at) VALUES
('user-001', 'The Lu Nguyen', 'manager@factory.local', 'role-factory-manager', 'Operations', 'Active', '2026-07-03T08:00:00'),
('user-002', 'Tran Anh', 'production@factory.local', 'role-production-manager', 'Production', 'Active', '2026-07-03T08:00:00'),
('user-003', 'Le Hoa', 'warehouse@factory.local', 'role-warehouse-manager', 'Warehouse', 'Active', '2026-07-03T08:00:00'),
('user-004', 'Pham Linh', 'safety@factory.local', 'role-safety-officer', 'Safety', 'Active', '2026-07-03T08:00:00'),
('user-005', 'Nguyen Minh', 'employee@factory.local', 'role-employee', 'Packaging', 'Active', '2026-07-03T08:00:00'),
('user-006', 'Vo Thanh', 'thanh@factory.local', 'role-employee', 'Production', 'Active', '2026-07-04T08:00:00'),
('user-007', 'Dang Hue', 'hue@factory.local', 'role-employee', 'Quality', 'Active', '2026-07-04T08:00:00'),
('user-008', 'Ho Nam', 'namho@factory.local', 'role-production-manager', 'Production', 'Active', '2026-07-05T08:00:00'),
('user-009', 'Ly Mai', 'mai@factory.local', 'role-warehouse-manager', 'Warehouse', 'Inactive', '2026-07-05T08:00:00'),
('user-010', 'Phan Duc', 'duc@factory.local', 'role-safety-officer', 'Safety', 'Active', '2026-07-06T08:00:00'),
('user-011', 'Tran Yen', 'yen@factory.local', 'role-employee', 'Packaging', 'Active', '2026-07-06T08:00:00');

INSERT INTO permissions (id, permission_code, module, action, description) VALUES
('perm-dashboard-view', 'DASHBOARD_VIEW', 'Dashboard', 'View', 'View dashboard KPIs and operational summary'),
('perm-production-manage', 'PRODUCTION_MANAGE', 'Production', 'Manage', 'Manage production lines and issues'),
('perm-warehouse-manage', 'WAREHOUSE_MANAGE', 'Warehouse', 'Manage', 'Manage warehouse items, IO, BU, and movements'),
('perm-safety-manage', 'SAFETY_MANAGE', 'Safety', 'Manage', 'Manage safety alerts and camera events'),
('perm-forms-approve', 'FORMS_APPROVE', 'Forms', 'Approve', 'Approve or reject workflow forms'),
('perm-reports-view', 'REPORTS_VIEW', 'Reports', 'View', 'View operational reports');

INSERT INTO role_permissions (role_id, permission_id, granted_at) VALUES
('role-factory-manager', 'perm-dashboard-view', '2026-07-03T08:00:00'),
('role-factory-manager', 'perm-production-manage', '2026-07-03T08:00:00'),
('role-factory-manager', 'perm-warehouse-manage', '2026-07-03T08:00:00'),
('role-factory-manager', 'perm-safety-manage', '2026-07-03T08:00:00'),
('role-factory-manager', 'perm-forms-approve', '2026-07-03T08:00:00'),
('role-factory-manager', 'perm-reports-view', '2026-07-03T08:00:00'),
('role-production-manager', 'perm-dashboard-view', '2026-07-03T08:00:00'),
('role-production-manager', 'perm-production-manage', '2026-07-03T08:00:00'),
('role-warehouse-manager', 'perm-dashboard-view', '2026-07-03T08:00:00'),
('role-warehouse-manager', 'perm-warehouse-manage', '2026-07-03T08:00:00'),
('role-safety-officer', 'perm-dashboard-view', '2026-07-03T08:00:00'),
('role-safety-officer', 'perm-safety-manage', '2026-07-03T08:00:00');

INSERT INTO factory_areas (id, name, area_type, risk_level, description) VALUES
('area-assembly', 'Assembly Zone A', 'Production', 'Low', 'Main assembly line area'),
('area-packaging', 'Packaging Zone P', 'Production', 'Medium', 'Packaging and labeling area'),
('area-final', 'Final Assembly Zone C', 'Production', 'Medium', 'Final assembly and inspection area'),
('area-press', 'Press Zone D', 'Production', 'High', 'Heavy press machine area'),
('area-robot', 'Robot Cell 2', 'Safety', 'Critical', 'Restricted robot automation cell'),
('area-warehouse-c', 'Warehouse Zone C', 'Warehouse', 'Medium', 'Finished goods and outbound staging'),
('area-storage-b', 'Storage Room B', 'Warehouse', 'High', 'Temperature sensitive storage room');

INSERT INTO cameras (camera_code, name, area_id, status) VALUES
('CAM-01', 'Robot Cell 2', 'area-robot', 'Active'),
('CAM-02', 'Warehouse Zone C', 'area-warehouse-c', 'Active'),
('CAM-03', 'Line C', 'area-final', 'Active'),
('CAM-04', 'Storage Room B', 'area-storage-b', 'Active');

INSERT INTO app_settings (setting_key, setting_value, value_type, category, description, updated_at) VALUES
('camera.auto_alert_confidence', '0.80', 'number', 'Camera', 'Minimum confidence (0-1) for a detection to auto-raise a safety alert.', '2026-07-21T00:00:00'),
('camera.allowed_severities', 'Low,Medium,High,Critical', 'list', 'Camera', 'Severity values accepted by camera detection input.', '2026-07-21T00:00:00'),
('camera.alert_severities', 'High,Critical', 'list', 'Camera', 'Severities that (with enough confidence) auto-raise a safety alert.', '2026-07-21T00:00:00'),
('warehouse.low_stock_threshold', '100', 'number', 'Warehouse', 'Item quantity at or below which status becomes Low Stock.', '2026-07-21T00:00:00'),
('warehouse.zone_warning_ratio', '0.95', 'number', 'Warehouse', 'Zone usage ratio (0-1) at or above which zone status becomes Warning.', '2026-07-21T00:00:00'),
('warehouse.zone_near_capacity_ratio', '0.85', 'number', 'Warehouse', 'Zone usage ratio (0-1) at or above which zone status becomes Near Capacity.', '2026-07-21T00:00:00'),
('dashboard.target_completion_warning', '90', 'number', 'Dashboard', 'Target completion percent below which the KPI shows a warning.', '2026-07-21T00:00:00'),
('forms.default_requester', 'user-005', 'string', 'Forms', 'User id used as requester when a form is created without one.', '2026-07-21T00:00:00'),
('forms.approver.default', 'user-001', 'string', 'Forms', 'Default approver (leave/overtime/other) - Factory Manager.', '2026-07-21T00:00:00'),
('forms.approver.machine', 'user-002', 'string', 'Forms', 'Approver for machine-related forms - Production Manager.', '2026-07-21T00:00:00'),
('forms.approver.warehouse', 'user-003', 'string', 'Forms', 'Approver for warehouse/import/export forms - Warehouse Manager.', '2026-07-21T00:00:00');

INSERT INTO production_lines (id, name, area_id, status, target_output, actual_output, efficiency, defect_rate, downtime_minutes, assigned_workers, issue) VALUES
('line-a', 'Line A', 'area-assembly', 'Normal', 2200, 2140, 97, 1.8, 4, 24, 'Running normally'),
('line-b', 'Line B', 'area-packaging', 'Normal', 1900, 1810, 94, 2.1, 9, 19, 'Minor output gap'),
('line-c', 'Line C', 'area-final', 'Warning', 2000, 1560, 78, 4.6, 18, 17, 'Likely staffing shortage'),
('line-d', 'Line D', 'area-press', 'Stopped', 1600, 860, 54, 7.2, 48, 12, 'Press Unit P-14 vibration');

INSERT INTO production_records (id, line_id, record_time, target_output, actual_output, defect_count, downtime_minutes) VALUES
('prod-rec-001', 'line-a', '2026-07-03T08:00:00', 550, 540, 8, 0),
('prod-rec-002', 'line-a', '2026-07-03T09:00:00', 550, 535, 7, 4),
('prod-rec-003', 'line-c', '2026-07-03T08:00:00', 500, 390, 18, 8),
('prod-rec-004', 'line-c', '2026-07-03T09:00:00', 500, 360, 21, 10),
('prod-rec-005', 'line-d', '2026-07-03T08:00:00', 400, 240, 25, 22),
('prod-rec-006', 'line-d', '2026-07-03T09:00:00', 400, 120, 33, 26);

INSERT INTO machines (id, machine_code, name, line_id, status, last_maintenance_at) VALUES
('machine-001', 'ASM-11', 'Assembly Conveyor 11', 'line-a', 'Running', '2026-06-28T15:00:00'),
('machine-002', 'PKG-07', 'Packaging Unit 07', 'line-b', 'Running', '2026-06-29T10:30:00'),
('machine-003', 'FIN-03', 'Final Assembly Bench 03', 'line-c', 'Running', '2026-06-30T14:20:00'),
('machine-004', 'P-14', 'Press Unit P-14', 'line-d', 'Warning', '2026-06-25T09:10:00');

INSERT INTO machine_issues (id, machine_id, line_id, severity, status, description, reported_by, reported_at) VALUES
('issue-001', 'machine-004', 'line-d', 'High', 'Submitted', 'Abnormal vibration detected on Press Unit P-14.', 'user-005', '2026-07-03T09:58:00');

INSERT INTO warehouse_zones (id, name, zone_type, capacity, current_usage, status) VALUES
('zone-a', 'Raw Material Zone A', 'Raw Material', 1000, 720, 'Available'),
('zone-c', 'Finished Goods Zone C', 'Finished Goods', 1000, 920, 'Near Capacity'),
('zone-p', 'Packaging Zone P', 'Packaging', 800, 510, 'Available'),
('zone-b', 'Storage Room B', 'Temperature Sensitive', 600, 440, 'Warning');

INSERT INTO business_units (id, bu_code, bu_name, status) VALUES
('bu-ryobi', 'Ryobi', 'Ryobi', 'Active'),
('bu-aes', 'AES', 'AES', 'Active'),
('bu-mil', 'MIL', 'MIL', 'Active');

INSERT INTO io_masters (id, io_id, io_code, bu_id, status) VALUES
('io-4229', '4229', 'TC2', 'bu-ryobi', 'Active'),
('io-4070', '4070', 'TC5', 'bu-ryobi', 'Active'),
('io-4506', '4506', 'TN2', 'bu-ryobi', 'Active'),
('io-3615', '3615', 'TN5', 'bu-ryobi', 'Active'),
('io-4073', '4073', 'TCB', 'bu-aes', 'Active'),
('io-4530', '4530', 'TD3', 'bu-mil', 'Active'),
('io-4327', '4327', 'TH3', 'bu-mil', 'Active'),
('io-4226', '4226', 'TP7', 'bu-mil', 'Active');

INSERT INTO warehouse_items (id, io_id, io_code, bu, item_code, item_name, batch_code, category, quantity, unit, zone_id, shelf, status, last_movement_at) VALUES
('item-001', '4229', 'TC2', 'Ryobi', 'TC2-RM-204', 'Ryobi TC2 Aluminum Frame', 'B-7782', 'Raw Material', 480, 'pcs', 'zone-c', 'C-18', 'Wrong Zone', '2026-07-03T09:12:00'),
('item-002', '4070', 'TC5', 'Ryobi', 'TC5-FK-18', 'Ryobi TC5 Fastener Kit', 'B-2221', 'Component', 56, 'kits', 'zone-a', 'A-04', 'Low Stock', '2026-07-03T08:26:00'),
('item-003', '4506', 'TN2', 'Ryobi', 'TN2-FG-887', 'Ryobi TN2 Completed Control Box', 'B-9920', 'Finished Goods', 320, 'pcs', 'zone-c', 'C-02', 'Correct', '2026-07-03T10:05:00'),
('item-004', '3615', 'TN5', 'Ryobi', 'TN5-PK-441', 'Ryobi TN5 Packaging Foam', 'B-3391', 'Packaging', 900, 'sheets', 'zone-p', 'P-11', 'Correct', '2026-07-03T07:44:00'),
('item-005', '4073', 'TCB', 'AES', 'TCB-CB-120', 'AES TCB Control Board', 'B-4440', 'Component', 180, 'pcs', 'zone-a', 'A-12', 'Correct', '2026-07-03T08:52:00'),
('item-006', '4530', 'TD3', 'MIL', 'TD3-HD-330', 'MIL TD3 Housing Door', 'B-5530', 'Raw Material', 260, 'pcs', 'zone-a', 'A-21', 'Correct', '2026-07-03T09:31:00'),
('item-007', '4327', 'TH3', 'MIL', 'TH3-LB-210', 'MIL TH3 Label Set', 'B-6727', 'Packaging', 74, 'sets', 'zone-p', 'P-04', 'Low Stock', '2026-07-03T09:48:00'),
('item-008', '4226', 'TP7', 'MIL', 'TP7-FG-510', 'MIL TP7 Finished Assembly', 'B-7226', 'Finished Goods', 145, 'pcs', 'zone-c', 'C-09', 'Correct', '2026-07-03T10:18:00');

INSERT INTO warehouse_item_io_links (item_id, io_master_id, link_type, effective_from, effective_to) VALUES
('item-001', 'io-4229', 'Primary', '2026-07-03T08:00:00', NULL),
('item-002', 'io-4070', 'Primary', '2026-07-03T08:00:00', NULL),
('item-003', 'io-4506', 'Primary', '2026-07-03T08:00:00', NULL),
('item-004', 'io-3615', 'Primary', '2026-07-03T08:00:00', NULL),
('item-005', 'io-4073', 'Primary', '2026-07-03T08:00:00', NULL),
('item-006', 'io-4530', 'Primary', '2026-07-03T08:00:00', NULL),
('item-007', 'io-4327', 'Primary', '2026-07-03T08:00:00', NULL),
('item-008', 'io-4226', 'Primary', '2026-07-03T08:00:00', NULL);

INSERT INTO goods_movements (id, item_id, from_zone_id, to_zone_id, quantity, movement_type, moved_by, moved_at, note) VALUES
('move-001', 'item-001', 'zone-a', 'zone-c', 480, 'Transfer', 'user-003', '2026-07-03T09:12:00', 'Detected as wrong placement for raw material batch'),
('move-002', 'item-003', NULL, 'zone-c', 320, 'Import', 'user-003', '2026-07-03T10:05:00', 'Finished goods intake from Line B');

INSERT INTO safety_alerts (id, title, alert_type, severity, area_id, status, detected_at, assigned_to, description, action_note) VALUES
('safe-001', 'Restricted zone entry', 'AI Camera', 'Critical', 'area-robot', 'New', '2026-07-03T09:42:00', 'user-004', 'Worker detected inside restricted area while Line B was active.', NULL),
('safe-002', 'Forklift congestion', 'Traffic', 'Medium', 'area-warehouse-c', 'In Review', '2026-07-03T10:15:00', 'user-004', 'Traffic density increased during inbound peak.', 'Monitoring traffic flow'),
('safe-003', 'Temperature variance', 'Sensor', 'High', 'area-storage-b', 'Escalated', '2026-07-03T10:31:00', 'user-004', 'Room temperature exceeded safe threshold for 6 minutes.', 'Escalated to operations manager');

INSERT INTO camera_events (id, camera_code, area_id, event_type, severity, confidence, event_time, related_alert_id) VALUES
('cam-event-001', 'CAM-01', 'area-robot', 'Restricted Zone Entry', 'Critical', 0.94, '2026-07-03T09:42:00', 'safe-001'),
('cam-event-002', 'CAM-02', 'area-warehouse-c', 'Traffic Congestion', 'Medium', 0.82, '2026-07-03T10:15:00', 'safe-002');

INSERT INTO safety_alert_camera_events (alert_id, camera_event_id, relation_type, linked_at) VALUES
('safe-001', 'cam-event-001', 'Primary Evidence', '2026-07-03T09:42:00'),
('safe-002', 'cam-event-002', 'Primary Evidence', '2026-07-03T10:15:00');

INSERT INTO employees (id, employee_code, full_name, department, skill_tags, availability_status, current_line_id) VALUES
('emp-001', 'E-1001', 'Nguyen Minh', 'Packaging', 'Packaging,Line B,Night Shift', 'Available', 'line-b'),
('emp-002', 'E-1002', 'Tran Quoc', 'Production', 'Assembly,Line C,Machine Setup', 'Available', 'line-c'),
('emp-003', 'E-1003', 'Le Van', 'Production', 'Press,Maintenance Support', 'Busy', 'line-d'),
('emp-004', 'E-1004', 'Hoang Mai', 'Quality', 'Inspection,Final Assembly', 'Available', 'line-c'),
('emp-005', 'E-1005', 'Pham Nhi', 'Warehouse', 'Inventory,Forklift', 'Available', NULL),
('emp-006', 'E-1006', 'Vu Ha', 'Production', 'Assembly,Line C,Final Assembly', 'Available', 'line-c'),
('emp-007', 'E-1007', 'Do Nam', 'Production', 'Line C,Machine Setup', 'Available', NULL),
('emp-008', 'E-1008', 'Bui Thu', 'Quality', 'Inspection,Line C', 'Available', 'line-c'),
('emp-009', 'E-1009', 'Ngo Kim', 'Production', 'Press,Line D', 'Available', 'line-d'),
('emp-010', 'E-1010', 'Ly Phong', 'Production', 'Assembly,Line C', 'Available', NULL),
('emp-011', 'E-1011', 'Trinh Lan', 'Packaging', 'Packaging,Line B', 'Busy', 'line-b'),
('emp-012', 'E-1012', 'Cao Son', 'Production', 'Line C,Night Shift', 'Available', 'line-c');

INSERT INTO skills (id, skill_code, skill_name, skill_group) VALUES
('skill-packaging', 'PACKAGING', 'Packaging Operation', 'Production'),
('skill-assembly', 'ASSEMBLY', 'Assembly Operation', 'Production'),
('skill-press', 'PRESS', 'Press Operation', 'Machine'),
('skill-quality', 'QUALITY_CHECK', 'Quality Inspection', 'Quality'),
('skill-forklift', 'FORKLIFT', 'Forklift Operation', 'Warehouse');

INSERT INTO employee_skills (employee_id, skill_id, proficiency_level, certified_at, expires_at) VALUES
('emp-001', 'skill-packaging', 'Advanced', '2026-01-10T08:00:00', '2027-01-10T08:00:00'),
('emp-002', 'skill-assembly', 'Advanced', '2026-02-12T08:00:00', '2027-02-12T08:00:00'),
('emp-003', 'skill-press', 'Expert', '2026-01-20T08:00:00', '2027-01-20T08:00:00'),
('emp-004', 'skill-quality', 'Advanced', '2026-03-04T08:00:00', '2027-03-04T08:00:00'),
('emp-005', 'skill-forklift', 'Certified', '2026-02-02T08:00:00', '2027-02-02T08:00:00');

INSERT INTO shift_plans (id, shift_date, shift_name, line_id, required_workers, assigned_workers, overtime_hours, status) VALUES
('shift-001', '2026-07-03', 'Morning', 'line-a', 24, 24, 0, 'Published'),
('shift-002', '2026-07-03', 'Afternoon', 'line-c', 21, 18, 1, 'Recommended'),
('shift-003', '2026-07-03', 'Night', 'line-c', 20, 18, 1.5, 'Recommended');

INSERT INTO shift_plan_assignments (shift_id, employee_id, assignment_role, assignment_status, assigned_at) VALUES
('shift-001', 'emp-002', 'Operator', 'Assigned', '2026-07-03T08:00:00'),
('shift-002', 'emp-004', 'Quality Inspector', 'Assigned', '2026-07-03T10:00:00'),
('shift-003', 'emp-001', 'Packaging Operator', 'Recommended', '2026-07-03T10:15:00');

INSERT INTO form_requests (id, form_type, requester_id, approver_id, status, submitted_at, approved_at, summary, rejection_reason) VALUES
('form-001', 'Overtime Request', 'user-005', 'user-001', 'Pending Approval', '2026-07-03T10:22:00', NULL, '1.5 overtime hours requested for night shift recovery.', NULL),
('form-002', 'Machine Issue Report', 'user-005', 'user-002', 'Pending Approval', '2026-07-03T09:58:00', NULL, 'Press Unit P-14 vibration detected on Line D.', NULL),
('form-003', 'Warehouse Export', 'user-003', 'user-001', 'Approved', '2026-07-03T09:35:00', '2026-07-03T09:50:00', 'Export request for batch FG-887.', NULL);

INSERT INTO form_approval_steps (id, form_id, step_order, approver_id, status, action_at, note) VALUES
('approval-001', 'form-001', 1, 'user-001', 'Pending', NULL, 'Waiting for factory manager approval'),
('approval-002', 'form-002', 1, 'user-002', 'Pending', NULL, 'Waiting for production manager review'),
('approval-003', 'form-003', 1, 'user-001', 'Approved', '2026-07-03T09:50:00', 'Approved for outbound staging');

INSERT INTO notifications (id, title, notification_type, severity, status, target_user_id, related_entity_type, related_entity_id, created_at) VALUES
('noti-001', 'Critical restricted-zone entry', 'Safety', 'Critical', 'Unread', 'user-004', 'SafetyAlert', 'safe-001', '2026-07-03T09:42:00'),
('noti-002', 'Line C below target efficiency', 'Production', 'High', 'Unread', 'user-002', 'ProductionLine', 'line-c', '2026-07-03T10:03:00'),
('noti-003', 'Overtime request pending', 'Forms', 'Medium', 'Read', 'user-001', 'FormRequest', 'form-001', '2026-07-03T10:22:00'),
('noti-004', 'Warehouse wrong placement alert', 'Warehouse', 'High', 'Unread', 'user-003', 'WarehouseItem', 'item-001', '2026-07-03T09:12:00');

INSERT INTO notification_links (notification_id, entity_type, entity_id, link_role) VALUES
('noti-001', 'SafetyAlert', 'safe-001', 'Primary'),
('noti-002', 'ProductionLine', 'line-c', 'Primary'),
('noti-003', 'FormRequest', 'form-001', 'Primary'),
('noti-004', 'WarehouseItem', 'item-001', 'Primary');

INSERT INTO ai_recommendations (id, module, title, reason, expected_impact, status, created_at, reviewed_by) VALUES
('ai-001', 'Workforce', 'Move 2 certified operators to Line C', 'Line C is below efficiency threshold and has a 3-worker gap.', 'Recover around 320 units before end of day.', 'New', '2026-07-03T10:10:00', NULL),
('ai-002', 'Workforce', 'Approve 1.5 hours overtime', 'Daily target is behind by 600 units.', 'Reduce target gap by approximately 50%.', 'Reviewed', '2026-07-03T10:15:00', 'user-001'),
('ai-003', 'Production', 'Inspect Press Unit P-14', 'Line D stopped after abnormal vibration report.', 'Prevent repeated downtime and safety risk.', 'New', '2026-07-03T10:20:00', NULL),
('ai-004', 'Warehouse', 'Move RM-204 to Raw Material Zone A', 'Raw material batch is stored in finished goods zone.', 'Resolve wrong placement and reduce search risk.', 'New', '2026-07-03T09:20:00', NULL);

INSERT INTO ai_recommendation_links (recommendation_id, entity_type, entity_id, link_reason) VALUES
('ai-001', 'ProductionLine', 'line-c', 'Line C has a worker gap'),
('ai-001', 'ShiftPlan', 'shift-003', 'Night shift requires coverage'),
('ai-003', 'Machine', 'machine-004', 'Machine vibration created downtime risk'),
('ai-004', 'WarehouseItem', 'item-001', 'Item is in wrong warehouse zone');

INSERT INTO reports (id, report_type, title, period_start, period_end, summary, created_by, created_at) VALUES
('report-001', 'Production', 'Daily Production Summary', '2026-07-03T00:00:00', '2026-07-03T23:59:59', 'Production is 84% complete with Line C and Line D requiring action.', 'user-001', '2026-07-03T11:00:00'),
('report-002', 'Safety', 'Safety Incident Summary', '2026-07-03T00:00:00', '2026-07-03T23:59:59', 'One critical restricted-zone entry and two active safety issues detected.', 'user-004', '2026-07-03T11:00:00'),
('report-003', 'Warehouse', 'Warehouse Capacity Summary', '2026-07-03T00:00:00', '2026-07-03T23:59:59', 'Warehouse Zone C is at 92% capacity and has one wrong placement issue.', 'user-003', '2026-07-03T11:00:00');

INSERT INTO report_source_links (report_id, source_type, source_id, included_at) VALUES
('report-001', 'ProductionLine', 'line-c', '2026-07-03T11:00:00'),
('report-001', 'ProductionLine', 'line-d', '2026-07-03T11:00:00'),
('report-002', 'SafetyAlert', 'safe-001', '2026-07-03T11:00:00'),
('report-003', 'WarehouseItem', 'item-001', '2026-07-03T11:00:00');