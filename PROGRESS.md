# Smart Factory AI Dashboard — Development Progress

> A single file for the whole team to track how far the app has come,
> described from the **end-user (UI) perspective**. Update it whenever a feature is completed.

**Last updated:** 2026-07-21

---

## 1. Status legend

| Symbol | Meaning |
|---|---|
| ✅ **Complete** | Works end-to-end; the action **writes and persists** to the database (SQLite on disk) |
| 🟢 **Viewable (read-only)** | Displays real data from the database, but **has no write action yet** |
| 🟡 **Stub button** | A button/endpoint exists but clicking it **does not persist** — returns a hard-coded result, demo only |
| ⚪ **Mock data** | The page only uses hard-coded mock data in the frontend, **not wired to the API** |
| ⬜ **Not started** | Not begun |

---

## 2. System overview

- **Frontend:** React + Vite + TypeScript — runs at `http://localhost:5173`
- **Backend:** C# ASP.NET Core Web API — runs at `http://localhost:8000`
- **Database:** **SQL Server** (local `.\SQLEXPRESS`, database `SmartFactory`, Windows Authentication) via **EF Core code-first**. The schema is generated from C# entities by migrations (`dotnet ef`), and the demo data is seeded on startup from `database/seed.sql`. SQL Server is now required (no SQLite/JSON fallback).

Full run instructions: see [README.md](README.md) and [backend-dotnet/README.md](backend-dotnet/README.md).
Quick start for testing:
1. Backend: open `backend-dotnet` → run the **`SmartFactory.Api`** profile (Visual Studio) or `dotnet run --urls http://localhost:8000`.
2. Frontend: open `frontend` → `npm run dev`.
3. Open `http://localhost:5173`.

> ⚠️ Start the **backend first**. If the backend is down or on the wrong port, pages fall back to mock data and action buttons report "Failed to fetch".

---

## 3. Progress by feature (UI pages)

| Page (menu) | What the user sees / can do | Status | Notes |
|---|---|---|---|
| **Dashboard** | Overall KPIs (output, % of target completed, active lines, safety alerts) + summary | 🟢 Viewable | KPIs computed from real line & alert data |
| **Production** | Production line table: status, output, efficiency, defect rate, downtime; view one line's detail | 🟢 Viewable | No line-update action yet |
| **Warehouse** | **Searchable + paginated** inventory + **movement form** (Import / Export / Transfer) + per-item **movement history** | ✅ **Complete** | Movements adjust quantity/zone, recompute status + zone usage, record history, and **auto-raise a notification on Low Stock / Wrong Zone** — validated & persisted |
| **Safety** | Safety alert list; view detail; **Resolve / Escalate** buttons | ✅ **Complete** | **Resolve/escalate persists (with action_note), survives restart** |
| **Cameras** | AI camera event log + **Simulate detection** form (camera, incident type, severity, confidence) | ✅ **Complete** | A serious, confident detection (High/Critical + confidence ≥ 80%) **auto-raises a linked safety alert and safety notification** (two-way linked); low-severity detections are only logged — validated & persisted |
| **Workforce** | Shift plan by line + AI recommendations; **generate** button | ✅ **Complete** | **Generate computes gaps from shifts and inserts recommendations; persists across restart** |
| **Forms** | Approval queue for forms; **Approve / Reject** buttons | ✅ **Complete** | **Approve/reject persists to the DB and stays after a restart** |
| **Notifications** | Notification list (safety, production, warehouse, forms); mark as **read** | ✅ **Complete** | Mark-as-read persists; **warehouse movements and camera detections now auto-create alerts here (cross-page)** |
| **Reports** | Reporting page by module | ⚪ Mock data | Backend endpoints exist but the frontend is not wired; no real aggregation |
| **Analytics** | Performance charts derived from production data | 🟢 Viewable | Reads real data, derived charts |
| **Settings** | Configuration page | ⚪ Mock data | Static, no logic yet |

**Summary:** 6 complete write features (Forms, Safety, Notifications, Workforce, Warehouse, Cameras), 3 pages showing real data (read-only), **no stub buttons left**, 2 pages still mock.

---

## 4. API status (backend)

