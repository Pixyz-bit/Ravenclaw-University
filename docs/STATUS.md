# Current status

Last verified: 2026-11-11.

## Completed

- Implemented student registration, login/logout, cookie authentication, fallback
  login protection, and Admin-only starter administration. Registration forces
  Student; active status/roles are revalidated per authenticated request.
- Implemented interactive --create-admin bootstrap; no university admin account
  has been created by the agent. Interactive terminal entry still needs a manual check.
- Added serializable duplicate-name prevention, generic login errors, anti-forgery
  checks, safe local redirects, 30-minute sessions, and account POST rate limiting.
- Built navy/ivory/gold account pages and minimal protected portal/admin shells.
- Build passed with zero warnings/errors and 28 isolated HTTP/database checks passed.
  Disposable test databases were removed; university records/schema were unchanged.
- Visually inspected desktop and mobile login/registration. Authentication setup
  and repeatable verification are documented in AUTHENTICATION.md.
- Refreshed UserTable and UniversityDbContext from a temporary database-first
  scaffold after the personal-details update. Names map to nullable strings with
  100-character limits; Birthday maps to DateOnly?.
- Build passed with zero warnings/errors. Extended --check-database verified a
  read of all four personal-detail columns without selecting password hashes,
  displaying account values, or changing data. The command exited successfully.
- Existing ASP.NET Core MVC net9.0 starter inspected.
- User-supplied schema compared against the final agreed eight-table design.
- Database/Schema.sql and Database/README.md created in the MVC project.
- Missing Admin/Student role CHECK restored in the documented schema.
- Project context documents and a project workflow skill created.
- Official institution name established as Ravenclaw University throughout
  context documents and AI instructions. Existing SQL identifiers are unchanged.
- AI workflow requires relevant skills, design consistency, and progress commits.
- Git initialized; generated output and local secrets excluded via .gitignore.
- Existing MVC application and .github instructions included in the source baseline.
- GitHub repository: https://github.com/Pixyz-bit/Ravenclaw-University.
  The development branch is main; origin points to this repository.
- Documentation links and skill frontmatter checked manually; staged whitespace
  checks passed. The bundled skill validator could not run because Python was unavailable.
- User added EF Core SQL Server and Design package references (9.*) and a local
  tool manifest while following the database-first setup guide.
- Fixed appsettings.json to contain one JSON object with ConnectionStrings:UniversityDb.
  JSON parsing and dotnet build --no-restore passed with zero warnings/errors.
- User generated all eight entity classes in Models/ and Data/UniversityDbContext.cs
  through database-first scaffolding. Context uses the configured connection,
  without an embedded connection string.
- Fixed Program.cs registration order: create builder, register MVC and DbContext,
  then build the app. Removed duplicate MVC registration.
- dotnet build --no-restore passed after integration with zero warnings/errors.
- Added an explicit --check-database command that reads AcademyInfoTable through
  the registered UniversityDbContext and exits without starting the website.
- Created CHECKLIST.md covering completed work and remaining implementation,
  authentication, content management, quality verification, and delivery.
- Verified the EF Core read against localhost / Ravenclaw University Db. The
  table exists and is empty. No records or schema were changed. Build passed
  with zero warnings/errors. Restricted execution failed with access denied;
  the same diagnostic succeeded with local SQL Server access.

## Not implemented / not verified

- Home and Admin are minimal protected starter pages; full university content and
  management forms are not implemented.
- Student self-registration selected. The user applied the four personal fields;
  live metadata verified all eight tables and the enabled Role CHECK on 2026-11-11.
  Schema.sql is the sole consolidated schema; the completed update script was removed.
  Generated UserTable and its mapping now include the four new fields.
  Registration UI/action and login are implemented.
- No content administration or database-backed page rendering exists yet.
- The agent did not create or alter the live database. Metadata comparison
  verified the eight table definitions. EF reads verified AcademyInfoTable and the
  four UserTable personal-detail columns; other runtime mappings still need testing.
- Local default SQL Server was running and the database was accessible during
  verification; future connection failures should be checked against current state.
- First-admin interactive terminal entry, final accessibility audit, and deployed
  HTTPS behavior are not verified. Deployment has not been requested or performed.
- The initial documentation commit contained only docs, schema, skill, and ignore
  rules. The subsequent source baseline adds the existing starter application.

## Next implementation work

1. User creates the first admin through the documented interactive command.
2. Build university content views and admin editing using the documented design and schema.
3. Verify remaining content-table runtime mappings and complete responsive/accessibility QA.

Update this file after completed progress. Use Git history for commit identifiers
and detailed change history rather than maintaining duplicate commit logs here.
