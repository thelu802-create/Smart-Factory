# Smart Factory AI Dashboard .NET API

This ASP.NET Core backend is the active API for the Smart Factory AI Dashboard. It uses SQLite as the primary local database and falls back to JSON sample data if the database cannot be opened.

Primary local database:

```text
database/smart_factory_demo.db
```

JSON fallback file:

```text
database/sample-data.json
```

## Run

```powershell
Set-Location backend-dotnet
dotnet restore
dotnet run --urls http://localhost:8000
```

## Run With Visual Studio

Open this solution file:

```text
backend-dotnet/SmartFactoryDashboard.sln
```

Then check these settings:

1. Set `SmartFactory.Api` as the startup project.
2. Select the `SmartFactory.Api` launch profile.
3. Press `F5` or click the green Run button.
4. The browser should open `http://localhost:8000/health`.

If Visual Studio cannot run, close any old solution opened from the previous root path and reopen the solution above. Also make sure port `8000` is not already used by another backend process.

Open:

```text
http://localhost:8000/health
```

The health response includes the active data source:

```json
{
	"status": "ok",
	"dataSource": {
		"source": "sqlite",
		"fallbackReason": null
	}
}
```

If SQLite is unavailable, `source` becomes `json-fallback` and the API still returns demo data from `database/sample-data.json`.

## Data Loading

`SampleDataService` loads data in this order:

1. Open SQLite from `ConnectionStrings:SmartFactoryDatabase`.
2. If the SQLite file does not exist or has no tables, create it from `database/schema.sql` and `database/seed.sql`.
3. If SQLite connection, initialization, or query fails, read `database/sample-data.json` instead.

The default connection string is configured in `appsettings.json`:

```json
{
	"ConnectionStrings": {
		"SmartFactoryDatabase": "Data Source=database/smart_factory_demo.db"
	}
}
```

## MVC Folder Structure

```text
Controllers/  HTTP endpoints grouped by feature module
Models/       Source data models and response DTOs
Models/Database/ Database entity models mapped from the SQL design
Models/Requests/ API request payload models
database/     JSON sample data, SQL schema, and SQL seed data
Services/     Data loading and mapping logic
Program.cs    Application startup, DI, CORS, and controller routing
```

`Program.cs` only configures the app and calls `MapControllers()`. Feature routes are handled in dedicated controllers such as `WarehouseController`, `ProductionController`, and `DashboardController`.

## Endpoint Groups

- `/dashboard/*`
- `/production/*`
- `/warehouse/*`
- `/safety/*`
- `/cameras/*`
- `/workforce/*`
- `/forms/*`
- `/notifications/*`
- `/reports/*`