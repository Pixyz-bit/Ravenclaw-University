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

Eight independent tables serve one academy. There are no foreign keys or extra
indexes; SQL Server creates primary-key indexes. Keep one academy-info row and
one contact row through application logic. See Database/README.md for details.

EF Core and DbContext were explained but not yet installed or implemented.
If selected, a Data/ folder can hold AcademyDbContext and Models/ can hold
entities. A separate Repositories/ layer is optional, not a requirement.
Do not use migrations and manual schema scripts as competing schema authorities.
Choose a workflow explicitly before introducing either scaffolding or migrations.

## Authentication

Role values are Admin and Student. MVC must require authentication for content
and admin authorization for every content mutation. Use framework password
hashing and anti-forgery protection for state-changing forms.

No authentication scheme, EF Core provider, or DbContext is currently configured.
UseAuthorization in the starter pipeline alone does not implement login.
