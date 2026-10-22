---
name: add-ef-migration
description: Safely change the data model and add an EF Core migration without altering the frozen initial schema. Use when adding or changing entity properties, relationships, or constraints.
---

# Add an EF Core migration

1. Change the entity in `Models/` and, if needed, its configuration in `ApplicationDbContext.OnModelCreating`
   (unique indexes, max lengths, restricted delete behaviour).
2. Update any affected view models, validation attributes, admin views and seed data.
3. Build first: `dotnet build Web4976_Assignment01.csproj`.
4. Create the migration:
   `dotnet tool restore`
   `dotnet ef migrations add <DescriptiveName> --project . --output-dir Data/Migrations`
5. Review the generated migration:
   - It must not modify the frozen `InitialSchema` migration or its metadata.
   - Check that no domain foreign key became cascade delete.
   - Check for unintended column drops (data loss).
6. Run the app locally; migrations apply on startup. Confirm the seed still runs on an empty database.
7. Commit the model, migration and snapshot together. Never commit `app.db` changes or `appsettings.Development.json`.
