# Smart Factory AI Dashboard Implementation Requirements

## 1. Document Overview

This document describes the detailed requirements for implementing the Smart Factory AI Dashboard web application. It should be used as the main reference before starting development.

The purpose of this document is to define:

1. Project scope.
2. Recommended technology stack.
3. User roles and permissions.
4. Required pages and modules.
5. Functional requirements.
6. Data requirements.
7. Suggested API structure.
8. UI and UX requirements.
9. MVP implementation scope.
10. Acceptance criteria.

## 2. Project Summary

Smart Factory AI Dashboard is a web-based platform for simulating an intelligent factory. The system helps users monitor production, manage warehouse operations, detect safety risks, analyze productivity, plan workforce allocation, automate internal forms, and generate reports.

The first version should focus on a realistic dashboard experience using simulated data. The system does not need to implement real AI models at the beginning. AI behavior can be simulated through rule-based recommendations and mock analytics.

## 3. Recommended Technology Stack

### 3.1 Frontend

Recommended stack:

1. React
2. Vite
3. TypeScript
4. React Router
5. CSS Modules or plain CSS initially
6. Recharts or Chart.js for charts

Reason:

React is suitable because the application contains many reusable UI parts such as KPI cards, tables, alerts, forms, filters, charts, and dashboard panels.

### 3.2 Backend

Recommended stack:

1. C#
2. ASP.NET Core MVC / Web API
3. Microsoft.Data.Sqlite for local SQLite access
4. SQLite for MVP/local demo, PostgreSQL for a more complete deployment

Reason:

C# and ASP.NET Core are suitable for the current environment, Visual Studio workflow, strongly typed API contracts, MVC controller organization, and future repository-based database access. SQLite gives the MVP a real local database without requiring a separate database server.

### 3.3 Initial Development Strategy

Recommended phases:

1. Phase 1: React frontend with mock data only.
2. Phase 2: Add routing and page-level modules.
3. Phase 3: Add ASP.NET Core MVC backend.
4. Phase 4: Add SQLite-first database persistence with JSON fallback.
5. Phase 5: Add AI recommendation logic and analytics services.

## 4. Proposed Project Structure

```text
smart-factory-ai-dashboard/
|
|-- frontend/
|   |-- src/
|   |   |-- main.tsx
|   |   |-- App.tsx
|   |   |-- routes/
|   |   |-- pages/
|   |   |-- components/
|   |   |-- layouts/
|   |   |-- data/
|   |   |-- services/
|   |   |-- types/
|   |   |-- styles/
|   |   |-- utils/
|   |
|   |-- public/
|   |-- package.json
|
|-- app/
|   |-- Legacy static HTML/CSS/JS prototype
|
|-- backend-dotnet/
|   |-- SmartFactoryDashboard.sln
|   |-- SmartFactory.Api.csproj
|   |-- Program.cs
|   |-- Controllers/
|   |-- Models/
|   |   |-- Database/
|   |   |-- Requests/
|   |-- Services/
|   |-- database/
|   |   |-- schema.sql
|   |   |-- seed.sql
|   |   |-- sample-data.json
|   |   |-- smart_factory_demo.db
|
|-- docs/
|   |-- overview/
|   |   |-- smart-factory-ai-dashboard-document.md
|   |-- sitemap/
|   |   |-- smart-factory-ai-dashboard-sitemap.md
|   |-- flows/
|   |   |-- smart-factory-ai-dashboard-user-flows.md
|   |-- requirements/
|   |   |-- smart-factory-ai-dashboard-requirements.md
|   |-- database/
|   |   |-- smart-factory-ai-dashboard-database-design.md
|   |   |-- smart-factory-ai-dashboard-database-erd.md
|   |   |-- smart-factory-ai-dashboard-database-erd.html
```

For the current prototype, `app/` can remain as a legacy static HTML, CSS, and JavaScript prototype. Active frontend work is in `frontend/`, and active backend work is in `backend-dotnet/`.

## 5. User Roles

### 5.1 Factory Manager

Permissions:

1. View all dashboard data.
2. View all production lines.
3. View and resolve safety alerts.
4. Approve shift plans and overtime.
5. Approve or reject forms.
6. View and export reports.
7. Review AI recommendations.

### 5.2 Production Manager

Permissions:

1. View production dashboard.
2. View line details.
3. Create or review machine issue reports.
4. Review productivity analytics.
5. Receive production alerts.
6. Request workforce support.

### 5.3 Warehouse Manager

Permissions:

1. View warehouse status.
2. Search and track items.
3. Review AI storage suggestions.
4. Approve warehouse import/export requests.
5. Resolve wrong placement alerts.
6. View warehouse reports.

### 5.4 Safety Officer

Permissions:

1. View AI camera alerts.
2. View factory risk map.
3. Record safety incidents.
4. Resolve or escalate safety alerts.
5. View safety reports.

### 5.5 Employee

Permissions:

1. View assigned shift and work line.
2. Submit leave request forms.
3. Submit overtime request forms.
4. Submit machine issue reports.
5. Receive notifications.

## 6. Required Pages

### 6.1 Authentication Pages

Required pages:

1. Login
2. Forgot Password
3. Reset Password

MVP requirement:

Only Login is required. Authentication can be simulated with a role selector.

### 6.2 Main Application Pages

Required pages:

1. Dashboard
2. Warehouse
3. Production
4. AI Cameras
5. Safety
6. Analytics
7. Workforce
8. Forms
9. Notifications
10. Reports
11. Settings

MVP requirement:

The first implementation should include Dashboard, Warehouse, Production, Safety, Workforce, Forms, and Reports.

## 7. Module Requirements

## 7.1 Dashboard Module

### 7.1.1 Purpose

The Dashboard is the main control center. It should summarize factory performance and guide users to urgent issues.

### 7.1.2 Required UI Sections

