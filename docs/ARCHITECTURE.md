# Architecture

## Verified project structure

- Solution: 241611JalopFinalQuiz2.sln.
- Application: 241611JalopFinalQuiz2/241611JalopFinalQuiz2.csproj.
- Framework: ASP.NET Core MVC targeting net9.0; nullable reference types enabled.
- Root namespace: _241611JalopFinalQuiz2.
- Controllers handle HTTP requests; Models hold application data; Views hold Razor UI.
- wwwroot contains public static assets, never database scripts or private docs.

## Database

Use Microsoft SQL Server. Database/Schema.sql inside the application is the
fresh-database creation source of truth. It is not executed automatically.
For an existing database, prepare a separate ALTER script rather than rerunning
CREATE DATABASE or dropping data. No live database changes have been performed
as part of the documentation work.

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
and reads at most one AcademyInfoTable record with AsNoTracking. It reports only
success and whether the table is empty, never record contents. It exits before
starting the HTTP server. Failure returns a nonzero process exit code. This
diagnostic does not call SaveChanges, run migrations, or create a database.

## Authentication

Role values are Admin and Student. MVC must require authentication for content
and admin authorization for every content mutation. Use framework password
hashing and anti-forgery protection for state-changing forms.

DbContext is registered; no authentication scheme is configured in the application.
UseAuthorization in the starter pipeline alone does not implement login.
