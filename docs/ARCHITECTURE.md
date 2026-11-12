# Architecture

## Verified project structure

- Solution: 241611JalopFinalQuiz2.sln.
- Application: 241611JalopFinalQuiz2/241611JalopFinalQuiz2.csproj.
- Framework: ASP.NET Core MVC targeting net9.0; nullable reference types enabled.
- Root namespace: _241611JalopFinalQuiz2.
- Controllers handle HTTP requests; Models hold application data; Views hold Razor UI.
- UniversityContentService assembles the read-only homepage. Each admin content
  section has its own named controller, ViewModel, and Views/<Section>/Index.cshtml.
  Each controller maps its form fields directly to its existing EF entity and uses
  UniversityDbContext. No repository layer or schema change is needed.
- AdminContentController retains GET redirects for old bookmarks only. Shared
  ContentSections contains sidebar labels/routes; AdminContentValidation contains
  reusable input validators and the sidebar section interface, not CRUD behavior.
- Views/Admin selects _AdminLayout through _ViewStart. Each dedicated content view
  selects that same layout explicitly. University views use _Layout; account views
  use _AccountLayout.
- wwwroot contains public static assets, never database scripts or private docs.
- ImageUploadService stores runtime uploads outside source assets in App_Data/Uploads
  (or ImageUploads:RootPath), publicly served under /images/uploads. Database image
  columns continue storing paths. AccountManagement has its own controller/ViewModel/
  Index view and uses existing UserTable; no schema changes are needed.

## Database

Use Microsoft SQL Server. Database/Schema.sql inside the application is the
fresh-database creation source of truth. It is not executed automatically.
Schema.sql is the only maintained schema file and contains every applied change.
Do not retain incremental schema scripts. Never rerun CREATE DATABASE or drop data
to update the existing database. After a future authorized database change, update
the consolidated schema and regenerate affected models. The agent's verification
uses read-only queries; the user applied the personal-details change.

Eight independent tables serve one university. There are no foreign keys or extra
indexes; SQL Server creates primary-key indexes. Keep one university-info row and
one contact row through application logic. See Database/README.md for details.

The public name is Ravenclaw University. Existing Academy-prefixed table and
column names remain unchanged; a branding correction does not require a database
migration or a SQL identifier rename.

The user selected EF Core database-first scaffolding from existing SQL tables.
SQL Server and Design package references (9.*) are present. Data/ contains the
generated UniversityDbContext and Models/ contains the eight entities. A separate
Repositories/ layer is optional, not a requirement. Continue using SQL scripts
for schema changes; do not introduce migrations as a competing schema authority.

appsettings.json contains ConnectionStrings:UniversityDb for the local Windows-
authenticated database connection. Keep exactly one root JSON object and merge
new settings into it rather than appending another object. Generated context
code omits OnConfiguring. Program.cs registers the context with UseSqlServer
after WebApplication.CreateBuilder and before builder.Build. Register MVC once.

For a repeatable local read check, run the application with --check-database.
Program.cs creates a service scope, resolves the registered UniversityDbContext,
and reads at most one AcademyInfoTable record with AsNoTracking, followed by at
most one projection of LastName, FirstName, MiddleName, and Birthday from UserTable.
The names have 100-character mappings; the nullable birthday uses DateOnly?.
It does not select password hashes or display account values. It reports only
success and whether the university-info table is empty. It exits before
starting the HTTP server. Failure returns a nonzero process exit code. This
diagnostic does not call SaveChanges, run migrations, or create a database.

## Authentication

Role values are Admin and Student. MVC must require authentication for content
and admin authorization for every content mutation. Use framework password
hashing and anti-forgery protection for state-changing forms.

Cookie authentication and a fallback login policy are configured in Program.cs.
AccountController handles anonymous login/registration and protected POST logout.
AdminController requires Admin. HomeController requires login through the fallback.
AccountService handles password hashing/verification and serialized account creation;
UniversityCookieEvents revalidates active status and roles on each request.
FirstAdminCommand provides an interactive, local-only first-admin bootstrap.
See AUTHENTICATION.md for setup, limits, and the disposable-database HTTP verifier.
