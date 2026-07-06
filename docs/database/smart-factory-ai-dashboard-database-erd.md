# Smart Factory AI Dashboard Database ERD

This document contains the full database relationship diagram for the Smart Factory AI Dashboard. It includes primary keys, foreign keys, and the main references used by the web modules.

For a more visual HTML version, open:

```text
docs/database/smart-factory-ai-dashboard-database-erd.html
```

## Full ERD

```mermaid
erDiagram
    roles {
        TEXT id PK
        TEXT name UK
        TEXT description
    }

    users {
        TEXT id PK
        TEXT full_name
        TEXT email UK
        TEXT role_id FK
        TEXT department
        TEXT status
        TEXT created_at
    }

    permissions {
        TEXT id PK
        TEXT permission_code UK
        TEXT module
        TEXT action
        TEXT description
    }

    role_permissions {
        TEXT role_id PK,FK
        TEXT permission_id PK,FK
        TEXT granted_at
    }

    factory_areas {
        TEXT id PK
        TEXT name
        TEXT area_type
        TEXT risk_level
        TEXT description
    }

    production_lines {
        TEXT id PK
        TEXT name
        TEXT area_id FK
        TEXT status
        INTEGER target_output
        INTEGER actual_output
        INTEGER efficiency
        REAL defect_rate
        INTEGER downtime_minutes
        INTEGER assigned_workers
        TEXT issue
    }

    production_records {
        TEXT id PK
        TEXT line_id FK
        TEXT record_time
        INTEGER target_output
        INTEGER actual_output
        INTEGER defect_count
        INTEGER downtime_minutes
    }

    machines {
        TEXT id PK
        TEXT machine_code UK
        TEXT name
        TEXT line_id FK
        TEXT status
        TEXT last_maintenance_at
    }

    machine_issues {
        TEXT id PK
        TEXT machine_id FK
        TEXT line_id FK
        TEXT severity
        TEXT status
        TEXT description
        TEXT reported_by FK
        TEXT reported_at
    }

    warehouse_zones {
        TEXT id PK
        TEXT name
        TEXT zone_type
        INTEGER capacity
        INTEGER current_usage
        TEXT status
    }

    business_units {
        TEXT id PK
        TEXT bu_code UK
        TEXT bu_name
        TEXT status
    }

    io_masters {
        TEXT id PK
        TEXT io_id UK
        TEXT io_code UK
        TEXT bu_id FK
        TEXT status
    }

    warehouse_items {
        TEXT id PK
        TEXT io_id
        TEXT io_code
        TEXT bu
        TEXT item_code UK
        TEXT item_name
        TEXT batch_code
        TEXT category
        INTEGER quantity
        TEXT unit
        TEXT zone_id FK
        TEXT shelf
        TEXT status
        TEXT last_movement_at
    }

    warehouse_item_io_links {
        TEXT item_id PK,FK
        TEXT io_master_id PK,FK
        TEXT link_type
        TEXT effective_from
        TEXT effective_to NULL
    }

    goods_movements {
        TEXT id PK
        TEXT item_id FK
        TEXT from_zone_id FK_NULL
        TEXT to_zone_id FK
        INTEGER quantity
        TEXT movement_type
        TEXT moved_by FK
        TEXT moved_at
        TEXT note
    }

    safety_alerts {
        TEXT id PK
        TEXT title
        TEXT alert_type
        TEXT severity
        TEXT area_id FK
        TEXT status
        TEXT detected_at
        TEXT assigned_to FK
        TEXT description
        TEXT action_note NULL
    }

    camera_events {
        TEXT id PK
        TEXT camera_code
        TEXT area_id FK
        TEXT event_type
        TEXT severity
        REAL confidence
        TEXT event_time
        TEXT related_alert_id FK_NULL
    }

    safety_alert_camera_events {
        TEXT alert_id PK,FK
        TEXT camera_event_id PK,FK
        TEXT relation_type
        TEXT linked_at
    }

    employees {
        TEXT id PK
        TEXT employee_code UK
        TEXT full_name
        TEXT department
        TEXT skill_tags
        TEXT availability_status
        TEXT current_line_id FK_NULL
    }

    skills {
        TEXT id PK
        TEXT skill_code UK
        TEXT skill_name
        TEXT skill_group
    }

    employee_skills {
        TEXT employee_id PK,FK
        TEXT skill_id PK,FK
        TEXT proficiency_level
        TEXT certified_at NULL
        TEXT expires_at NULL
    }

    shift_plans {
        TEXT id PK
        TEXT shift_date
        TEXT shift_name
        TEXT line_id FK
        INTEGER required_workers
        INTEGER assigned_workers
        REAL overtime_hours
        TEXT status
    }

    shift_plan_assignments {
        TEXT shift_id PK,FK
        TEXT employee_id PK,FK
        TEXT assignment_role
        TEXT assignment_status
        TEXT assigned_at
    }

    form_requests {
        TEXT id PK
        TEXT form_type
        TEXT requester_id FK
        TEXT approver_id FK
        TEXT status
        TEXT submitted_at
        TEXT approved_at NULL
        TEXT summary
        TEXT rejection_reason NULL
    }

    form_approval_steps {
        TEXT id PK
        TEXT form_id FK
        INTEGER step_order
        TEXT approver_id FK
        TEXT status
        TEXT action_at NULL
        TEXT note NULL
    }

    notifications {
        TEXT id PK
        TEXT title
        TEXT notification_type
        TEXT severity
        TEXT status
        TEXT target_user_id FK
        TEXT related_entity_type
        TEXT related_entity_id
        TEXT created_at
    }

    notification_links {
        TEXT notification_id PK,FK
        TEXT entity_type PK
        TEXT entity_id PK
        TEXT link_role
    }

    ai_recommendations {
        TEXT id PK
        TEXT module
        TEXT title
        TEXT reason
        TEXT expected_impact
        TEXT status
        TEXT created_at
        TEXT reviewed_by FK_NULL
    }

    ai_recommendation_links {
        TEXT recommendation_id PK,FK
        TEXT entity_type PK
        TEXT entity_id PK
        TEXT link_reason
    }

    reports {
        TEXT id PK
        TEXT report_type
        TEXT title
        TEXT period_start
        TEXT period_end
        TEXT summary
        TEXT created_by FK
        TEXT created_at
    }

    report_source_links {
        TEXT report_id PK,FK
        TEXT source_type PK
        TEXT source_id PK
        TEXT included_at
    }

    roles ||--o{ users : "users.role_id -> roles.id"
    roles ||--o{ role_permissions : "role_permissions.role_id -> roles.id"
    permissions ||--o{ role_permissions : "role_permissions.permission_id -> permissions.id"

    factory_areas ||--o{ production_lines : "production_lines.area_id -> factory_areas.id"
    production_lines ||--o{ production_records : "production_records.line_id -> production_lines.id"
    production_lines ||--o{ machines : "machines.line_id -> production_lines.id"
    machines ||--o{ machine_issues : "machine_issues.machine_id -> machines.id"
    production_lines ||--o{ machine_issues : "machine_issues.line_id -> production_lines.id"
    users ||--o{ machine_issues : "machine_issues.reported_by -> users.id"

    warehouse_zones ||--o{ warehouse_items : "warehouse_items.zone_id -> warehouse_zones.id"
    business_units ||--o{ io_masters : "io_masters.bu_id -> business_units.id"
    io_masters ||--o{ warehouse_item_io_links : "warehouse_item_io_links.io_master_id -> io_masters.id"
    warehouse_items ||--o{ warehouse_item_io_links : "warehouse_item_io_links.item_id -> warehouse_items.id"
    warehouse_items ||--o{ goods_movements : "goods_movements.item_id -> warehouse_items.id"
    warehouse_zones ||--o{ goods_movements : "goods_movements.from_zone_id -> warehouse_zones.id"
    warehouse_zones ||--o{ goods_movements : "goods_movements.to_zone_id -> warehouse_zones.id"
    users ||--o{ goods_movements : "goods_movements.moved_by -> users.id"

    factory_areas ||--o{ safety_alerts : "safety_alerts.area_id -> factory_areas.id"
    users ||--o{ safety_alerts : "safety_alerts.assigned_to -> users.id"
    factory_areas ||--o{ camera_events : "camera_events.area_id -> factory_areas.id"
    safety_alerts ||--o{ camera_events : "camera_events.related_alert_id -> safety_alerts.id"
    safety_alerts ||--o{ safety_alert_camera_events : "safety_alert_camera_events.alert_id -> safety_alerts.id"
    camera_events ||--o{ safety_alert_camera_events : "safety_alert_camera_events.camera_event_id -> camera_events.id"

    production_lines ||--o{ employees : "employees.current_line_id -> production_lines.id"
    production_lines ||--o{ shift_plans : "shift_plans.line_id -> production_lines.id"
    employees ||--o{ employee_skills : "employee_skills.employee_id -> employees.id"
    skills ||--o{ employee_skills : "employee_skills.skill_id -> skills.id"
    shift_plans ||--o{ shift_plan_assignments : "shift_plan_assignments.shift_id -> shift_plans.id"
    employees ||--o{ shift_plan_assignments : "shift_plan_assignments.employee_id -> employees.id"

    users ||--o{ form_requests : "form_requests.requester_id -> users.id"
    users ||--o{ form_requests : "form_requests.approver_id -> users.id"
    form_requests ||--o{ form_approval_steps : "form_approval_steps.form_id -> form_requests.id"
    users ||--o{ form_approval_steps : "form_approval_steps.approver_id -> users.id"

    users ||--o{ notifications : "notifications.target_user_id -> users.id"
    notifications ||--o{ notification_links : "notification_links.notification_id -> notifications.id"
    users ||--o{ ai_recommendations : "ai_recommendations.reviewed_by -> users.id"
    ai_recommendations ||--o{ ai_recommendation_links : "ai_recommendation_links.recommendation_id -> ai_recommendations.id"
    users ||--o{ reports : "reports.created_by -> users.id"
    reports ||--o{ report_source_links : "report_source_links.report_id -> reports.id"
```

