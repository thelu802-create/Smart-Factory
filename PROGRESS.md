# Smart Factory AI Dashboard — Theo Dõi Tiến Độ

> File tổng hợp tiến độ phát triển để cả nhóm theo dõi được app đang làm tới đâu,
> mô tả theo **góc nhìn người dùng trên giao diện**. Cập nhật mỗi khi hoàn thành một chức năng.

**Cập nhật lần cuối:** 2026-07-14

---

## 1. Chú thích trạng thái

| Ký hiệu | Ý nghĩa |
|---|---|
| ✅ **Hoàn chỉnh** | Chạy thật end-to-end, thao tác **ghi và lưu** xuống database (SQLite trên ổ cứng) |
| 🟢 **Xem được (read-only)** | Hiển thị dữ liệu thật từ database, nhưng **chưa có thao tác ghi** |
| 🟡 **Nút giả (stub)** | Có nút/endpoint nhưng bấm vào **không lưu** — trả kết quả cứng, chỉ để demo |
| ⚪ **Dữ liệu mẫu (mock)** | Trang chỉ dùng dữ liệu mẫu cứng trong frontend, **chưa nối API** |
| ⬜ **Chưa làm** | Chưa bắt đầu |

---

## 2. Tổng quan hệ thống

- **Frontend:** React + Vite + TypeScript — chạy ở `http://localhost:5173`
- **Backend:** C# ASP.NET Core Web API — chạy ở `http://localhost:8000`
- **Database:** SQLite (file thật trên ổ cứng tại `C:\SmartFactoryData\smart_factory_demo.db`), tự seed từ `schema.sql` + `seed.sql`; có fallback JSON khi không mở được DB.

Cách chạy chi tiết: xem [README.md](README.md) và [backend-dotnet/README.md](backend-dotnet/README.md).
Cách chạy nhanh để test:
1. Backend: mở `backend-dotnet` → chạy profile **`SmartFactory.Api`** (VS) hoặc `dotnet run --urls http://localhost:8000`.
2. Frontend: mở `frontend` → `npm run dev`.
3. Mở `http://localhost:5173`.

> ⚠️ Phải chạy **backend trước**. Nếu backend tắt hoặc chạy sai cổng, các trang sẽ tự rơi về dữ liệu mẫu và các nút thao tác sẽ báo "Failed to fetch".

---

## 3. Tiến độ theo chức năng (trang giao diện)

| Trang (menu) | Người dùng nhìn thấy / làm được gì | Trạng thái | Ghi chú |
|---|---|---|---|
| **Dashboard** | KPI tổng (sản lượng, % hoàn thành mục tiêu, số line đang chạy, cảnh báo an toàn) + tóm tắt | 🟢 Xem được | KPI tính thật từ dữ liệu line & cảnh báo |
| **Production** | Bảng các dây chuyền: trạng thái, sản lượng, hiệu suất, tỉ lệ lỗi, downtime; xem chi tiết 1 line | 🟢 Xem được | Chưa có thao tác cập nhật line |
| **Warehouse** | Bảng tồn kho theo BU / IO / mã hàng / lô; xem chi tiết 1 item | 🟢 Xem được | Chưa làm nhập/xuất/chuyển kho (goods movement) |
| **Safety** | Danh sách cảnh báo an toàn; xem chi tiết; nút **Resolve / Escalate** | ✅ **Hoàn chỉnh** | **Bấm resolve/escalate lưu thật (kèm action_note), giữ nguyên sau restart** |
| **Cameras** | Nhật ký sự kiện camera AI (loại sự kiện, độ tin cậy, thời gian) | 🟢 Xem được | — |
| **Workforce** | Kế hoạch ca theo dây chuyền + đề xuất AI; nút **tạo đề xuất** | 🟡 Nút giả | Xem chạy thật; nút generate **chưa lưu** |
| **Forms** | Hàng chờ duyệt biểu mẫu; nút **Approve / Reject** | ✅ **Hoàn chỉnh** | **Bấm duyệt/từ chối lưu thật xuống DB, giữ nguyên sau khi restart** |
| **Notifications** | Danh sách thông báo (an toàn, sản xuất, kho, biểu mẫu); đánh dấu **đã đọc** | 🟡 Nút giả | Xem chạy thật; đánh dấu đã đọc **chưa lưu** |
| **Reports** | Trang báo cáo theo module | ⚪ Dữ liệu mẫu | Backend có endpoint nhưng frontend chưa nối; chưa tổng hợp số liệu thật |
| **Analytics** | Biểu đồ hiệu suất suy ra từ dữ liệu sản xuất | 🟢 Xem được | Đọc dữ liệu thật, biểu đồ dẫn xuất |
| **Settings** | Trang cấu hình | ⚪ Dữ liệu mẫu | Tĩnh, chưa có logic |

