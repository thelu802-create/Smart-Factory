# Smart Factory AI Dashboard

Smart Factory AI Dashboard is an MVP web application for monitoring production, warehouse, safety, workforce, forms, notifications, reports, and AI recommendations.

## Project Structure

```text
app/       Legacy static HTML/CSS/JS prototype
backend-dotnet/ ASP.NET Core MVC API, database models, sample data, SQL schema, and seed data
docs/      Product requirements, sitemap, user flows, and diagrams
frontend/  React + Vite + TypeScript dashboard application
```

## Current Setup Status

Frontend setup is ready. The active backend is the C# ASP.NET Core MVC API in `backend-dotnet`.

## Frontend Setup

Run from the project root:

```powershell
Set-Location frontend
npm install
npm run dev -- --host 0.0.0.0
```

Open:

```text
http://localhost:5173/
```

Build check:

```powershell
Set-Location frontend
npm run build
```

## Backend Setup With C#

This machine has .NET available, so the practical backend path is `backend-dotnet`.

```powershell
Set-Location backend-dotnet
dotnet restore
dotnet run --urls http://localhost:8000
```

Backend health check:

```text
http://localhost:8000/health
```

The C# backend reads SQLite first from `backend-dotnet/database/smart_factory_demo.db`. If SQLite cannot be opened, it falls back to `backend-dotnet/database/sample-data.json` and still exposes endpoint groups for the React frontend.

## Backend Data Source

Primary local database:

```text
backend-dotnet/database/smart_factory_demo.db
```

JSON fallback file:

```text
backend-dotnet/database/sample-data.json
```

The database setup files are:

```text
backend-dotnet/database/schema.sql
backend-dotnet/database/seed.sql
backend-dotnet/database/smart_factory_demo.db
```

`smart_factory_demo.db` is generated locally by the C# backend when needed and should not be committed.

Check the active backend data source at:

```text
http://localhost:8000/health
```

## Recommended Development Order

1. Run the frontend and review the dashboard pages.
2. Run the C# backend API from Visual Studio or `dotnet run`.
3. Verify frontend API services against the ASP.NET Core endpoints.
4. Expand SQLite repository/query coverage or migrate to PostgreSQL when needed.