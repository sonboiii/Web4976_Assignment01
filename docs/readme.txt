COMP4976 - Assignment 1
Municipal Elections Management System (CivicVote)
=================================================

TEAM MEMBERS
------------
- Son Bui (A01450344)
- Larry Lin ([A01461078])


LINKS
-----
Azure URL:   https://web4976-a1-gvasedgrajewcgfn.westus3-01.azurewebsites.net/
GitHub repo: https://github.com/sonboiii/Web4976_Assignment01


CONTRIBUTIONS
-------------
Work was split roughly evenly: one member led the data, security and admin
side, the other led the public site and deployment. Both members tested the
full application and fixed bugs.
 
[Son Bui]
- Set up the GitHub repository, .gitignore and project configuration.
- Created the Azure App Service and GitHub Actions deploy workflow, configured
  the production SQLite connection string and startup migrations.
- Built the public site: municipality and position browsing, candidate
  directory with search and filters, and candidate profile pages.
- Built the voting guide (session-based, seat-limited) and printable guide.
- Implemented bonus features: candidate comparison and dark mode.
- Produced the ERD and docs/readme.txt.
 
[Larry Lin]
- Designed the entity models, DbContext and EF Core Code First migrations.
- Wrote the seed data: municipalities, positions, candidates, roles and users.
- Configured ASP.NET Core Identity with username login, disabled public
  registration, and implemented municipality-scoped admin authorization.
- Built the Admin area: dashboard and CRUD for municipalities, positions and
  candidates, with validated logo and photo uploads and client/server validation.
- Implemented bonus features: pagination, candidate social and video links,
  and mobile-first responsive layout.
- Wrote the GitHub Copilot instruction, prompt and skill files.


WHAT HAS NOT BEEN COMPLETED
---------------------------
- All required features have been completed.


MAJOR CHALLENGES
----------------
- Restricting municipality admins to their own municipality on every admin
  action, including direct URL access to Edit and Delete pages. Solved with a
  nullable MunicipalityId on ApplicationUser and a shared municipality scope
  service used by all admin controllers.

- Azure deployment: the first GitHub Actions deploy failed because the new
  managed identity did not yet have access to the subscription. We also had
  to configure the SQLite path and apply migrations automatically on startup.

TESTING INSTRUCTIONS
--------------------
Seeded accounts:

  Username   Password    Access
  super      P@$$w0rd    Super Admin - all municipalities and admin accounts
  pm         P@$$w0rd    Pitt Meadows Admin - Pitt Meadows positions and candidates only

Seed data: 4 municipalities, 12 positions, 21 fictional candidates.
Sample election date: October 17, 2026.

Public features (no login required):
  1. Browse municipalities and positions, open candidate profiles.
  2. Search candidates by name; filter by municipality and position.
  3. Add candidates to the voting guide (limited by seats per position),
     view and remove selections, then print the guide.
  4. Bonus: compare up to three candidates, pagination, dark/light theme
     toggle (remembered), candidate social and video links.

Admin features:
  1. Log in as "super" and open /Admin - dashboard counts and CRUD for all
     municipalities, positions and candidates (logo and photo upload).
  2. Log in as "pm" - only Pitt Meadows data is visible. Try opening another
     municipality's candidate edit URL directly; access is denied.
  3. Image uploads accept JPG, PNG and WebP up to 2 MB.

Running locally:
  1. Install the .NET 10 SDK.
  2. Create appsettings.Development.json in the project root (it is excluded
     from source control):

     {
       "ConnectionStrings": {
         "DefaultConnection": "Data Source=app.db;Foreign Keys=True"
       },
       "SeedDemo": true
     }

  3. Run ./run.sh (or: dotnet run) and open http://localhost:5159.
     Migrations are applied on startup and demo data is seeded if the
     database is empty.