1. Topbar with search, alert count, and user profile.
2. Sidebar navigation.
3. Factory summary panel.
4. KPI cards.
5. Production line status panel.
6. Safety alert panel.
7. Warehouse signal panel.
8. Workforce planning panel.
9. Pending forms panel.
10. Reports summary panel.

### 7.1.3 Required KPI Cards

1. Daily output.
2. Target completion percentage.
3. Active production lines.
4. Safety alerts.
5. Warehouse occupancy.
6. Workforce coverage.

### 7.1.4 Functional Requirements

1. User can view real-time or simulated factory metrics.
2. User can click a KPI or alert to open related module detail.
3. User can see priority issues first.
4. Dashboard should update when mock data changes.
5. Dashboard should display different widgets based on user role in later versions.

### 7.1.5 Acceptance Criteria

1. Dashboard loads without errors.
2. All KPI cards show values and labels.
3. Alerts are clearly visible and prioritized.
4. Navigation to related modules is possible.

## 7.2 Warehouse Module

### 7.2.1 Purpose

The Warehouse module helps users track goods, manage inventory, and simulate AI-based storage recommendations.

### 7.2.2 Required UI Sections

1. Warehouse overview cards.
2. Item search box.
3. Warehouse zone list.
4. Item location table.
5. Storage recommendation panel.
6. Wrong placement alert list.
7. Goods movement history.

### 7.2.3 Functional Requirements

1. User can search for an item by item code, batch code, or product name.
2. System shows item zone, shelf, quantity, and last movement time.
3. System shows low-stock and over-capacity alerts.
4. System shows AI storage suggestions.
5. User can accept or override a storage suggestion.
6. System records goods movement history.
7. User can mark wrong placement alerts as resolved or false alerts.

### 7.2.4 Acceptance Criteria

1. Item search returns matching mock data.
2. Wrong placement alerts are visible.
3. Storage suggestions include reason and recommended zone.
4. User action changes the alert or recommendation status.

## 7.3 Production Module

### 7.3.1 Purpose

The Production module monitors all production lines and helps managers identify delays, downtime, and defect issues.

### 7.3.2 Required UI Sections

1. Line overview table.
2. Line status filters.
3. Line detail view.
4. Output vs target chart.
5. Downtime summary.
6. Defect rate summary.
7. Machine issue panel.
8. AI root-cause suggestion panel.

### 7.3.3 Functional Requirements

1. User can view all production lines.
2. User can filter lines by status.
3. User can open a line detail page.
4. System shows output, target, efficiency, downtime, defect count, and assigned workers.
5. System creates warning state if efficiency is below threshold.
6. User can create or review a machine issue report.
7. System shows possible causes for low performance.

### 7.3.4 Acceptance Criteria

1. Production line list shows at least normal, warning, and stopped states.
2. Line detail shows all required metrics.
3. Low-performance lines are visually highlighted.
4. User can move from dashboard to line detail.

## 7.4 AI Camera Module

### 7.4.1 Purpose

The AI Camera module simulates camera-based monitoring and event detection.

### 7.4.2 Required UI Sections

1. Camera grid.
2. Camera detail panel.
3. Incident detection log.
4. Restricted zone alerts.
5. Obstacle detection alerts.
6. Alert severity labels.

### 7.4.3 Functional Requirements

1. User can view simulated camera feeds or camera cards.
2. System displays detected events.
3. System labels event severity.
4. User can open incident details.
5. User can mark events as resolved, escalated, or false alerts.

### 7.4.4 Acceptance Criteria

1. Camera grid displays multiple factory areas.
2. Incident log shows event type, location, time, and severity.
3. User can update incident status.

## 7.5 Safety Module

### 7.5.1 Purpose

The Safety module helps safety officers monitor dangerous areas and manage incident response.

### 7.5.2 Required UI Sections

1. Safety overview cards.
2. Factory risk map.
3. Active safety alerts.
4. Incident detail panel.
5. High-risk area report.
6. Early warning suggestions.

### 7.5.3 Functional Requirements

1. User can view safety alerts by severity.
2. User can open alert details.
3. User can record response notes.
4. User can close, escalate, or mark an alert as false.
5. System updates safety statistics after alert closure.
6. System shows repeated high-risk areas.

### 7.5.4 Acceptance Criteria

1. Critical alerts are visually distinct.
2. Risk map displays at least several factory zones.
3. Alert detail includes timestamp, location, severity, and action history.

## 7.6 Analytics Module

### 7.6.1 Purpose

The Analytics module helps managers understand productivity trends and improvement opportunities.

### 7.6.2 Required UI Sections

1. Date range filter.
2. Line or area filter.
3. Output trend chart.
4. Target vs actual chart.
5. Defect rate chart.
6. Downtime analysis.
7. AI improvement suggestions.

### 7.6.3 Functional Requirements

1. User can filter analytics by date and line.
2. System shows production trends.
3. System compares planned and actual output.
4. System identifies low-performing lines.
5. System displays improvement suggestions.

### 7.6.4 Acceptance Criteria

1. Charts render correctly with mock data.
2. Filters update the visible data.
3. AI suggestions include reason and expected impact.

## 7.7 Workforce Module

### 7.7.1 Purpose

The Workforce module supports shift planning, employee allocation, absence tracking, and overtime planning.

### 7.7.2 Required UI Sections

1. Workforce overview cards.
2. Shift calendar or schedule table.
3. Employee availability list.
4. Line allocation table.
5. Overtime suggestion panel.
6. AI schedule recommendation panel.

### 7.7.3 Functional Requirements

1. User can view workforce availability.
2. User can enter production target and planning date.
3. System suggests workers per line.
4. System suggests overtime if target cannot be reached.
5. User can approve, modify, or reject AI schedule recommendation.
6. System sends notification to affected employees after schedule approval.

### 7.7.4 Acceptance Criteria