## Reference Notes

### Explicit Foreign Keys

| Table | Foreign Key | References |
|---|---|---|
| `users` | `role_id` | `roles.id` |
| `role_permissions` | `role_id` | `roles.id` |
| `role_permissions` | `permission_id` | `permissions.id` |
| `production_lines` | `area_id` | `factory_areas.id` |
| `production_records` | `line_id` | `production_lines.id` |
| `machines` | `line_id` | `production_lines.id` |
| `machine_issues` | `machine_id` | `machines.id` |
| `machine_issues` | `line_id` | `production_lines.id` |
| `machine_issues` | `reported_by` | `users.id` |
| `warehouse_items` | `zone_id` | `warehouse_zones.id` |
| `io_masters` | `bu_id` | `business_units.id` |
| `warehouse_item_io_links` | `item_id` | `warehouse_items.id` |
| `warehouse_item_io_links` | `io_master_id` | `io_masters.id` |
| `goods_movements` | `item_id` | `warehouse_items.id` |
| `goods_movements` | `from_zone_id` | `warehouse_zones.id` nullable |
| `goods_movements` | `to_zone_id` | `warehouse_zones.id` |
| `goods_movements` | `moved_by` | `users.id` |
| `safety_alerts` | `area_id` | `factory_areas.id` |
| `safety_alerts` | `assigned_to` | `users.id` |
| `camera_events` | `area_id` | `factory_areas.id` |
| `camera_events` | `related_alert_id` | `safety_alerts.id` nullable |
| `safety_alert_camera_events` | `alert_id` | `safety_alerts.id` |
| `safety_alert_camera_events` | `camera_event_id` | `camera_events.id` |
| `employees` | `current_line_id` | `production_lines.id` nullable |
| `employee_skills` | `employee_id` | `employees.id` |
| `employee_skills` | `skill_id` | `skills.id` |
| `shift_plans` | `line_id` | `production_lines.id` |
| `shift_plan_assignments` | `shift_id` | `shift_plans.id` |
| `shift_plan_assignments` | `employee_id` | `employees.id` |
| `form_requests` | `requester_id` | `users.id` |
| `form_requests` | `approver_id` | `users.id` |
| `form_approval_steps` | `form_id` | `form_requests.id` |
| `form_approval_steps` | `approver_id` | `users.id` |
| `notifications` | `target_user_id` | `users.id` |
| `notification_links` | `notification_id` | `notifications.id` |
| `ai_recommendations` | `reviewed_by` | `users.id` nullable |
| `ai_recommendation_links` | `recommendation_id` | `ai_recommendations.id` |
| `reports` | `created_by` | `users.id` |
| `report_source_links` | `report_id` | `reports.id` |

