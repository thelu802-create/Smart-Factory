# Smart Factory AI Dashboard User Flows

## 1. Document Purpose

This document describes the detailed user flows for the Smart Factory AI Dashboard. It explains how different users move through the system, how data flows between modules, and how AI recommendations, alerts, and approvals should work in the application.

The goal is to support UI design, page planning, database design, and future implementation.

## 2. Main User Roles

### 2.1 Factory Manager

Main responsibilities:

1. Monitor the overall factory status.
2. Review production performance.
3. Approve shift plans and overtime.
4. Respond to critical alerts.
5. Review reports and make operational decisions.

Primary modules used:

1. Dashboard
2. Production Monitoring
3. Safety Management
4. Workforce Planning
5. Reports
6. Notifications

### 2.2 Production Manager

Main responsibilities:

1. Monitor line performance.
2. Detect production delays and quality issues.
3. Review AI productivity suggestions.
4. Coordinate with workforce and maintenance teams.

Primary modules used:

1. Production Monitoring
2. Productivity Analytics
3. AI Camera Monitoring
4. Notifications
5. Reports

### 2.3 Warehouse Manager

Main responsibilities:

1. Track item locations.
2. Monitor inventory status.
3. Review storage recommendations.
4. Handle import and export requests.
5. Resolve misplaced goods alerts.

Primary modules used:

1. Warehouse Management
2. Forms and Workflow Automation
3. Notifications
4. Reports

### 2.4 Safety Officer

Main responsibilities:

1. Monitor dangerous areas.
2. Review AI camera alerts.
3. Handle safety incidents.
4. Record incident responses.
5. Review high-risk area reports.

Primary modules used:

1. Safety Management
2. AI Camera Monitoring
3. Notifications
4. Reports

### 2.5 Employee

Main responsibilities:

1. View assigned shifts and work line.
2. Submit leave or overtime forms.
3. Report machine issues.
4. Receive alerts and instructions.

Primary modules used:

1. Forms and Workflow Automation
2. Notifications
3. Workforce Planning

## 3. Global Application Flow

```text
User opens application
|
|-- Login
|   |-- Valid account
|   |   |-- Load user role and permissions
|   |   |-- Redirect to Dashboard
|   |
|   |-- Invalid account
|       |-- Show error message
|       |-- Allow retry or password reset
|
|-- Dashboard
|   |-- Load factory KPIs
|   |-- Load active alerts
|   |-- Load production summary
|   |-- Load warehouse summary
|   |-- Load workforce summary
|
|-- User selects module
|   |-- Warehouse
|   |-- Production
|   |-- AI Cameras
|   |-- Safety
|   |-- Analytics
|   |-- Workforce
|   |-- Forms
|   |-- Reports
|
|-- System displays module data
|-- AI analyzes available data
|-- System shows alerts, insights, or recommendations
|-- User reviews and takes action
|-- System records the result
|-- Reports and future recommendations are updated
```

## 4. Dashboard Flow

### 4.1 Main Goal

The Dashboard gives users a quick view of the whole factory and helps them decide which problem should be handled first.

### 4.2 Flow Steps

```text
User logs in
|
|-- System checks user role
|-- System loads dashboard widgets based on permissions
|
|-- Dashboard displays:
|   |-- Daily output
|   |-- Target completion rate
|   |-- Active production lines
|   |-- Safety alerts
|   |-- Warehouse capacity
|   |-- Workforce status
|   |-- Pending forms
|
|-- User reviews summary cards
|
|-- If no issue exists:
|   |-- User continues monitoring
|
|-- If issue exists:
|   |-- User clicks related alert or module card
|   |-- System redirects to the detailed module page
```

### 4.3 Dashboard Actions

1. Open production line detail.
2. Open safety alert detail.
3. Open warehouse issue detail.
4. Open shift planning recommendation.
5. Open pending form approval.
6. Export quick summary report.

## 5. Warehouse Management Flow

### 5.1 Main Goal

The Warehouse flow helps users track goods, find storage locations, and optimize warehouse space.

### 5.2 Item Location Tracking Flow