1. Workforce plan shows employees, shifts, and line assignments.
2. AI recommendation is generated from mock target and availability data.
3. Manager can change recommendation status.

## 7.8 Forms Module

### 7.8.1 Purpose

The Forms module replaces paper workflows with electronic forms and approval tracking.

### 7.8.2 Required Form Types

1. Leave request.
2. Overtime request.
3. Machine issue report.
4. Warehouse import request.
5. Warehouse export request.

### 7.8.3 Functional Requirements

1. User can create a new form.
2. System prefills user information when possible.
3. User can submit a form.
4. Approver can approve or reject a form.
5. User can track form status.
6. Related module data updates when a form is approved.

### 7.8.4 Acceptance Criteria

1. Forms have required fields and validation.
2. Submitted forms appear in approval list.
3. Status changes are visible.
4. Approval or rejection creates a notification.

## 7.9 Notifications Module

### 7.9.1 Purpose

The Notifications module centralizes alerts, approvals, reminders, and system messages.

### 7.9.2 Functional Requirements

1. User can view all notifications.
2. User can filter by type and status.
3. User can open related module detail from a notification.
4. Notification status changes to read after opening.
5. Critical notifications appear on the dashboard.

### 7.9.3 Acceptance Criteria

1. Notification center lists multiple notification types.
2. Each notification has title, type, severity, time, and status.
3. Clicking notification opens related page or detail panel.

## 7.10 Reports Module

### 7.10.1 Purpose

The Reports module summarizes operational data for review and presentation.

### 7.10.2 Required Report Types

1. Production report.
2. Warehouse report.
3. Safety report.
4. Workforce report.
5. Forms report.

### 7.10.3 Functional Requirements

1. User can select report type.
2. User can select date range.
3. User can filter by line, area, or department.
4. System shows report preview.
5. User can export report as PDF or Excel in later versions.

### 7.10.4 Acceptance Criteria

1. Report preview loads with mock data.
2. Filters change the displayed report.
3. Export button exists even if export is simulated in MVP.

## 7.11 Settings Module

### 7.11.1 Purpose

The Settings module configures users, permissions, factory areas, production lines, alert thresholds, and AI rules.

### 7.11.2 Functional Requirements

1. Admin can view user list.
2. Admin can assign roles.
3. Admin can configure production lines.
4. Admin can configure warehouse zones.
5. Admin can configure alert thresholds.
6. Admin can configure AI recommendation rules.

### 7.11.3 MVP Requirement

Settings can be read-only in the first version.

## 8. Data Requirements

## 8.1 Core Entities

The system should include the following core data entities:

1. User
2. Role
3. Factory Area
4. Production Line
5. Production Record
6. Machine
7. Machine Issue
8. Warehouse Zone
9. Warehouse Item
10. Goods Movement
11. Safety Alert
12. Camera Event
13. Employee
14. Shift Plan
15. Form Request
16. Notification
17. AI Recommendation
18. Report

## 8.2 Suggested Entity Fields

### 8.2.1 User

1. id
2. fullName
3. email
4. role
5. department
6. status
7. createdAt

### 8.2.2 ProductionLine

1. id
2. name
3. area
4. status
5. targetOutput
6. actualOutput
7. efficiency
8. defectRate
9. downtimeMinutes
10. assignedWorkers

### 8.2.3 WarehouseItem

1. id
2. itemCode
3. itemName
4. batchCode
5. category
6. quantity
7. unit
8. zone
9. shelf
10. status
11. lastMovementAt

### 8.2.4 SafetyAlert

1. id
2. title
3. type
4. severity
5. location
6. status
7. detectedAt
8. assignedTo
9. description
10. actionHistory

### 8.2.5 ShiftPlan

1. id
2. date
3. shiftName
4. lineId
5. requiredWorkers
6. assignedEmployees
7. status
8. aiRecommendationId

### 8.2.6 FormRequest

1. id
2. formType
3. requesterId
4. approverId
5. status
6. submittedAt
7. approvedAt
8. payload
9. rejectionReason

### 8.2.7 AIRecommendation

1. id
2. module
3. title
4. reason
5. expectedImpact
6. status
7. createdAt
8. reviewedBy

## 9. Suggested API Requirements

## 9.1 Authentication APIs

```text
POST /auth/login
POST /auth/logout
GET  /auth/me
```

## 9.2 Dashboard APIs

```text
GET /dashboard/summary
GET /dashboard/kpis
GET /dashboard/alerts
GET /dashboard/activity
```

## 9.3 Warehouse APIs

```text
GET  /warehouse/summary
GET  /warehouse/items
GET  /warehouse/items/{id}
GET  /warehouse/movements
GET  /warehouse/recommendations
POST /warehouse/recommendations/{id}/accept
POST /warehouse/recommendations/{id}/reject
POST /warehouse/alerts/{id}/resolve
```

## 9.4 Production APIs

```text
GET  /production/lines
GET  /production/lines/{id}
GET  /production/records
GET  /production/issues
POST /production/issues
POST /production/issues/{id}/resolve
```

## 9.5 Safety APIs

```text
GET  /safety/summary
GET  /safety/alerts
GET  /safety/alerts/{id}
POST /safety/alerts/{id}/resolve
POST /safety/alerts/{id}/escalate
POST /safety/alerts/{id}/mark-false
```

## 9.6 AI Camera APIs

```text
GET  /cameras
GET  /cameras/events
GET  /cameras/events/{id}
POST /cameras/events/{id}/resolve
```

## 9.7 Workforce APIs

```text
GET  /workforce/summary
GET  /workforce/employees
GET  /workforce/shifts
POST /workforce/shift-plans
POST /workforce/recommendations/generate
POST /workforce/recommendations/{id}/apply
POST /workforce/recommendations/{id}/reject
```

## 9.8 Forms APIs

