CREATE TABLE roles (
  id TEXT PRIMARY KEY,
  name TEXT NOT NULL UNIQUE,
  description TEXT NOT NULL
);

CREATE TABLE users (
  id TEXT PRIMARY KEY,
  full_name TEXT NOT NULL,
  email TEXT NOT NULL UNIQUE,
  role_id TEXT NOT NULL,
  department TEXT NOT NULL,
  status TEXT NOT NULL,
  created_at TEXT NOT NULL,
  FOREIGN KEY (role_id) REFERENCES roles(id)
);

CREATE TABLE permissions (
  id TEXT PRIMARY KEY,
  permission_code TEXT NOT NULL UNIQUE,
  module TEXT NOT NULL,
  action TEXT NOT NULL,
  description TEXT NOT NULL
);

CREATE TABLE role_permissions (
  role_id TEXT NOT NULL,
  permission_id TEXT NOT NULL,
  granted_at TEXT NOT NULL,
  PRIMARY KEY (role_id, permission_id),
  FOREIGN KEY (role_id) REFERENCES roles(id),
  FOREIGN KEY (permission_id) REFERENCES permissions(id)
);

CREATE TABLE factory_areas (
  id TEXT PRIMARY KEY,
  name TEXT NOT NULL,
  area_type TEXT NOT NULL,
  risk_level TEXT NOT NULL,
  description TEXT NOT NULL
);

CREATE TABLE production_lines (
  id TEXT PRIMARY KEY,
  name TEXT NOT NULL,
  area_id TEXT NOT NULL,
  status TEXT NOT NULL,
  target_output INTEGER NOT NULL,
  actual_output INTEGER NOT NULL,
  efficiency INTEGER NOT NULL,
  defect_rate REAL NOT NULL,
  downtime_minutes INTEGER NOT NULL,
  assigned_workers INTEGER NOT NULL,
  issue TEXT NOT NULL,
  FOREIGN KEY (area_id) REFERENCES factory_areas(id)
);

CREATE TABLE production_records (
  id TEXT PRIMARY KEY,
  line_id TEXT NOT NULL,
  record_time TEXT NOT NULL,
  target_output INTEGER NOT NULL,
  actual_output INTEGER NOT NULL,
  defect_count INTEGER NOT NULL,
  downtime_minutes INTEGER NOT NULL,
  FOREIGN KEY (line_id) REFERENCES production_lines(id)
);

CREATE TABLE machines (
  id TEXT PRIMARY KEY,
  machine_code TEXT NOT NULL UNIQUE,
  name TEXT NOT NULL,
  line_id TEXT NOT NULL,
  status TEXT NOT NULL,
  last_maintenance_at TEXT NOT NULL,
  FOREIGN KEY (line_id) REFERENCES production_lines(id)
);

CREATE TABLE machine_issues (
  id TEXT PRIMARY KEY,
  machine_id TEXT NOT NULL,
  line_id TEXT NOT NULL,
  severity TEXT NOT NULL,
  status TEXT NOT NULL,
  description TEXT NOT NULL,
  reported_by TEXT NOT NULL,
  reported_at TEXT NOT NULL,
  FOREIGN KEY (machine_id) REFERENCES machines(id),
  FOREIGN KEY (line_id) REFERENCES production_lines(id),
  FOREIGN KEY (reported_by) REFERENCES users(id)
);

CREATE TABLE warehouse_zones (
  id TEXT PRIMARY KEY,
  name TEXT NOT NULL,
  zone_type TEXT NOT NULL,
  capacity INTEGER NOT NULL,
  current_usage INTEGER NOT NULL,
  status TEXT NOT NULL
);

CREATE TABLE business_units (
  id TEXT PRIMARY KEY,
  bu_code TEXT NOT NULL UNIQUE,
  bu_name TEXT NOT NULL,
  status TEXT NOT NULL
);

CREATE TABLE io_masters (
  id TEXT PRIMARY KEY,
  io_id TEXT NOT NULL UNIQUE,
  io_code TEXT NOT NULL UNIQUE,
  bu_id TEXT NOT NULL,
  status TEXT NOT NULL,
  FOREIGN KEY (bu_id) REFERENCES business_units(id)
);