### Logical References

`notifications.related_entity_type` and `notifications.related_entity_id` are logical references. They can point to different modules such as:

| Related Entity Type | Expected Table |
|---|---|
| `SafetyAlert` | `safety_alerts` |
| `ProductionLine` | `production_lines` |
| `FormRequest` | `form_requests` |
| `WarehouseItem` | `warehouse_items` |

This design is flexible for notifications, but it does not enforce a physical foreign key for `related_entity_id` in the MVP schema.

### Unique Keys

| Table | Unique Key |
|---|---|
| `roles` | `name` |
| `users` | `email` |
| `machines` | `machine_code` |
| `warehouse_items` | `item_code` |
| `employees` | `employee_code` |

## Module-Level Reading

### Dashboard

Reads from:

1. `production_lines`
2. `safety_alerts`
3. `warehouse_items`
4. `form_requests`
5. `ai_recommendations`

### Production

Reads from:

1. `production_lines`
2. `factory_areas`
3. `production_records`
4. `machines`
5. `machine_issues`

### Warehouse

Reads from:

1. `warehouse_items`
2. `warehouse_zones`
3. `goods_movements`

### Safety And Cameras

Reads from:

1. `safety_alerts`
2. `camera_events`
3. `factory_areas`
4. `users`

### Workforce

Reads from:

1. `employees`
2. `shift_plans`
3. `production_lines`
4. `ai_recommendations`

### Forms And Notifications

Reads from:

1. `form_requests`
2. `notifications`
3. `users`

### Reports

Reads from all module tables and stores generated snapshots in `reports`.