# CivicVote · Municipal Elections Management System

## Setup

- Install the .NET 10 SDK.
- Start Docker in Linux container mode.
- Check that `docker buildx version` works. On Arch Linux, install `docker-buildx` if needed.

From workspace, trust the local HTTPS certificate and start the app:

```sh
cd WA2
dotnet dev-certs https --trust
dotnet run --project MEMS.AppHost
```

If the Aspire dashboard reports `UntrustedRoot` on Linux after you trust the certificate, start AppHost with:

```sh
SSL_CERT_DIR="${SSL_CERT_DIR:-/etc/ssl/certs}:$HOME/.aspnet/dev-certs/trust" \
  dotnet run --project MEMS.AppHost
```

## Development accounts

The default local development password is `P@$$w0rd` for all seeded accounts. To set a different initial password, configure `Seed__Password` on the API before first startup.

| Username | Access |
| --- | --- |
| `super` | All municipalities and administrator account management |
| `burnabyadmin` | Burnaby |
| `surreyadmin` | Surrey |
| `richmondadmin` | Richmond |
| `pm` | Pitt Meadows |

## Solution and project layout


| Project | Responsibility |
| --- | --- |
| `MEMS.AppHost` | Starts SQL Server, the API, the admin site, and the public site container |
| `MEMS.ServiceDefaults` | Service discovery, telemetry, health checks, and exception handling |
| `MEMS.Admin` | Blazor Server administration site and sign-in |
| `MEMS.Public` | Blazor WebAssembly municipality directory and voting guide |
| `MEMS.Api` | ASP.NET Core Web API for public reads and authorized administration |
| `MEMS.Data` | EF Core database context, migrations, Identity, and seed data |
| `MEMS.Shared` | Shared API contracts, validation, and roles |

