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

## Not implemented / not verified

- The application still has the starter Home page and controller.
- Login, account provisioning, active-account checks, and role restrictions are not implemented.
- No content administration or database-backed page rendering exists yet.
- No live database was created, altered, or validated during documentation.
- Earlier local SQL services were shown running by the user; current connectivity
  and the live database schema must be checked when needed, not assumed.
- Generated models and the context compile. Application-level database reads
  have not yet been tested; generation does not implement login or content pages.
- The initial documentation commit contained only docs, schema, skill, and ignore
  rules. The subsequent source baseline adds the existing starter application.

## Next implementation work

1. Verify the intended SQL instance and existing database schema without changing data.
2. Verify a read through UniversityDbContext; retain SQL scripts as schema authority.
3. Implement login and Admin/Student authorization before exposing content.
4. Build content views and admin editing using the documented design and schema.

Update this file after completed progress. Use Git history for commit identifiers
and detailed change history rather than maintaining duplicate commit logs here.