```text
GET  /forms
GET  /forms/{id}
POST /forms
POST /forms/{id}/approve
POST /forms/{id}/reject
```

## 9.9 Notifications APIs

```text
GET  /notifications
POST /notifications/{id}/read
POST /notifications/{id}/resolve
```

## 9.10 Reports APIs

```text
GET /reports/production
GET /reports/warehouse
GET /reports/safety
GET /reports/workforce
GET /reports/forms
```

## 10. Business Rules

### 10.1 Production Rules

1. A line is marked as warning when efficiency is below 85%.
2. A line is marked as stopped when actual status is stopped or downtime exceeds the configured threshold.
3. A high defect rate should create a production alert.
4. Low output should trigger AI root-cause analysis.

### 10.2 Warehouse Rules

1. Warehouse occupancy above 90% should create a capacity warning.
2. Items stored in the wrong zone should create a wrong placement alert.
3. Low inventory should be shown when quantity is below minimum stock level.
4. AI storage recommendation should consider item category, movement frequency, and available capacity.

### 10.3 Safety Rules

1. Restricted-zone entry should create at least a high severity alert.
2. Critical safety alerts should appear on the dashboard immediately.
3. Every safety alert must have a final status: resolved, escalated, false alert, or investigation required.

### 10.4 Workforce Rules

1. If available workers are fewer than required workers, the system should show a staffing shortage alert.
2. Overtime recommendation should only be generated when output target cannot be reached with current staffing.
3. AI shift recommendations must be reviewable and editable by a manager before applying.

### 10.5 Forms Rules

1. Leave requests should check schedule impact.
2. Overtime requests require manager approval.
3. Machine issue reports should notify production and maintenance users.
4. Approved warehouse import/export forms should update warehouse movement records.

## 11. UI and UX Requirements

### 11.1 Layout Requirements

1. The application should use a dashboard layout with a sidebar and topbar.
2. The sidebar should contain the main modules.
3. The topbar should include search, notification count, and user profile.
4. Important alerts should be visible without scrolling on desktop.
5. Cards, tables, and charts should be easy to scan.

### 11.2 Visual Requirements

1. The design should feel operational, modern, and professional.
2. Color should communicate status clearly:
   - Green for normal or success.
   - Amber for warning.
   - Red for critical or danger.
   - Neutral colors for default information.
3. The interface should not look like a marketing landing page.
4. The design should support dense information while remaining readable.

### 11.3 Responsive Requirements

1. Desktop layout should show sidebar and multi-column dashboard panels.
2. Tablet layout may reduce dashboard columns.
3. Mobile layout should stack sections vertically.
4. Text must not overflow cards or buttons.

### 11.4 Interaction Requirements

1. Cards should be clickable when they represent a drill-down path.
2. Tables should support filtering and searching in later versions.
3. Alerts should have clear action buttons.
4. Forms should show validation errors clearly.
5. Loading, empty, and error states should be designed for each module.

## 12. Non-Functional Requirements

### 12.1 Performance

1. Dashboard should load quickly with mock data.
2. Data tables should remain usable with at least 100 rows in the demo version.
3. Charts should render without blocking the UI.

### 12.2 Security

1. Role-based access should be planned from the beginning.
2. Users should only see modules allowed by their role.
3. API endpoints should require authentication in backend versions.
4. Sensitive operational data should not be stored in frontend code in production.

### 12.3 Maintainability

1. UI should be split into reusable components.
2. Mock data should be separated from UI components.
3. Types should be defined for core entities.
4. API service functions should be separated from page components.

### 12.4 Accessibility

1. Buttons and inputs should be keyboard accessible.
2. Color should not be the only way to show severity.
3. Text contrast should be readable.
4. Form labels should be clear.

## 13. MVP Scope

The MVP should include the following features:

1. Dashboard with mock KPIs and alerts.
2. Sidebar navigation.
3. Production line overview and line detail.
4. Warehouse overview and item search.
5. Safety alert list and alert detail.
6. Workforce planning page with AI-style suggestion.
7. Forms overview with submit and approval simulation.
8. Reports overview with mock report preview.
9. Notification center.

The MVP should run with the C# backend and SQLite sample database. The frontend can still keep local mock data as a fallback when the backend is not running.

## 14. Out of Scope for MVP

The following features should not be required in the first version:

1. Real camera integration.
2. Real AI model training.
3. Real-time sensor connection.
4. Complex authentication.
5. PDF or Excel export implementation.
6. Advanced role-based permission editor.
7. Multi-factory support.
8. Live WebSocket updates.

These features can be added after the MVP is stable.

## 15. Mock Data Requirements

The frontend should include fallback mock data for:

1. Users and roles.
2. Dashboard KPIs.
3. Production lines.
4. Production records.
5. Warehouse items.
6. Warehouse zones.
7. Goods movements.
8. Safety alerts.
9. Camera events.
10. Employees.
11. Shift plans.
12. Forms.
13. Notifications.
14. Reports.
15. AI recommendations.

Mock/fallback data should be realistic enough for presentation and should cover normal, warning, and critical states. The backend should use SQLite sample data first and JSON fallback if SQLite is unavailable.

## 16. Implementation Milestones

### 16.1 Milestone 1: Frontend Foundation

Deliverables:

1. React project setup.
2. Main layout with sidebar and topbar.
3. Routing structure.
4. Shared UI components.
5. Mock data structure.

### 16.2 Milestone 2: Dashboard MVP

Deliverables:

1. KPI cards.
2. Production summary.
3. Safety alert summary.
4. Warehouse summary.
5. Workforce summary.
6. Pending forms panel.

### 16.3 Milestone 3: Core Pages

Deliverables:

1. Production page.
2. Warehouse page.
3. Safety page.
4. Workforce page.
5. Forms page.
6. Reports page.

### 16.4 Milestone 4: Interactions

