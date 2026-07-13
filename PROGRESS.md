# Smart Factory AI Dashboard — Development Progress

> A single file for the whole team to track how far the app has come,
> described from the **end-user (UI) perspective**. Update it whenever a feature is completed.

**Last updated:** 2026-07-14

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
- **Database:** SQLite (a real file on disk at `C:\SmartFactoryData\smart_factory_demo.db`), auto-seeded from `schema.sql` + `seed.sql`; falls back to JSON when the DB cannot be opened.

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
| **Warehouse** | Inventory table by BU / IO / item code / batch; view one item's detail | 🟢 Viewable | Goods movement (in/out/transfer) not built yet |
| **Safety** | Safety alert list; view detail; **Resolve / Escalate** buttons | ✅ **Complete** | **Resolve/escalate persists (with action_note), survives restart** |
| **Cameras** | AI camera event log (event type, confidence, time) | 🟢 Viewable | — |
| **Workforce** | Shift plan by line + AI recommendations; **generate** button | 🟡 Stub button | Viewing works; the generate button **does not persist** |
| **Forms** | Approval queue for forms; **Approve / Reject** buttons | ✅ **Complete** | **Approve/reject persists to the DB and stays after a restart** |
| **Notifications** | Notification list (safety, production, warehouse, forms); mark as **read** | 🟡 Stub button | Viewing works; mark-as-read **does not persist** |
| **Reports** | Reporting page by module | ⚪ Mock data | Backend endpoints exist but the frontend is not wired; no real aggregation |
| **Analytics** | Performance charts derived from production data | 🟢 Viewable | Reads real data, derived charts |
| **Settings** | Configuration page | ⚪ Mock data | Static, no logic yet |

**Summary:** 2 complete write features (Forms, Safety), 5 pages showing real data, 2 action buttons still stubs, 2 pages still mock.

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
| `GET /cameras/events` | Camera events | 🟢 Real read |
| `GET /workforce/shifts`, `/workforce/recommendations` | Shifts + recommendations | 🟢 Real read |
| `POST /workforce/recommendations/generate` | Generate recommendations | 🟡 Stub (no persist) |
| `GET /forms` | Form list | 🟢 Real read (live) |
| `POST /forms/{id}/approve`, `/reject` | **Approve / reject form** | ✅ **Real write (transaction)** |
| `GET /notifications` | Notification list | 🟢 Real read |
| `POST /notifications/{id}/read` | Mark as read | 🟡 Stub (no persist) |
| `GET /reports/{module}` | Report by module | 🟡 Only re-wraps a list, no aggregation |

---

## 5. Infrastructure / foundations in place

- 🔵 **On-disk SQLite database** — fixed location `C:\SmartFactoryData\`, auto-creates the folder + seeds on first run, keeps data across restarts.
- 🔵 **Full 31-table schema** (`schema.sql`) matching the design docs, including the enterprise relationship tables (permissions, io_masters, form_approval_steps, ...).
- 🔵 **JSON fallback** — if SQLite cannot be opened, the API still returns sample data from `sample-data.json` so the frontend keeps working.
- 🔵 **Repository pattern** — `DbConnectionFactory` + `FormsRepository` + `SafetyRepository`: a reusable live-read + write template for other modules.
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

### ▶ Recommended next: Notifications — mark as read

> **What:** `POST /notifications/{id}/read` updates `notifications.status = 'Read'`; the bell/list
> reflects it and it survives a restart.
> **Why it's next:** it is the **cheapest possible** repeat of the exact Forms/Safety pattern — a single
> `UPDATE` of one column, no joins, no new concepts. High confidence, ~30 minutes, and it clears one of
> the two remaining stub buttons. It also unlocks the natural follow-up of an **unread badge** in the
> top bar, which makes the whole app feel live.

### Full backlog

| # | Feature | Why do it (rationale) | Effort |
|---|---|---|---|
| 1 | **Notifications** `read` | Cheapest reuse of the proven pattern (one-column `UPDATE`); removes a stub; enables an unread badge. | 🟢 Small |
| 2 | **Workforce** `generate` | Clears the last stub button. Introduces **insert + rule logic** (create `ai_recommendations` rows), a small step up from pure updates. | 🟡 Medium |
| 3 | **Warehouse** in/out/transfer | Highest operational value but the hardest write: **multi-table transaction** (insert `goods_movements` + adjust `warehouse_items.quantity`/`zone_id`) with validation. Best done once the pattern is second nature. | 🔴 Large |
| 4 | **Reports** for real | Wire `ReportsPage` to the API and **aggregate** (sums/trends) instead of re-wrapping lists. Depends on having real write data to make reports meaningful, so it naturally comes after the write features. | 🟡 Medium |
| 5 | **Refactor** SampleDataService | Move remaining inline SQL into per-module repositories. Pure cleanup — do it **after** enough modules exist to reveal the right shared abstractions, not before. | 🟡 Medium |
| 6 | **Auth / Login** | Use `roles`/`permissions` for login + role-based menus. A cross-cutting "big rock" that should sit on a **working, persisted** app — so it comes last. | 🔴 Large |

> Rule of thumb for reordering: if a stub button exists, prefer making it real (items 1–3) before
> building new pages or infrastructure (items 4–6). A user trusts an app where **every button does what
> it says** far more than one with more screens that don't.

---

## 7. Changelog

| Date | Notes |
|---|---|
| 2026-07-14 | ✅ **Safety alert actions (resolve/escalate)** now persist to SQLite (`SafetyRepository`, with `action_note`). Added Resolve/Escalate buttons on the Safety page. Added the progress tracker (`PROGRESS.md` + `progress.html`). |
| 2026-07-13 | ✅ **Form approval flow (approve/reject)** persists to SQLite (repository + transaction). Moved the DB to the fixed location `C:\SmartFactoryData`. Added Approve/Reject buttons + `reload()`. |
| (earlier) | Project bootstrap: React frontend, .NET backend, 31-table schema, seed data, read endpoints, JSON fallback. |
