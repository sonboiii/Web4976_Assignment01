# CivicVote · Municipal Elections Management System

WA2 migrates the WA1 MVC application to a seven-project .NET 10 solution. It carries forward the four WA1 municipalities and their original candidate profiles, then adds Richmond. A new database contains five municipalities, fifteen positions and twenty-four fictional candidates.

## Run locally

Install the .NET 10 SDK, start Docker with Linux containers, and check that `docker buildx version` succeeds. Aspire uses Buildx to build the public image. On Arch Linux, the `docker-buildx` package supplies the plugin. Trust your ASP.NET development certificate with `dotnet dev-certs https --trust` on supported platforms. The first run downloads NuGet packages and the SQL Server, .NET SDK and nginx container images.

From the course workspace:

```sh
cd WA2
dotnet run --project MEMS.AppHost
```

On Linux, if the Aspire dashboard reports `UntrustedRoot` when connecting to the resource service, trust the development certificate and add its OpenSSL directory for the AppHost launch.

```sh
dotnet dev-certs https --trust
SSL_CERT_DIR="${SSL_CERT_DIR:-/etc/ssl/certs}:$HOME/.aspnet/dev-certs/trust" \
  dotnet run --project MEMS.AppHost
```

Open the Aspire dashboard link printed by AppHost. The **public** endpoint at `http://localhost:5180` redirects to the municipality directory at `https://localhost:7182/`. Try `/burnaby`, `/surrey`, `/Richmond`, or `/pitt-meadows`. Open the **admin** HTTPS endpoint at `https://localhost:7182/admin`. The API uses `http://localhost:5181`. Aspire supplies the database connection and service addresses. SQL Server stores its data in a named volume.

### Development accounts

| Username | Access |
| --- | --- |
| `super` | All tenants, municipality creation and deletion, administrator accounts |
| `burnabyadmin` | Burnaby |
| `surreyadmin` | Surrey |
| `richmondadmin` | Richmond |
| `pm` | Pitt Meadows |

The local development password is `P@$$w0rd`. Identity accepts case-insensitive usernames. The API applies migrations before serving requests and seeds demo accounts on an empty Identity store in Development. To set the initial account password explicitly, configure `Seed__Password` on the API before first startup. Outside Development, creating the initial accounts requires this setting.

### Try the assignment workflows

1. Open Pitt Meadows. Filter by Mayor, compare Amelia Chen and Daniel Brooks, and add one to your guide. Choosing the second mayor requires removing the first. Visit Surrey, add another candidate, then reload the tab and open **Voting guide** at `/guide`. The printout groups both cities’ candidates by municipality and office and includes each election date and the print date.
2. Visit another municipality in the same tab. Each guide has its own session storage key. In browser developer tools, inspect Session Storage and watch the Network panel while adding or removing a choice. A choice changes only browser state. Local Storage holds the theme preference.
3. Sign in as `pm`. The dashboard and public directory show only Pitt Meadows. Open its administration workspace to manage its details, positions and candidates. Public profiles omit the guide and comparison controls while signed in. Open a saved record’s details to upload its logo or photograph.
4. Sign in as `super`. Add a municipality with a unique slug, colour and election date. Create positions and candidates and visit the new slug on the public site. The Administrators page assigns a new manager to exactly one municipality.
5. In Aspire, inspect resource health, open the admin dashboard, and follow the resulting Admin → API requests in **Traces**. **Structured logs** includes the API database initialization event. The API readiness check connects to SQL Server. The public container exposes a readiness check for its static web server.

## Solution

| Project | Responsibility |
| --- | --- |
| MEMS.AppHost | Starts SQL Server, the API, the Admin site and the published public WASM container |
| MEMS.ServiceDefaults | Service discovery, OpenTelemetry logging, metrics, tracing, health checks and exception handling |
| MEMS.Admin | Interactive Blazor Server components, validated forms and protected administrator sign-in |
| MEMS.Public | Standalone Blazor WebAssembly directory, comparisons, session-only voting guide and print layout |
| MEMS.Api | ASP.NET Core Web API for public reads, Identity authentication and authorized administration |
| MEMS.Data | SQL Server EF Core context, migrations, Identity entities, tenant-scoped queries and seed data |
| MEMS.Shared | Client/server contracts, validation, roles and image format checks |

The public container builds MEMS.Public and serves its static WASM output through nginx. A shared HTTPS gateway serves those assets and relays public API reads to the endpoints supplied by Aspire. This lets both apps recognize the same administrator session. The browser receives neither database credentials nor an administrator token. The admin server signs in through the Identity-backed API and uses a protected HTTP-only cookie to carry its session. The API rechecks account membership when it receives a bearer token.

## Verification

```sh
dotnet build MEMS.slnx
node --test tests/storage.test.cjs
dotnet run --file tests/GuideChecks.cs
# With AppHost running and the development accounts seeded
python3 tests/api_smoke.py
```

The API smoke test exercises tenant scoping, validation and the full create/edit/delete lifecycle using temporary records. See [API documentation](docs/API.md) and the [WA1 deficiency report](docs/Deficiencies.md).