```text
Warehouse manager opens Warehouse module
|
|-- Selects Item Location Tracking
|-- Enters item code, batch code, or product name
|-- System searches warehouse database
|
|-- If item is found:
|   |-- Show warehouse zone
|   |-- Show shelf or pallet location
|   |-- Show quantity
|   |-- Show last movement time
|   |-- Show movement history
|
|-- If item is not found:
|   |-- Show no result message
|   |-- Suggest checking import/export records
```

### 5.3 AI Storage Recommendation Flow

```text
New goods arrive at warehouse
|
|-- Warehouse staff creates import request
|-- System reads item type, quantity, size, and movement frequency
|-- AI checks current warehouse capacity
|-- AI suggests best storage zone
|
|-- Warehouse manager reviews suggestion
|   |-- Accept suggestion
|   |   |-- System assigns storage location
|   |   |-- Goods movement record is created
|   |
|   |-- Modify suggestion
|   |   |-- Manager selects another location
|   |   |-- System records manual override reason
|
|-- Inventory status is updated
```

### 5.4 Wrong Placement Alert Flow

```text
System detects item in unexpected location
|
|-- Create wrong placement alert
|-- Notify warehouse manager
|-- Show expected location and actual location
|
|-- Manager assigns staff to verify
|-- Staff confirms item position
|
|-- If wrong placement is confirmed:
|   |-- Move item to correct zone
|   |-- Close alert
|   |-- Save movement history
|
|-- If alert is false:
|   |-- Mark as false alert
|   |-- Add review note
```

## 6. Production Monitoring Flow

### 6.1 Main Goal

The Production flow helps managers monitor output, line status, downtime, and defects in real time.

### 6.2 Line Monitoring Flow

```text
Production manager opens Production module
|
|-- System displays all production lines
|-- Each line shows status, output, efficiency, and defect rate
|
|-- User selects a line
|-- System opens Line Detail page
|   |-- Current output
|   |-- Target output
|   |-- Machine status
|   |-- Downtime duration
|   |-- Defect count
|   |-- Assigned workforce
|
|-- User reviews line condition
```

### 6.3 Low Performance Alert Flow

```text
Line efficiency falls below threshold
|
|-- System creates performance alert
|-- AI checks possible causes:
|   |-- Machine downtime
|   |-- Low staffing
|   |-- High defect rate
|   |-- Material shortage
|
|-- System shows recommended actions
|
|-- Production manager chooses action:
|   |-- Request maintenance
|   |-- Request additional workers
|   |-- Adjust production target
|   |-- Monitor without action
|
|-- System records decision and outcome
```

### 6.4 Machine Downtime Flow

```text
Machine stops unexpectedly
|
|-- Sensor or operator reports downtime
|-- System updates line status to stopped or warning
|-- Notification is sent to production manager
|
|-- Manager reviews downtime details
|-- Manager creates machine issue report or assigns maintenance
|
|-- Maintenance team updates repair status
|-- Line resumes operation
|-- System calculates downtime impact
|-- Report is updated
```

## 7. AI Camera and Safety Flow

### 7.1 Main Goal

The AI Camera and Safety flow helps detect dangerous events, warn users quickly, and record incident handling.

### 7.2 Restricted Zone Detection Flow

```text
AI camera detects person entering restricted area
|
|-- System identifies camera location
|-- System creates safety alert
|-- Alert severity is classified
|-- Snapshot or event log is stored
|
|-- Safety officer receives notification
|-- Safety officer opens alert detail
|   |-- Camera location
|   |-- Timestamp
|   |-- Detected event type
|   |-- Severity level
|   |-- Suggested response
|
|-- Safety officer takes action
|   |-- Warn worker immediately
|   |-- Dispatch safety staff
|   |-- Mark as false alert
|   |-- Escalate to factory manager
|
|-- System records action and closes or updates alert
```

### 7.3 Obstacle Detection on Production Line Flow

```text
AI camera detects obstacle on production line
|
|-- System sends alert to production manager and safety officer
|-- Affected line is marked as warning
|-- System recommends pausing or inspecting the line
|
|-- User confirms inspection
|-- Staff removes obstacle or confirms false detection
|
|-- If obstacle is removed:
|   |-- Line returns to normal
|   |-- Incident log is closed
|
|-- If false detection:
|   |-- Alert is marked as false
|   |-- AI log is updated for review
```