**Tóm tắt:** 2 chức năng ghi hoàn chỉnh (Forms, Safety), 5 trang xem dữ liệu thật, 2 nút thao tác còn là stub, 2 trang còn mock.

---

## 4. Trạng thái API (backend)

| Endpoint | Chức năng | Trạng thái |
|---|---|---|
| `GET /health` | Kiểm tra sống + nguồn dữ liệu (sqlite / json) | ✅ |
| `GET /dashboard/summary`, `/dashboard/kpis`, `/dashboard/alerts` | Số liệu tổng quan | 🟢 Đọc thật |
| `GET /production/lines`, `/production/lines/{id}` | Dây chuyền + chi tiết | 🟢 Đọc thật |
| `GET /warehouse/items`, `/warehouse/items/{id}` | Tồn kho + chi tiết | 🟢 Đọc thật |
| `GET /safety/alerts`, `/safety/alerts/{id}` | Cảnh báo an toàn + chi tiết | 🟢 Đọc thật |
| `POST /safety/alerts/{id}/resolve`, `/escalate` | Xử lý / leo thang cảnh báo | ✅ Ghi thật (status + action_note) |
| `GET /cameras/events` | Sự kiện camera | 🟢 Đọc thật |
| `GET /workforce/shifts`, `/workforce/recommendations` | Ca làm + đề xuất | 🟢 Đọc thật |
| `POST /workforce/recommendations/generate` | Tạo đề xuất | 🟡 Stub (chưa lưu) |
| `GET /forms` | Danh sách biểu mẫu | 🟢 Đọc thật (live) |
| `POST /forms/{id}/approve`, `/reject` | **Duyệt / từ chối biểu mẫu** | ✅ **Ghi thật (transaction)** |
| `GET /notifications` | Danh sách thông báo | 🟢 Đọc thật |
| `POST /notifications/{id}/read` | Đánh dấu đã đọc | 🟡 Stub (chưa lưu) |
| `GET /reports/{module}` | Báo cáo theo module | 🟡 Chỉ gói lại danh sách, chưa tổng hợp |

---

## 5. Hạ tầng / nền tảng đã có

- 🔵 **Database SQLite trên ổ cứng** — vị trí cố định `C:\SmartFactoryData\`, tự tạo thư mục + seed lần đầu, giữ dữ liệu qua các lần restart.
- 🔵 **Schema đầy đủ 31 bảng** (`schema.sql`) khớp với tài liệu thiết kế, gồm cả các bảng quan hệ enterprise (permissions, io_masters, form_approval_steps, ...).
- 🔵 **Fallback JSON** — nếu không mở được SQLite, API vẫn trả dữ liệu mẫu từ `sample-data.json` để frontend không chết.
- 🔵 **Repository pattern (mới)** — `DbConnectionFactory` + `FormsRepository`: khuôn mẫu đọc live + ghi có transaction, dùng lại được cho các module khác.
- 🔵 **CORS** cho phép frontend localhost gọi API.
- 🔵 **Frontend layer** — `factoryApi` (gọi API), `useApiData` (tự fallback mock + `reload()` sau thao tác), component UI dùng lại (KpiCard, Panel, StatusBadge, PageHeader).

---

## 6. Việc tiếp theo (backlog, theo thứ tự ưu tiên)

Áp dụng đúng khuôn mẫu đã tạo ở luồng Forms/Safety (`DbConnectionFactory` + Repository):

1. **Notifications** `read` — cập nhật `notifications.status = Read`. (Nhanh nhất tiếp theo.)
2. **Workforce** `recommendations/generate` — sinh + lưu `ai_recommendations`.
3. **Warehouse** nhập/xuất/chuyển kho — ghi `goods_movements` + đổi `zone_id`/`quantity`. (Phức tạp hơn.)
4. **Reports** thật — nối `ReportsPage` vào API, tổng hợp số liệu thay vì gói lại danh sách.
5. **Refactor** — tách dần SQL trong `SampleDataService` ra các repository theo module.
6. **Auth/Login** — dùng bảng roles/permissions để đăng nhập + ẩn/hiện menu theo vai trò.

---

## 7. Nhật ký (changelog)

| Ngày | Nội dung |
|---|---|
| 2026-07-14 | ✅ Luồng **xử lý cảnh báo an toàn (Safety resolve/escalate)** ghi thật xuống SQLite (`SafetyRepository`, kèm `action_note`). Thêm nút Resolve/Escalate trên trang Safety. Thêm trang theo dõi tiến độ (`PROGRESS.md` + `progress.html`). |
| 2026-07-13 | ✅ Luồng **duyệt biểu mẫu (Forms approve/reject)** ghi thật xuống SQLite (repository + transaction). Chuyển DB sang vị trí cố định `C:\SmartFactoryData`. Thêm nút Approve/Reject + `reload()`. |
| (trước đó) | Khởi tạo dự án: frontend React, backend .NET, schema 31 bảng, seed, các endpoint đọc, fallback JSON. |