| Endpoint | Function | Status |
|---|---|---|
| `GET /health` | Liveness + data source (sqlite / json) | ✅ |
| `GET /dashboard/summary`, `/dashboard/kpis`, `/dashboard/alerts` | Overview metrics | 🟢 Real read |
| `GET /production/lines`, `/production/lines/{id}` | Lines + detail | 🟢 Real read |
| `GET /warehouse/items`, `/warehouse/items/{id}` | Inventory + detail | 🟢 Real read |
| `GET /safety/alerts`, `/safety/alerts/{id}` | Safety alerts + detail | 🟢 Real read |
| `POST /safety/alerts/{id}/resolve`, `/escalate` | Resolve / escalate alert | ✅ Real write (status + action_note) |
| `GET /cameras/events` | Camera events (live; includes linked `alertId`) | 🟢 Real read |
| `POST /cameras/detect` | Simulate a camera detection | ✅ Real write (multi-table txn; auto-raises linked safety alert + notification when severe) |
| `GET /workforce/shifts`, `/workforce/recommendations` | Shifts + recommendations | 🟢 Real read |
| `POST /workforce/recommendations/generate` | Generate recommendations | ✅ Real write (inserts ai_recommendations from shift gaps) |
| `GET /warehouse/zones` | Zone list (for transfer picker) | 🟢 Real read |
| `GET /warehouse/items/{id}/movements` | Per-item movement history | 🟢 Real read |
| `POST /warehouse/items/{id}/move` | Movement (Import/Export/Transfer) | ✅ Real write (multi-table txn + status/zone recompute) |
| `GET /forms` | Form list | 🟢 Real read (live) |
| `POST /forms/{id}/approve`, `/reject` | **Approve / reject form** | ✅ **Real write (transaction)** |
| `GET /notifications` | Notification list | 🟢 Real read |
| `POST /notifications/{id}/read` | Mark as read | ✅ Real write (status = Read) |
| `GET /reports/{module}` | Report by module | 🟡 Only re-wraps a list, no aggregation |

---

## 5. Infrastructure / foundations in place

- 🔵 **SQL Server + EF Core code-first** — `SmartFactoryDbContext` (31 entities, snake_case via `EFCore.NamingConventions`) is the single source of truth. `InitialCreate` migration builds the schema; `Migrate()` + `DatabaseSeeder` run on startup (creates schema if missing, seeds `seed.sql` when empty). Change the model → `dotnet dotnet-ef migrations add <Name>` → `database update`.
- 🔵 **Full 31-table schema** generated from the C# entities, including the enterprise relationship tables (permissions, io_masters, form_approval_steps, ...). Two-way FKs, unique indexes, and composite keys all reproduced. (`database/schema.sql` is kept for reference only; it is no longer executed.)
- 🔵 **Repository pattern on EF Core** — per-module repositories (Forms, Safety, Notifications, Workforce, Warehouse, Cameras) query and write through the `DbContext`, including multi-table transactions (warehouse move, camera auto-alert). `SampleDataService` is the EF-backed read/aggregation service for dashboard and list views.
- 🔵 **CORS** allowing the localhost frontend to call the API.
- 🔵 **Frontend layer** — `factoryApi` (API calls), `useApiData` (auto mock fallback + `reload()` after an action), reusable UI components (KpiCard, Panel, StatusBadge, PageHeader).

---

## 6. Next up (backlog, with rationale)

### Guiding principle

We build one **vertical write-slice at a time** (button → API → repository → DB → visible on reload),
ordered by **value ÷ effort**, and we **reuse the pattern** proven in Forms/Safety
(`DbConnectionFactory` + Repository) rather than inventing new plumbing each time.

Why this strategy:
- **Turn stubs into real actions before adding new surface.** Every 🟡 stub button is a promise to the
  user that currently lies. Making existing buttons real builds a trustworthy app faster than adding
  more half-working screens.
- **Cheap wins first keep momentum and de-risk the pattern.** Each repeat of the repository pattern is
  faster and confirms it generalizes, so by the time we hit a hard module (Warehouse) the mechanics are
  boring and reliable.
- **Defer the "big rocks" (Auth, real Reports) until the write path is proven.** They are large and
  touch everything; doing them last means they sit on a foundation that already works end-to-end.

> **All stub buttons are now real.** Every action in the app persists to the database. From here the
> work shifts from "make existing buttons real" to "add new capability and depth."

### ▶ Recommended next: Reports — real aggregation

> **What:** wire `ReportsPage` to the API and make `/reports/{module}` **aggregate** real data
> (totals, averages, trends) instead of re-wrapping the raw list.
> **Why it's next:** with every write feature done, the database now holds **real, changing data**
> (approvals, resolutions, generated recommendations, stock movements). Reports turn that data into
> insight — the natural next value once the data itself is real. It is also read-heavy (safer than more
> writes) and moves the app from "operate" to "understand." Reports on mock data would have been
> meaningless; now they aren't.