### 7.4 Safety Incident Closure Flow

```text
Safety alert is created
|
|-- Safety officer reviews alert
|-- Officer assigns response action
|-- Officer adds notes and evidence
|-- Officer selects incident result:
|   |-- Resolved
|   |-- Escalated
|   |-- False alert
|   |-- Needs investigation
|
|-- System saves incident record
|-- Safety report is updated
|-- High-risk area statistics are recalculated
```

## 8. Productivity Analytics Flow

### 8.1 Main Goal

The Productivity Analytics flow helps users understand performance trends and make improvement decisions.

### 8.2 Analysis Flow

```text
User opens Analytics module
|
|-- Selects date range
|-- Selects factory area or production line
|-- System loads historical output, defects, downtime, and staffing data
|-- AI analyzes trends
|
|-- System displays:
|   |-- Output trend
|   |-- Target vs actual comparison
|   |-- Efficiency by line
|   |-- Defect rate
|   |-- Peak efficiency time
|   |-- Suggested improvements
|
|-- User exports analysis or opens related module
```

### 8.3 AI Improvement Suggestion Flow

```text
AI detects improvement opportunity
|
|-- Example: Line C performs better with senior operators
|-- System creates recommendation
|-- User reviews evidence and expected impact
|
|-- User decision:
|   |-- Apply recommendation
|   |-- Modify recommendation
|   |-- Reject recommendation
|
|-- System records user decision
|-- Future recommendation quality can be reviewed
```

## 9. Workforce and Shift Planning Flow

### 9.1 Main Goal

The Workforce flow helps managers plan shifts, allocate employees to lines, and decide overtime based on targets and historical performance.

### 9.2 AI Shift Planning Flow

```text
Factory manager opens Workforce module
|
|-- Enters production target
|-- Selects planning date or week
|-- System loads:
|   |-- Available employees
|   |-- Employee skills
|   |-- Absences and holidays
|   |-- Previous line performance
|   |-- Current production demand
|
|-- AI generates shift plan
|   |-- Suggested number of workers per line
|   |-- Suggested employee allocation
|   |-- Suggested overtime hours
|   |-- Risk notes
|
|-- Manager reviews plan
|   |-- Approve plan
|   |-- Modify employee allocation
|   |-- Reject and regenerate
|
|-- Final plan is saved
|-- Employees receive shift notification
```

### 9.3 Overtime Suggestion Flow

```text
System predicts target may not be achieved
|
|-- AI calculates production gap
|-- AI checks available employees and overtime limits
|-- System suggests overtime plan
|
|-- Manager reviews:
|   |-- Expected recovered output
|   |-- Required employees
|   |-- Overtime duration
|   |-- Cost impact
|
|-- Manager approves or rejects suggestion
|-- Notification is sent to selected employees
|-- Overtime record is saved for reporting
```

## 10. Forms and Workflow Automation Flow

### 10.1 Main Goal

The Forms flow reduces manual paperwork and routes internal requests to the correct approver.

### 10.2 Leave Request Flow

```text
Employee opens Forms module
|
|-- Selects Leave Request
|-- System prefills employee information
|-- Employee enters leave date and reason
|-- System checks schedule impact
|
|-- If leave causes staffing risk:
|   |-- Show warning to employee and manager
|
|-- Employee submits request
|-- Manager receives approval notification
|
|-- Manager approves or rejects
|-- Employee receives result notification
|-- Workforce schedule is updated if approved
```

### 10.3 Machine Issue Report Flow

```text
Employee detects machine issue
|
|-- Opens Machine Issue Report form
|-- Selects machine, line, issue type, and severity
|-- Adds description or image if available
|-- Submits report
|
|-- System notifies production manager and maintenance team
|-- Line status may change to warning
|-- Maintenance team updates issue progress
|-- Report is closed when issue is resolved
|-- Downtime and repair data are saved
```

### 10.4 Warehouse Import or Export Request Flow