CREATE TABLE warehouse_items (
  id TEXT PRIMARY KEY,
  io_id TEXT NOT NULL,
  io_code TEXT NOT NULL,
  bu TEXT NOT NULL,
  item_code TEXT NOT NULL UNIQUE,
  item_name TEXT NOT NULL,
  batch_code TEXT NOT NULL,
  category TEXT NOT NULL,
  quantity INTEGER NOT NULL,
  unit TEXT NOT NULL,
  zone_id TEXT NOT NULL,
  shelf TEXT NOT NULL,
  status TEXT NOT NULL,
  last_movement_at TEXT NOT NULL,
  FOREIGN KEY (zone_id) REFERENCES warehouse_zones(id)
);

CREATE TABLE warehouse_item_io_links (
  item_id TEXT NOT NULL,
  io_master_id TEXT NOT NULL,
  link_type TEXT NOT NULL,
  effective_from TEXT NOT NULL,
  effective_to TEXT,
  PRIMARY KEY (item_id, io_master_id),
  FOREIGN KEY (item_id) REFERENCES warehouse_items(id),
  FOREIGN KEY (io_master_id) REFERENCES io_masters(id)
);

CREATE TABLE goods_movements (
  id TEXT PRIMARY KEY,
  item_id TEXT NOT NULL,
  from_zone_id TEXT,
  to_zone_id TEXT NOT NULL,
  quantity INTEGER NOT NULL,
  movement_type TEXT NOT NULL,
  moved_by TEXT NOT NULL,
  moved_at TEXT NOT NULL,
  note TEXT NOT NULL,
  FOREIGN KEY (item_id) REFERENCES warehouse_items(id),
  FOREIGN KEY (from_zone_id) REFERENCES warehouse_zones(id),
  FOREIGN KEY (to_zone_id) REFERENCES warehouse_zones(id),
  FOREIGN KEY (moved_by) REFERENCES users(id)
);

CREATE TABLE safety_alerts (
  id TEXT PRIMARY KEY,
  title TEXT NOT NULL,
  alert_type TEXT NOT NULL,
  severity TEXT NOT NULL,
  area_id TEXT NOT NULL,
  status TEXT NOT NULL,
  detected_at TEXT NOT NULL,
  assigned_to TEXT NOT NULL,
  description TEXT NOT NULL,
  action_note TEXT,
  FOREIGN KEY (area_id) REFERENCES factory_areas(id),
  FOREIGN KEY (assigned_to) REFERENCES users(id)
);

CREATE TABLE camera_events (
  id TEXT PRIMARY KEY,
  camera_code TEXT NOT NULL,
  area_id TEXT NOT NULL,
  event_type TEXT NOT NULL,
  severity TEXT NOT NULL,
  confidence REAL NOT NULL,
  event_time TEXT NOT NULL,
  related_alert_id TEXT,
  FOREIGN KEY (area_id) REFERENCES factory_areas(id),
  FOREIGN KEY (related_alert_id) REFERENCES safety_alerts(id)
);

CREATE TABLE safety_alert_camera_events (
  alert_id TEXT NOT NULL,
  camera_event_id TEXT NOT NULL,
  relation_type TEXT NOT NULL,
  linked_at TEXT NOT NULL,
  PRIMARY KEY (alert_id, camera_event_id),
  FOREIGN KEY (alert_id) REFERENCES safety_alerts(id),
  FOREIGN KEY (camera_event_id) REFERENCES camera_events(id)
);

CREATE TABLE employees (
  id TEXT PRIMARY KEY,
  employee_code TEXT NOT NULL UNIQUE,
  full_name TEXT NOT NULL,
  department TEXT NOT NULL,
  skill_tags TEXT NOT NULL,
  availability_status TEXT NOT NULL,
  current_line_id TEXT,
  FOREIGN KEY (current_line_id) REFERENCES production_lines(id)
);

CREATE TABLE skills (
  id TEXT PRIMARY KEY,
  skill_code TEXT NOT NULL UNIQUE,
  skill_name TEXT NOT NULL,
  skill_group TEXT NOT NULL
);

CREATE TABLE employee_skills (
  employee_id TEXT NOT NULL,
  skill_id TEXT NOT NULL,
  proficiency_level TEXT NOT NULL,
  certified_at TEXT,
  expires_at TEXT,
  PRIMARY KEY (employee_id, skill_id),
  FOREIGN KEY (employee_id) REFERENCES employees(id),
  FOREIGN KEY (skill_id) REFERENCES skills(id)
);

