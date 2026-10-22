# Copilot Instructions — CivicVote (Web4976_Assignment01)

Municipal Elections Management System. ASP.NET Core MVC on .NET 10, EF Core (Code First) with SQLite, ASP.NET Core Identity, Bootstrap 5.

## Project structure
- `Controllers/` — public site (directory, profiles, comparison, voting guide, account).
- `Areas/Admin/` — administration controllers and views, served under `/Admin`.
- `Models/` — entities (`Municipality`, `Position`, `Candidate`, `ApplicationUser`); `Models/ViewModels/` for view and form models.
- `Data/` — `ApplicationDbContext`, seed data; `Data/Migrations/` for EF migrations.
- `Services/` — municipality authorization scope, identity claims, image storage.

## Domain rules
- A Municipality has many Positions; a Position has many Candidates. Candidates reach their municipality through `Position`.
- Municipality `Name` is unique. Position `(MunicipalityId, Name)` is unique. `SeatsAvailable` is 1–100.
- Domain foreign keys use restricted delete. Do not switch them to cascade.
- `Candidate.PhotoUrl` is computed in C# and ignored by EF. Only `PhotoFileName` is stored.

## Security
- Public users never log in. Public registration is disabled; do not re-enable it.
- Login is by username (`super`, `pm`), not email.
- `ApplicationUser.MunicipalityId` is null for Super Admins and set for municipality admins.
- Every admin action (Index, Details, Create, Edit, Delete, and their POSTs) must apply the municipality scope service. Never trust an id from the route without checking it belongs to the user's municipality.
- Super Admin only: creating/deleting municipalities and managing admin accounts.

## Conventions
- Use async EF Core calls and `[ValidateAntiForgeryToken]` on every POST.
- Use data annotations for validation and include `_ValidationScriptsPartial` on forms, so validation runs on client and server.
- Use view models for forms; do not bind entities directly.
- Image uploads go through the image storage service: JPG, PNG or WebP, max 2 MB, saved under `wwwroot/uploads/`.
- The voting guide is stored in session and limited by each position's seats.
- Use Bootstrap utility classes, keep layouts mobile-first, and support the light/dark theme toggle (`data-bs-theme`).

## Configuration
- The connection string belongs in `appsettings.Development.json` (git-ignored) locally and in Azure App Service configuration in production. Never commit secrets.
- Migrations are applied on startup.