```text
Warehouse staff opens Forms module
|
|-- Selects Import or Export Request
|-- Enters item code, quantity, batch, and reason
|-- System validates inventory or capacity
|
|-- If valid:
|   |-- Request is submitted to warehouse manager
|   |-- Manager approves request
|   |-- Inventory is updated
|   |-- Movement history is recorded
|
|-- If invalid:
|   |-- System shows capacity or stock warning
|   |-- User edits request or cancels
```

## 11. Notifications Flow

### 11.1 Main Goal

Notifications ensure that the right users receive the right information at the right time.

### 11.2 Notification Routing Flow

```text
Event occurs in system
|
|-- Event type is identified:
|   |-- Production issue
|   |-- Safety risk
|   |-- Warehouse issue
|   |-- Staffing issue
|   |-- Form approval
|
|-- System determines responsible role
|-- Notification is created
|-- Notification appears in Notification Center
|-- Critical alerts also appear on Dashboard
|
|-- User opens notification
|-- User is redirected to related detail page
|-- Notification status changes to read or resolved
```

### 11.3 Notification Statuses

1. Unread: notification has not been opened.
2. Read: user has opened the notification.
3. In progress: related issue is being handled.
4. Resolved: related issue is completed.
5. Escalated: issue requires higher-level attention.

## 12. Reports Flow

### 12.1 Main Goal

Reports summarize operational data for review, presentation, and decision-making.

### 12.2 Report Generation Flow

```text
User opens Reports module
|
|-- Selects report type
|   |-- Production report
|   |-- Warehouse report
|   |-- Safety report
|   |-- Workforce report
|
|-- Selects date range
|-- Selects filters such as line, area, or department
|-- System loads related data
|-- System generates report preview
|
|-- User reviews report
|-- User exports report as PDF or Excel
|-- Export record is saved
```

## 13. Settings and Administration Flow

### 13.1 Main Goal

Settings allow administrators to configure the factory structure, users, permissions, and AI rules.

### 13.2 User and Role Management Flow

```text
Admin opens Settings module
|
|-- Selects User Management
|-- Creates or edits user account
|-- Assigns role and department
|-- Sets permissions
|-- Saves changes
|-- System applies access rules immediately
```

### 13.3 Alert Threshold Configuration Flow

```text
Admin opens Alert Threshold Settings
|
|-- Selects alert type
|   |-- Production efficiency
|   |-- Safety zone
|   |-- Warehouse capacity
|   |-- Staffing shortage
|
|-- Updates threshold value
|-- Saves configuration
|-- Future alerts follow the new rule
```

## 14. Critical Data States

### 14.1 Production Line Status

1. Normal: line is running as expected.
2. Slow: line is below target but still operating.
3. Warning: issue detected and action may be needed.
4. Stopped: line is not operating.
5. Maintenance: line is under repair or inspection.

### 14.2 Alert Severity

1. Low: informational issue with low urgency.
2. Medium: issue should be reviewed soon.
3. High: issue needs immediate response.
4. Critical: issue may affect safety, production, or compliance.

### 14.3 Form Status

1. Draft: form is being prepared.
2. Submitted: form has been sent for approval.
3. Pending approval: approver has not decided yet.
4. Approved: request has been accepted.
5. Rejected: request has been denied.
6. Cancelled: requester cancelled the form.

### 14.4 Recommendation Status

1. New: recommendation has just been generated.
2. Reviewed: user has opened it.
3. Applied: user accepted and applied it.
4. Modified: user changed the recommendation before applying.
5. Rejected: user decided not to use it.

## 15. MVP Flow Scope

For the first demo version, the application should focus on the following flows:

1. Login to Dashboard.
2. Dashboard to Production Line Detail.
3. Dashboard to Safety Alert Detail.
4. Dashboard to Warehouse Issue Detail.
5. Workforce Planning with AI suggestion.
6. Submit and approve a basic form.
7. Generate a simple production or safety report.

These flows are enough to demonstrate the main value of the system without making the prototype too large.

## 16. Recommended Flow Priority

The recommended implementation priority is:

1. Dashboard overview flow.
2. Production monitoring flow.
3. Safety alert flow.
4. Warehouse tracking flow.
5. Workforce scheduling flow.
6. Forms approval flow.
7. Reports flow.
8. Settings and administration flow.

This order keeps the demo focused on the most visible factory control experience first, then expands into supporting modules.