Deliverables:

1. Search and filters.
2. Detail panels.
3. Alert status updates.
4. Form submission simulation.
5. AI recommendation apply/reject actions.

### 16.5 Milestone 5: Backend Preparation

Deliverables:

1. ASP.NET Core MVC project setup.
2. Basic API endpoints grouped by controller.
3. SQLite-first data loading with JSON fallback.
4. Frontend service layer connected to backend.

## 17. Definition of Done

The implementation can be considered complete for the MVP when:

1. User can open the application and view the dashboard.
2. Sidebar navigation works for MVP pages.
3. Dashboard displays realistic factory data.
4. Production, Warehouse, Safety, Workforce, Forms, and Reports pages exist.
5. At least one detail view exists for Production or Safety.
6. User can simulate changing an alert, form, or recommendation status.
7. UI works on desktop and mobile widths.
8. No obvious layout overflow occurs.
9. Mock data is separated from UI components.
10. Documentation remains updated in the `docs/` folder.

## 18. Detailed Screen Requirements

This section describes what each MVP screen should contain in more implementation-ready detail.

### 18.1 Login Screen

Purpose:

Allow users to enter the application and simulate role-based access for the demo.

Required UI elements:

1. Application name and short system description.
2. Email input.
3. Password input.
4. Role selector for demo mode: Factory Manager, Production Manager, Warehouse Manager, Safety Officer, Employee.
5. Login button.
6. Error message area.

Main behavior:

1. If the user enters demo credentials or selects demo role, the app should redirect to Dashboard.
2. If required fields are empty, the system should show validation messages.
3. Selected role should determine which navigation items and actions are visible.

MVP simplification:

Authentication can be simulated in frontend state. Real token-based login can be added later.

### 18.2 Dashboard Screen

Purpose:

Show the factory control overview and direct the user to the most urgent operational issue.

Required UI elements:

1. Sidebar navigation.
2. Topbar with search, notification count, and profile menu.
3. Factory summary panel.
4. KPI card grid.
5. Production status panel.
6. Safety alert panel.
7. Warehouse signal panel.
8. Workforce recommendation panel.
9. Pending forms panel.
10. Reports summary panel.

Required interactions:

1. Clicking a production line opens Production Line Detail.
2. Clicking a safety alert opens Safety Alert Detail.
3. Clicking a warehouse issue opens Warehouse Item or Warehouse Alert Detail.
4. Clicking a workforce recommendation opens Workforce Planning.
5. Clicking a pending form opens Form Detail.

Required states:

1. Normal: no critical alerts.
2. Warning: one or more operational risks need attention.
3. Critical: one or more safety or stopped-line issues need immediate action.
4. Loading: dashboard data is being loaded.
5. Empty: no data is available for selected filters.
6. Error: data cannot be loaded.

### 18.3 Production Screen

Purpose:

Allow production managers to monitor all lines and inspect line-level problems.

Required UI elements:

1. Line status filter: All, Normal, Warning, Stopped, Maintenance.
2. Search input for line name or machine code.
3. Production line table.
4. Output vs target chart.
5. Defect rate chart.
6. Downtime summary.
7. AI root-cause suggestion panel.

Production line table columns:

1. Line name.
2. Area.
3. Status.
4. Target output.
5. Actual output.
6. Efficiency.
7. Defect rate.
8. Downtime minutes.
9. Assigned workers.
10. Action button.

Line detail required data:

1. Current status.
2. Target and actual output.
3. Hourly output trend.
4. Machine status.
5. Downtime reason.
6. Defect records.
7. Assigned employees.
8. AI recommendation.
9. Related machine issue reports.

### 18.4 Warehouse Screen

Purpose:

Allow warehouse managers to track goods, manage item locations, and review storage recommendations.

Required UI elements:

1. Warehouse occupancy cards.
2. Zone capacity overview.
3. Item search input.
4. Item table.
5. Wrong placement alert panel.
6. AI storage recommendation panel.
7. Goods movement history.

Item table columns:

1. Item code.
2. Item name.
3. Batch code.
4. Category.
5. Quantity.
6. Current zone.
7. Shelf.
8. Status.
9. Last movement time.
10. Action button.

Required interactions:

1. Search item by item code, name, or batch code.
2. Open item detail.
3. Accept storage recommendation.
4. Override storage recommendation with reason.
5. Resolve wrong placement alert.
6. Mark wrong placement alert as false.

### 18.5 Safety Screen

Purpose:

Allow safety officers to review, classify, and handle safety alerts.

Required UI elements:

1. Safety KPI cards.
2. Factory risk map.
3. Active safety alert list.
4. Alert severity filters.
5. Alert detail panel.
6. Action history.
7. High-risk area summary.

Safety alert list columns:

1. Alert title.
2. Type.
3. Severity.
4. Location.
5. Detected time.
6. Assigned user.
7. Status.
8. Action button.

Required interactions:

1. Open alert detail.
2. Add response note.
3. Mark alert as resolved.
4. Escalate alert.
5. Mark alert as false.
6. Filter by severity and status.

### 18.6 Workforce Screen

Purpose:

Allow managers to create shift plans and review AI scheduling recommendations.

Required UI elements:

1. Date selector.
2. Production target input.
3. Available employee summary.
4. Absence and holiday list.
5. Shift schedule table.
6. Employee allocation by line.
7. AI recommendation panel.
8. Overtime suggestion panel.

Shift schedule table columns:

1. Shift name.
2. Production line.
3. Required workers.
4. Assigned workers.
5. Gap or surplus.
6. Overtime required.
7. Status.
8. Action button.

Required interactions:

1. Generate AI schedule recommendation.
2. Apply recommendation.
3. Modify employee assignment.
4. Reject recommendation.
5. Publish final schedule.
6. Notify affected employees.

### 18.7 Forms Screen

Purpose:

