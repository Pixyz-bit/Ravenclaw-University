# Current status

Last verified: 2026-11-11.

## Completed

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
- Verified the EF Core read against localhost / Ravenclaw University Db. The
  table exists and is empty. No records or schema were changed. Build passed
  with zero warnings/errors. Restricted execution failed with access denied;
  the same diagnostic succeeded with local SQL Server access.

## Not implemented / not verified

- The application still has the starter Home page and controller.
- Login, account provisioning, active-account checks, and role restrictions are not implemented.
- No content administration or database-backed page rendering exists yet.
- No live database was created or altered during this work. The read check
  verifies AcademyInfoTable, not the mappings of all eight tables.
- Local default SQL Server was running and the database was accessible during
  verification; future connection failures should be checked against current state.
- Login and content pages remain pending even though the database read works.
- The initial documentation commit contained only docs, schema, skill, and ignore
  rules. The subsequent source baseline adds the existing starter application.

## Next implementation work

1. Implement login and Admin/Student authorization before exposing content.
2. Provide an explicit way to provision the first admin using a password hasher.
3. Build content views and admin editing using the documented design and schema.

Update this file after completed progress. Use Git history for commit identifiers
and detailed change history rather than maintaining duplicate commit logs here.