### Full backlog

| # | Feature | Why do it (rationale) | Effort |
|---|---|---|---|
| 1 | **Reports** for real | Wire `ReportsPage` to the API and **aggregate** (sums/trends) instead of re-wrapping lists. Meaningful now that there is real write data (approvals, movements) to summarize. | 🟡 Medium |
| 2 | **Refactor** SampleDataService | Move remaining inline read SQL into per-module repositories (Production/Cameras/etc.), matching the write repositories already built. Cleanup best done now that the pattern is established. | 🟡 Medium |
| 3 | **Auth / Login** | Use `roles`/`permissions` for login + role-based menus. A cross-cutting "big rock" that should sit on a **working, persisted** app — so it comes last. | 🔴 Large |

> Phase note: the "make every stub button real" phase is **done** — every action persists. The backlog
> now moves to **depth** (multi-table business rules in Warehouse), **insight** (real Reports), and
> **hardening** (repository refactor, then Auth). Keep the same rule of thumb: finish real capability on
> existing screens before adding new surface.

---

## 7. Changelog

| Date | Notes |
|---|---|
| 2026-07-21 | 🔵 **Migrated the whole backend from SQLite to SQL Server with EF Core code-first.** Added `SmartFactoryDbContext` (31 entities, snake_case, composite keys, 39 FKs, 10 unique indexes) + `InitialCreate` migration; connection to local `.\SQLEXPRESS` database `SmartFactory` (Windows Auth). Rewrote all 6 repositories and the read service (`SampleDataService`) onto the `DbContext`; startup now runs `Migrate()` + seeds `seed.sql`. Removed the SQLite layer (`DbConnectionFactory`, `Microsoft.Data.Sqlite`) and JSON fallback. Every completed feature (Forms, Safety, Notifications, Workforce, Warehouse, Cameras) re-verified end-to-end and durable across restart. |
| 2026-07-21 | ✅ **Cameras — Camera → Safety → Notification auto-alert**: added a Simulate-detection form and `POST /cameras/detect` (`CameraRepository`). A High/Critical detection with confidence ≥ 80% records a camera event **and** auto-raises a linked `safety_alert` (two-way linked via `safety_alert_camera_events` + `related_alert_id`) plus a Safety notification — one action now flows across three modules. Low-severity detections are only logged. |
| 2026-07-19 | ✅ **Warehouse inventory search + pagination** on the table; **auto-alerts** — a movement that turns an item Low Stock or Wrong Zone now inserts a Warehouse notification (only on status transition, no duplicates), visible on the Notifications page. |
| 2026-07-18 | ✅ **Warehouse full movement model**: added Transfer between zones, automatic recompute of item status (Low Stock / Correct / Wrong Zone) and zone usage+status after every movement, and per-item movement history (`GET /warehouse/zones`, `GET /warehouse/items/{id}/movements`). |
| 2026-07-17 | ✅ **Warehouse stock movement (Import/Export)** now persists (`WarehouseRepository`): a multi-table transaction that inserts `goods_movements` and adjusts `warehouse_items.quantity`, with validation (quantity > 0, no overselling). Added a movement form on the Warehouse page. |
| 2026-07-16 | ✅ **Workforce generate recommendations** now persists (`WorkforceRepository`): computes shift gaps and inserts `ai_recommendations` (idempotent — replaces prior auto-generated rows). Wired the header Generate button. **All stub buttons are now real.** |
| 2026-07-15 | ✅ **Notification mark-as-read** now persists to SQLite (`NotificationsRepository`, `status = Read`). Added a Mark-read button on the Notifications page. |
| 2026-07-14 | ✅ **Safety alert actions (resolve/escalate)** now persist to SQLite (`SafetyRepository`, with `action_note`). Added Resolve/Escalate buttons on the Safety page. Added the progress tracker (`PROGRESS.md` + `progress.html`). |
| 2026-07-13 | ✅ **Form approval flow (approve/reject)** persists to SQLite (repository + transaction). Moved the DB to the fixed location `C:\SmartFactoryData`. Added Approve/Reject buttons + `reload()`. |
| (earlier) | Project bootstrap: React frontend, .NET backend, 31-table schema, seed data, read endpoints, JSON fallback. |
