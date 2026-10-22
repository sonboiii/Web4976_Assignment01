# Web4976_Assignment01

CivicVote municipal elections app, adapted from W-mems into WA1's standard ASP.NET Core MVC structure. Uses .NET 10, EF Core, SQLite, Identity, and Bootstrap.

## Run

```bash
dotnet run --project Web4976_Assignment01.csproj --launch-profile http
```

Open http://localhost:5159. WA1 includes the W-mems database in `app.db`. Startup applies migrations; Development also seeds the demo if the database is empty and `SeedDemo` is enabled in `appsettings.Development.json`.

| Username | Password | Access |
| --- | --- | --- |
| `super` | `P@$$w0rd` | All municipalities and administrator accounts |
| `pm` | `P@$$w0rd` | Pitt Meadows positions and candidates |

Public users can browse and filter candidates, compare up to three profiles, and save a seat-limited voting guide to print without an account. The theme toggle remembers your preference. The demo includes four municipalities, twelve positions, and twenty-one fictional candidates. October 17, 2026 is the sample election date.

## Project structure

- `Controllers/`: public directory, profiles, comparisons, guide, and account actions.
- `Models/`: entities and roles; `Models/ViewModels/` contains view and form models.
- `Data/`: application database context and development seed data.
- `Data/Migrations/`: initial election schema, frozen migration metadata, and model snapshot.
- `Services/`: municipality authorization scope, identity claims, and validated image storage.
- `Views/`: public views grouped by controller, with layout and error messaging in `Views/Shared/`.
- `Areas/Admin/Controllers/` and `Areas/Admin/Views/`: scoped administration, with common list, details, and delete views in `Views/Shared/` within the area.
- `wwwroot/css/`, `wwwroot/js/`, and `wwwroot/lib/`: original W-mems styles, scripts, and bundled libraries in WA1's asset folders.
- `wwwroot/portraits.svg`: demo candidate portraits.
- `Properties/launchSettings.json`: WA1's HTTP and HTTPS launch profiles.

Routes and page behavior match W-mems, including administration under `/Admin`. Razor views use conventional MVC discovery.

## Configuration

The SQLite connection is `ConnectionStrings:DefaultConnection` in `appsettings.json`, defaulting to `Data Source=app.db;Foreign Keys=True`. Environment variables and user secrets can override it. Development demo seeding is controlled by `SeedDemo`.

Data protection keys are generated in the ignored `App_Data/keys/` directory. Uploaded JPG, PNG, and WebP files, up to 2 MB, are stored in the ignored `wwwroot/uploads/` directory. Guides use browser sessions with an eight-hour idle timeout and may be lost on server restart.

## Build and migrations

```bash
dotnet build Web4976_Assignment01.csproj
dotnet tool restore
dotnet ef migrations add YourChange --project . --output-dir Data/Migrations
```

The initial migration and snapshot share frozen `InitialSchema` metadata. New migrations must leave that historical schema unchanged.
