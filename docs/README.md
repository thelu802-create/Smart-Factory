# Smart Factory AI Dashboard Documentation

This folder is organized by documentation purpose so the project is easier to browse.

## Current Implementation Stack

```text
Frontend: React + Vite + TypeScript
Backend:  C# ASP.NET Core MVC / Web API
Database: SQLite first, JSON fallback for demo safety
Solution: backend-dotnet/SmartFactoryDashboard.sln
```

## Folder Structure

```text
docs/
  overview/      Product idea and concept overview
  requirements/  Implementation requirements and acceptance criteria
  flows/         User flows, activity diagrams, and sequence diagrams
  sitemap/       Text sitemap and visual sitemap
  database/      Database design, ERD, schema explanation, and visual DB diagram
```

## Main Documents

### Development Progress (living status)

The live progress tracker is kept at the repository root (updated whenever a feature is completed):

- [../PROGRESS.md](../PROGRESS.md) — feature/module status, API status, backlog, and changelog.
- [../progress.html](../progress.html) — a visual, auto-refreshing view of `PROGRESS.md` (open via a static server / VS Code Live Server).

### Overview

- [overview/smart-factory-ai-dashboard-document.md](overview/smart-factory-ai-dashboard-document.md)

### Requirements

- [requirements/smart-factory-ai-dashboard-requirements.md](requirements/smart-factory-ai-dashboard-requirements.md)

### Flows

- [flows/smart-factory-ai-dashboard-user-flows.md](flows/smart-factory-ai-dashboard-user-flows.md)
- [flows/smart-factory-ai-dashboard-activity-sequence-diagrams.html](flows/smart-factory-ai-dashboard-activity-sequence-diagrams.html)

### Sitemap

- [sitemap/smart-factory-ai-dashboard-sitemap.md](sitemap/smart-factory-ai-dashboard-sitemap.md)
- [sitemap/smart-factory-ai-dashboard-sitemap.html](sitemap/smart-factory-ai-dashboard-sitemap.html)

### Database

- [database/smart-factory-ai-dashboard-database-design.md](database/smart-factory-ai-dashboard-database-design.md)
- [database/smart-factory-ai-dashboard-database-erd.md](database/smart-factory-ai-dashboard-database-erd.md)
- [database/smart-factory-ai-dashboard-database-erd.html](database/smart-factory-ai-dashboard-database-erd.html)

## Recommended Reading Order

1. Read the overview document.
2. Review the requirements document.
3. Review user flows and activity/sequence diagrams.
4. Review sitemap.
5. Review database design and ERD.