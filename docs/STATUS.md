# Current status

Last verified: 2026-11-11.

## Completed

- Split all seven admin content editors into dedicated named controllers, form
  ViewModels, and Index views, retaining the existing EF entities and database.
  University information directly displays hero image, description, mission,
  and vision. Legacy AdminContent GET links redirect to the matching new page;
  generic POST editors and ContentEditorService were retired.
- All 104 isolated account/content checks passed; the form build had no warnings
  or errors. Desktop and 390px checks confirmed the University/program form layout
  and four schema-backed university inputs with no horizontal overflow.
- Rebuilt normal output and restarted localhost:5182 with the dedicated editors.
  Disposable verification previews and their databases were cleaned up.
- Corrected the university navbar after the user rejected the literal sketch frame:
  full-width navy, ivory wordmark/links, fine gold rule, compact crest, and an
  inner container aligned with the page. Removed conflicting old header rules.
  Verified at 1280px, 1920px, and 390px, including mobile menu and Escape dismissal
  of account actions. Build and all 69 isolated checks passed.
- Pushed the previous 13 local progress commits through 33b6609 to origin/main
  at the user's request before continuing the navbar correction.
- Separated administration into its own layout with a persistent desktop sidebar,
  seven content links, an active-page indicator, and collapsible phone navigation.
- Updated the university navbar to the supplied reference: crest/name left,
  Home/About/Academics/Contact/Leadership links, circular account menu right,
  rounded upper border, and bottom divider. Existing brand colors remain.
- All 69 isolated account/content checks passed. Desktop and 390px browser checks
  verified the navbar, account actions, sidebar, and active section without
  horizontal overflow. No university records or database definitions changed.
- Rebuilt the normal application output with zero warnings/errors and restarted
  the owned development server on localhost:5182 with the new navigation layouts.
  The disposable preview completed cleanup successfully.
- Implemented the database-backed five-section homepage, protected admin dashboard,
  and seven content editors. University/contact creation is serialized and limited
  to one row; the other sections support add/edit/delete.
- Added validation, confirmations, safe image/HTTPS link handling, optional social
  icons, encoded text, empty states, and generic database error feedback.
- All 66 isolated account/content checks passed, including CRUD for all seven
  sections, singleton rules, CSRF, roles, and stored text escaping. Build passed
  without warnings/errors. University records and Schema.sql were unchanged.
- Visually inspected the homepage, dashboard, and program editor at desktop and
  390px phone widths with no horizontal overflow. Disposable previews were stopped
  and their databases removed. Rebuilt/restarted the main app on localhost:5182
  with the new content screens; no data was seeded into the university database.
- Restarted the verified stale web process on localhost:5182, rebuilt the normal
  output successfully, and confirmed the live login HTML no longer contains the
  student-access caption. Source removal alone had not updated the running process.
- Integrated the user-approved transparent crest into both shared wordmarks and
  the favicon; documented the canonical asset and kept the original PNG unchanged.
- Removed the requested student-access caption and compacted account forms.
  Browser checks confirmed no normal-form scrolling at 1280x720, 1280x600,
  390x844, and 375x667; fields/actions remain visible and crest loads correctly.
- Logo/layout build passed with zero warnings/errors using a separate ignored
  output directory. The default output was locked by the user's running app;
  it was not stopped. Restart that app to load newly compiled Razor layouts.
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

- University pages and editors are implemented; approved images, real content,
  and verified founder facts still need entering through the editors.
- Student self-registration selected. The user applied the four personal fields;
  live metadata verified all eight tables and the enabled Role CHECK on 2026-11-11.
  Schema.sql is the sole consolidated schema; the completed update script was removed.
  Generated UserTable and its mapping now include the four new fields.
  Registration UI/action and login are implemented.
- The agent did not create or alter the live database. Metadata comparison
  verified the eight table definitions. Isolated integration checks now exercise
  reads/writes for all seven content tables and account access using the existing schema.
- Local default SQL Server was running and the database was accessible during
  verification; future connection failures should be checked against current state.
- First-admin interactive terminal entry, final accessibility audit, and deployed
  HTTPS behavior are not verified. Deployment has not been requested or performed.
- The initial documentation commit contained only docs, schema, skill, and ignore
  rules. The subsequent source baseline adds the existing starter application.

## Next implementation work

1. User creates the first admin through the documented interactive command.
2. Enter approved university content and preselected imagery using the admin editors.
3. Complete final accessibility, unavailable-database, and fresh-checkout QA before submission.

Update this file after completed progress. Use Git history for commit identifiers
and detailed change history rather than maintaining duplicate commit logs here.