Allow employees to submit electronic forms and managers to approve or reject requests.

Required UI elements:

1. Form type selector.
2. Form list with status filters.
3. Form creation modal or page.
4. Form detail panel.
5. Approval action area.
6. Rejection reason input.

Required form fields:

Leave request:

1. Requester.
2. Department.
3. Shift.
4. Leave date.
5. Leave type.
6. Reason.

Overtime request:

1. Requester.
2. Line.
3. Date.
4. Overtime hours.
5. Reason.

Machine issue report:

1. Reporter.
2. Machine code.
3. Production line.
4. Severity.
5. Description.
6. Optional image or evidence placeholder.

Warehouse import/export request:

1. Requester.
2. Item code.
3. Quantity.
4. Batch code.
5. Warehouse zone.
6. Reason.

### 18.8 Reports Screen

Purpose:

Allow users to preview operational reports using backend sample data or frontend fallback data.

Required UI elements:

1. Report type selector.
2. Date range filter.
3. Line, area, or department filter.
4. Report summary cards.
5. Report table.
6. Export button.

Required report types:

1. Production report.
2. Warehouse report.
3. Safety report.
4. Workforce report.
5. Forms report.

MVP simplification:

Export can be simulated by showing a success notification instead of generating a real file.

## 19. Detailed Use Cases

### 19.1 Use Case: Monitor Factory Dashboard

Primary actor:

Factory Manager.

Preconditions:

1. User is logged in.
2. Dashboard sample data is available from SQLite, JSON fallback, or frontend fallback data.
3. At least one production line, warehouse zone, and safety alert exists in the sample data.

Main flow:

1. User opens Dashboard.
2. System loads KPIs and summary widgets.
3. System sorts critical alerts above warnings.
4. User reviews target completion and alert count.
5. User clicks a critical alert.
6. System opens related detail page.

Alternate flows:

1. If no alerts exist, system shows normal operating message.
2. If data fails to load, system shows retry action.
3. If user role has limited permission, hidden modules are not displayed.

Postconditions:

1. Selected issue detail is opened.
2. User action can update the dashboard state.

### 19.2 Use Case: Resolve Safety Alert

Primary actor:

Safety Officer.

Preconditions:

1. Safety alert exists.
2. User has permission to manage safety alerts.

Main flow:

1. User opens Safety module.
2. System displays active alerts by severity.
3. User selects a critical alert.
4. System displays location, detected time, severity, description, and suggested response.
5. User adds response note.
6. User chooses Resolve.
7. System updates alert status and action history.
8. System updates dashboard alert count.

Alternate flows:

1. User selects Escalate if the incident requires manager decision.
2. User selects Mark False if the detection is not valid.
3. If response note is required and empty, system blocks status update.

Postconditions:

1. Alert status is updated.
2. Safety report data is updated.
3. Notification status changes to resolved.

### 19.3 Use Case: Track Warehouse Item

Primary actor:

Warehouse Manager.

Preconditions:

1. Warehouse item data exists.
2. User has warehouse permission.

Main flow:

1. User opens Warehouse module.
2. User enters item code or batch code.
3. System searches item data.
4. System displays current zone, shelf, quantity, and movement history.
5. System checks whether item is in the correct zone.
6. If wrong placement exists, system shows alert.
7. User resolves the alert after item is moved.

Alternate flows:

1. If item is not found, system shows empty state.
2. If item is intentionally moved, user updates movement history instead of resolving as wrong placement.
3. If target zone is full, system suggests another available zone.

Postconditions:

1. Item location is confirmed or updated.
2. Movement history is saved.
3. Wrong placement alert is closed or marked false.

### 19.4 Use Case: Handle Low Production Efficiency

Primary actor:

Production Manager.

Preconditions:

1. A production line has efficiency below threshold.
2. Production data is available.

Main flow:

1. User opens Production module.
2. System highlights line with warning status.
3. User opens line detail.
4. System displays output, target, downtime, defect rate, and assigned workers.
5. AI suggestion panel shows possible root cause.
6. User applies recommended action or creates machine issue report.
7. System records the decision.

Alternate flows:

1. If line is stopped, system prioritizes downtime incident instead of efficiency analysis.
2. If high defect rate is the cause, system suggests quality inspection.
3. If staffing shortage is the cause, system suggests workforce adjustment.

Postconditions:

1. Production recommendation status is updated.
2. Related module receives request if needed.
3. Dashboard risk summary updates.

### 19.5 Use Case: Generate AI Shift Recommendation

Primary actor:

Factory Manager.

Preconditions:

1. Workforce data exists.
2. Production target is entered.
3. Employee availability is available.

Main flow:

1. User opens Workforce module.
2. User selects date and enters production target.
3. System loads available employees, absences, skills, and previous performance.
4. User clicks Generate Recommendation.
5. System creates suggested workers per line and overtime plan.
6. User reviews risk notes.
7. User applies or modifies the plan.
8. User publishes final schedule.

Alternate flows:

1. If target cannot be met, system suggests overtime or target adjustment.
2. If employee skill does not match line requirement, system warns before publishing.
3. If manager rejects recommendation, status becomes rejected and reason can be recorded.

Postconditions:

1. Shift plan is saved.
2. Employees receive notifications.
3. Workforce report includes the final plan.

### 19.6 Use Case: Submit and Approve Form

Primary actors:

Employee and Manager.

Preconditions:

1. Employee is logged in.
2. Manager account exists as approver.

Main flow:

1. Employee opens Forms module.
2. Employee selects form type.
3. System prefills available user information.
4. Employee enters required fields.
5. System validates the form.
6. Employee submits form.
7. Manager receives notification.
8. Manager opens form detail.
9. Manager approves or rejects request.
10. System notifies employee of result.

Alternate flows:

1. If required fields are missing, form cannot be submitted.
2. If leave request causes staffing shortage, manager sees warning before approval.
3. If rejected, manager must enter rejection reason.

