# Current status

Last verified: 2026-11-11.

## Completed

- Existing ASP.NET Core MVC net9.0 starter inspected.
- User-supplied schema compared against the final agreed eight-table design.
- Database/Schema.sql and Database/README.md created in the MVC project.
- Missing Admin/Student role CHECK restored in the documented schema.
- Project context documents and a project workflow skill created.
- AI workflow requires relevant skills, design consistency, and progress commits.
- Git initialized; generated output and local secrets excluded via .gitignore.
- Documentation links and skill frontmatter checked manually; staged whitespace
  checks passed. The bundled skill validator could not run because Python was unavailable.

## Not implemented / not verified

- The application still has the starter Home page and controller.
- No EF Core packages or DbContext are present in the inspected project.
- Login, account provisioning, active-account checks, and role restrictions are not implemented.
- No content administration or database-backed page rendering exists yet.
- No live database was created, altered, or validated during documentation.
- Earlier local SQL services were shown running by the user; current connectivity
  and the live database schema must be checked when needed, not assumed.
- No application build was run for these documentation-only changes.
- Existing application and .github files were left untracked in the initial
  documentation commit; that commit contains only documentation, schema, skill,
  and ignore rules. Establish a source baseline before relying on Git for app recovery.

## Next implementation work

1. Verify the intended SQL instance and existing database schema without changing data.
2. Choose the EF Core/existing-database workflow and configure database access.
3. Implement login and Admin/Student authorization before exposing content.
4. Build content views and admin editing using the documented design and schema.

Update this file after completed progress. Use Git history for commit identifiers
and detailed change history rather than maintaining duplicate commit logs here.