CREATE TABLE shift_plans (
  id TEXT PRIMARY KEY,
  shift_date TEXT NOT NULL,
  shift_name TEXT NOT NULL,
  line_id TEXT NOT NULL,
  required_workers INTEGER NOT NULL,
  assigned_workers INTEGER NOT NULL,
  overtime_hours REAL NOT NULL,
  status TEXT NOT NULL,
  FOREIGN KEY (line_id) REFERENCES production_lines(id)
);

CREATE TABLE shift_plan_assignments (
  shift_id TEXT NOT NULL,
  employee_id TEXT NOT NULL,
  assignment_role TEXT NOT NULL,
  assignment_status TEXT NOT NULL,
  assigned_at TEXT NOT NULL,
  PRIMARY KEY (shift_id, employee_id),
  FOREIGN KEY (shift_id) REFERENCES shift_plans(id),
  FOREIGN KEY (employee_id) REFERENCES employees(id)
);

CREATE TABLE form_requests (
  id TEXT PRIMARY KEY,
  form_type TEXT NOT NULL,
  requester_id TEXT NOT NULL,
  approver_id TEXT NOT NULL,
  status TEXT NOT NULL,
  submitted_at TEXT NOT NULL,
  approved_at TEXT,
  summary TEXT NOT NULL,
  rejection_reason TEXT,
  FOREIGN KEY (requester_id) REFERENCES users(id),
  FOREIGN KEY (approver_id) REFERENCES users(id)
);

CREATE TABLE form_approval_steps (
  id TEXT PRIMARY KEY,
  form_id TEXT NOT NULL,
  step_order INTEGER NOT NULL,
  approver_id TEXT NOT NULL,
  status TEXT NOT NULL,
  action_at TEXT,
  note TEXT,
  FOREIGN KEY (form_id) REFERENCES form_requests(id),
  FOREIGN KEY (approver_id) REFERENCES users(id)
);

CREATE TABLE notifications (
  id TEXT PRIMARY KEY,
  title TEXT NOT NULL,
  notification_type TEXT NOT NULL,
  severity TEXT NOT NULL,
  status TEXT NOT NULL,
  target_user_id TEXT NOT NULL,
  related_entity_type TEXT NOT NULL,
  related_entity_id TEXT NOT NULL,
  created_at TEXT NOT NULL,
  FOREIGN KEY (target_user_id) REFERENCES users(id)
);

CREATE TABLE notification_links (
  notification_id TEXT NOT NULL,
  entity_type TEXT NOT NULL,
  entity_id TEXT NOT NULL,
  link_role TEXT NOT NULL,
  PRIMARY KEY (notification_id, entity_type, entity_id),
  FOREIGN KEY (notification_id) REFERENCES notifications(id)
);

CREATE TABLE ai_recommendations (
  id TEXT PRIMARY KEY,
  module TEXT NOT NULL,
  title TEXT NOT NULL,
  reason TEXT NOT NULL,
  expected_impact TEXT NOT NULL,
  status TEXT NOT NULL,
  created_at TEXT NOT NULL,
  reviewed_by TEXT,
  FOREIGN KEY (reviewed_by) REFERENCES users(id)
);

CREATE TABLE ai_recommendation_links (
  recommendation_id TEXT NOT NULL,
  entity_type TEXT NOT NULL,
  entity_id TEXT NOT NULL,
  link_reason TEXT NOT NULL,
  PRIMARY KEY (recommendation_id, entity_type, entity_id),
  FOREIGN KEY (recommendation_id) REFERENCES ai_recommendations(id)
);

CREATE TABLE reports (
  id TEXT PRIMARY KEY,
  report_type TEXT NOT NULL,
  title TEXT NOT NULL,
  period_start TEXT NOT NULL,
  period_end TEXT NOT NULL,
  summary TEXT NOT NULL,
  created_by TEXT NOT NULL,
  created_at TEXT NOT NULL,
  FOREIGN KEY (created_by) REFERENCES users(id)
);

CREATE TABLE report_source_links (
  report_id TEXT NOT NULL,
  source_type TEXT NOT NULL,
  source_id TEXT NOT NULL,
  included_at TEXT NOT NULL,
  PRIMARY KEY (report_id, source_type, source_id),
  FOREIGN KEY (report_id) REFERENCES reports(id)
);