Postconditions:

1. Form status is updated.
2. Notification is created.
3. Related data updates if applicable.

## 20. Detailed Demo Scenarios

### 20.1 Scenario: Critical Safety Alert

Precondition:

Line B is operating normally. Robot Cell 2 is configured as a restricted area. Safety Officer is logged in.

Steps:

1. AI Camera creates a restricted-zone event.
2. System classifies the event as critical.
3. Dashboard alert count increases.
4. Safety Officer opens Safety module.
5. Safety Officer opens alert detail.
6. Safety Officer reviews location, time, event type, and suggested action.
7. Safety Officer adds response note: `Worker warned and area cleared`.
8. Safety Officer marks alert as resolved.

Expected result:

1. Alert status becomes resolved.
2. Action history contains officer note.
3. Dashboard critical alert count decreases.
4. Safety report includes the incident.

### 20.2 Scenario: Production Line Efficiency Drops

Precondition:

Line C target output is 2,000 units. Actual output is 1,560 units. Efficiency is 78%.

Steps:

1. User opens Dashboard.
2. Dashboard shows Line C warning.
3. User clicks Line C.
4. Production detail page opens.
5. AI panel shows likely cause: staffing shortage.
6. User applies recommendation to request two operators from Line 5.
7. System creates workforce recommendation.

Expected result:

1. Production recommendation status becomes applied.
2. Workforce module receives staffing request.
3. Line C remains warning until staffing is updated.

### 20.3 Scenario: Warehouse Wrong Placement

Precondition:

Batch RM-204 should be in Raw Material Zone A but is detected in Finished Goods Zone C.

Steps:

1. Warehouse Manager opens Warehouse module.
2. System shows wrong placement alert.
3. Manager opens alert detail.
4. System displays expected zone and actual zone.
5. Manager confirms item is misplaced.
6. Manager records movement back to Zone A.
7. Manager resolves alert.

Expected result:

1. Item zone changes to Zone A.
2. Goods movement history records the correction.
3. Wrong placement alert becomes resolved.

### 20.4 Scenario: AI Suggests Overtime

Precondition:

Daily target is behind by 600 units. Night shift is missing two operators.

Steps:

1. Factory Manager opens Workforce module.
2. Manager enters daily target and selected date.
3. Manager clicks Generate Recommendation.
4. System suggests moving two certified operators to Line 3.
5. System suggests 1.5 overtime hours for packaging team.
6. Manager modifies one employee assignment.
7. Manager approves final plan.

Expected result:

1. Shift plan status becomes published.
2. Affected employees receive notifications.
3. Overtime record appears in workforce report.

### 20.5 Scenario: Employee Leave Request

Precondition:

Employee is assigned to Line A morning shift.

Steps:

1. Employee opens Forms module.
2. Employee selects Leave Request.
3. System prefills employee name, department, and shift.
4. Employee selects leave date and enters reason.
5. System checks staffing impact.
6. Employee submits request.
7. Manager approves the request.

Expected result:

1. Form status becomes approved.
2. Employee receives approval notification.
3. Workforce availability updates for that date.

### 20.6 Scenario: Machine Fault Report

Precondition:

Operator detects abnormal vibration on Press Unit P-14 while Line D is running.

Steps:

1. Employee opens Machine Issue Report.
2. Employee selects Press Unit P-14 and Line D.
3. Employee selects severity: High.
4. Employee enters description.
5. Employee submits report.
6. System notifies Production Manager.
7. System marks Line D as warning.

Expected result:

1. Machine issue report status becomes submitted.
2. Production alert is created.
3. Line D detail shows related machine issue.

## 21. State Transition Requirements

### 21.1 Production Line Status Transitions

```text
Normal
|-- efficiency below 85% --> Warning
|-- machine stopped --> Stopped
|-- scheduled maintenance --> Maintenance

Warning
|-- issue resolved --> Normal
|-- machine stopped --> Stopped
|-- maintenance started --> Maintenance

Stopped
|-- repair completed --> Normal
|-- maintenance started --> Maintenance

Maintenance
|-- maintenance completed --> Normal
```

### 21.2 Safety Alert Status Transitions

```text
New
|-- opened by officer --> In Review
|-- critical auto-route --> Escalated

In Review
|-- officer resolves --> Resolved
|-- officer escalates --> Escalated
|-- false detection --> False Alert
|-- more evidence needed --> Investigation Required

Escalated
|-- manager resolves --> Resolved
|-- investigation needed --> Investigation Required
```

### 21.3 Form Status Transitions

```text
Draft
|-- submit --> Pending Approval

Pending Approval
|-- approver approves --> Approved
|-- approver rejects --> Rejected
|-- requester cancels --> Cancelled

Rejected
|-- requester edits --> Draft
```

### 21.4 AI Recommendation Status Transitions

```text
New
|-- user opens --> Reviewed

Reviewed
|-- user applies --> Applied
|-- user modifies and applies --> Modified
|-- user rejects --> Rejected
```

## 22. Validation Requirements

### 22.1 General Form Validation

1. Required fields must not be empty.
2. Date fields must use valid dates.
3. Numeric fields must not accept negative values.
4. Status fields must only use allowed values.
5. Rejection reason is required when rejecting a form.

### 22.2 Production Validation

1. Target output must be greater than 0.
2. Actual output must be 0 or greater.
3. Efficiency must be between 0 and 100.
4. Defect rate must be between 0 and 100.
5. Downtime minutes must be 0 or greater.

### 22.3 Warehouse Validation

1. Quantity must be greater than or equal to 0.
2. Item code must be unique.
3. Batch code must not be empty for tracked items.
4. Warehouse zone must exist before assigning item location.
5. A full zone cannot be recommended for new storage.

### 22.4 Safety Validation

