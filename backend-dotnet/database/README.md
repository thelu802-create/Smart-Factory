# Smart Factory AI Dashboard Sample Database

This folder contains sample data for the Smart Factory AI Dashboard project.

## Files

1. `schema.sql`: relational database schema for the MVP entities.
2. `seed.sql`: sample SQL data for demo and development.
3. `sample-data.json`: structured JSON sample data that can be used by frontend mock services or backend APIs.

## Suggested Usage

For the first MVP, the React frontend can fall back to local mock data when the backend is not running. When the backend is running, the C# MVC API reads SQLite first and falls back to JSON sample data if SQLite cannot be opened.

The current C# backend uses these files through:

```text
backend-dotnet/Services/SampleDataService.cs
```

At runtime, `smart_factory_demo.db` is created from `schema.sql` and `seed.sql` if the database file does not exist yet. `sample-data.json` remains the fallback data source.

Recommended database options:

1. SQLite for local demo.
2. PostgreSQL for a more realistic deployment.

## Main Data Areas

1. Users and roles.
2. Factory areas.
3. Production lines and records.
4. Machines and machine issues.
5. Warehouse zones, items, and movements.
6. Safety alerts and camera events.
7. Employees and shift plans.
8. Form requests.
9. Notifications.
10. AI recommendations.
11. Report snapshots.

## Next Backend Step

The backend can initialize SQLite automatically from:

```text
backend-dotnet/database/schema.sql
backend-dotnet/database/seed.sql
```

and creates a local SQLite database file:

```text
backend-dotnet/database/smart_factory_demo.db
```

For the full design, see:

```text
docs/database/smart-factory-ai-dashboard-database-design.md
```