1. Severity must be Low, Medium, High, or Critical.
2. Alert location must map to a known factory area.
3. Resolve action must include response note for High or Critical alerts.
4. False alert action should include reason.

### 22.5 Workforce Validation

1. Required workers must be greater than 0.
2. Assigned workers must not exceed available workers unless overtime is approved.
3. Employee must have required skill for assigned line.
4. Overtime hours must not exceed configured limit.

## 23. Error, Empty, and Loading States

Each major module must define these states:

1. Loading state: data is being fetched or generated.
2. Empty state: no records match the current filter.
3. Error state: data cannot be loaded or action fails.
4. Success state: action is completed successfully.
5. Permission denied state: user role cannot access the action.

Examples:

1. Dashboard error: `Unable to load factory summary. Retry`.
2. Warehouse empty search: `No item found for this code`.
3. Safety permission denied: `You do not have permission to close this alert`.
4. Forms validation error: `Rejection reason is required`.
5. Workforce generation loading: `Generating optimized schedule`.

## 24. Component Requirements for React Implementation

### 24.1 Layout Components

1. `AppLayout`
2. `Sidebar`
3. `Topbar`
4. `PageHeader`
5. `ContentGrid`

### 24.2 Shared UI Components

1. `KpiCard`
2. `StatusBadge`
3. `SeverityBadge`
4. `DataTable`
5. `FilterBar`
6. `SearchInput`
7. `ActionButton`
8. `DetailDrawer`
9. `ConfirmDialog`
10. `EmptyState`
11. `LoadingState`
12. `ErrorState`

### 24.3 Module Components

Dashboard:

1. `FactorySummaryPanel`
2. `ProductionSummaryPanel`
3. `SafetyAlertPanel`
4. `WarehouseSignalPanel`
5. `WorkforceRecommendationPanel`

Production:

1. `ProductionLineTable`
2. `ProductionLineDetail`
3. `DowntimePanel`
4. `DefectRatePanel`
5. `RootCauseSuggestionPanel`

Warehouse:

1. `WarehouseZoneOverview`
2. `WarehouseItemTable`
3. `ItemLocationDetail`
4. `StorageRecommendationPanel`
5. `GoodsMovementHistory`

Safety:

1. `FactoryRiskMap`
2. `SafetyAlertList`
3. `SafetyAlertDetail`
4. `IncidentActionHistory`

Workforce:

1. `ShiftPlanningTable`
2. `EmployeeAvailabilityList`
3. `AiScheduleRecommendation`
4. `OvertimeSuggestionPanel`

Forms:

1. `FormTypeSelector`
2. `FormRequestList`
3. `FormRequestDetail`
4. `ApprovalActions`

## 25. Test Cases and Acceptance Checklist

### 25.1 Dashboard Test Cases

1. Dashboard displays all KPI cards with mock data.
2. Critical alert appears before warning alerts.
3. Clicking production line opens related line detail.
4. Clicking safety alert opens related alert detail.
5. Dashboard displays loading, empty, and error states.

### 25.2 Warehouse Test Cases

1. Search by item code returns correct item.
2. Search by unknown item code shows empty state.
3. Wrong placement alert can be resolved.
4. Storage recommendation can be accepted.
5. Storage recommendation can be overridden with reason.

### 25.3 Production Test Cases

1. Lines can be filtered by status.
2. Line below 85% efficiency is marked warning.
3. Stopped line is visually distinct.
4. Line detail shows output, target, efficiency, defect rate, and downtime.
5. Applying AI suggestion updates recommendation status.

### 25.4 Safety Test Cases

1. Alerts can be filtered by severity.
2. Critical alert requires response note before resolve.
3. Alert can be escalated.
4. Alert can be marked false with reason.
5. Resolving alert updates dashboard count.

### 25.5 Workforce Test Cases

1. AI recommendation can be generated from target and availability data.
2. Recommendation shows workers per line.
3. Recommendation shows overtime suggestion when needed.
4. Manager can modify assignment.
5. Published schedule creates notifications.

### 25.6 Forms Test Cases

1. Required fields block submission when empty.
2. Submitted form appears in pending list.
3. Manager can approve form.
4. Manager can reject form with reason.
5. Employee receives status notification.

### 25.7 Reports Test Cases

1. Report type can be selected.
2. Date range filter updates report preview.
3. Export button shows simulated success message.
4. Production report includes line output data.
5. Safety report includes resolved and active alert counts.

## 26. Traceability Matrix

| Requirement Area | Related Module | Main Actor | MVP Required | Related Scenario |
| --- | --- | --- | --- | --- |
| Factory overview | Dashboard | Factory Manager | Yes | Monitor Factory Dashboard |
| Production monitoring | Production | Production Manager | Yes | Production Line Efficiency Drops |
| Warehouse tracking | Warehouse | Warehouse Manager | Yes | Warehouse Wrong Placement |
| Safety incident handling | Safety, AI Cameras | Safety Officer | Yes | Critical Safety Alert |
| Shift planning | Workforce | Factory Manager | Yes | AI Suggests Overtime |
| Electronic forms | Forms | Employee, Manager | Yes | Employee Leave Request |
| Reports | Reports | Factory Manager | Yes | Production and Safety Reports |
| Settings | Settings | Admin | No for MVP | Later configuration phase |

## 27. Development Priority

Recommended implementation order:

1. React project setup and layout.
2. Mock data and TypeScript types.
3. Dashboard page.
4. Production page and line detail.
5. Safety page and alert detail.
6. Warehouse page and item search.
7. Workforce recommendation page.
8. Forms submission and approval simulation.
9. Reports preview.
10. Notification center.
11. Backend API preparation.

## 28. Recommended Next Step

The next practical step is to migrate the current static dashboard prototype into a React + Vite project. Start with the frontend only, keep all data mocked, and build the MVP pages based on this requirements document.