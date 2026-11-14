# Database schema

Database: `Ravenclaw University Db` (Microsoft SQL Server).

The official institution and website name is Ravenclaw University. Existing
Academy-prefixed SQL table and column names remain unchanged for compatibility.

`Schema.sql` is the single consolidated schema, including all applied changes.
Do not keep separate incremental schema/update files. It belongs beside
Controllers, Models, and Views, outside the publicly served wwwroot folder.
The application does not execute this file automatically.

## Running the script

Open Schema.sql in SSMS or the MSSQL extension, connect to the intended SQL
Server instance, and execute the whole script once to create a fresh database
and its eight tables. GO is a client batch separator, not a SQL statement to
send directly through application code.

If the database or tables already exist, do not run this creation script.
Do not rerun it against the current database. Future database changes must also
be incorporated into this file so it always describes the complete current schema.
Saving the schema file does not execute SQL or modify a live database.

### Verified personal-details update

The user applied LastName, FirstName, MiddleName (NVARCHAR(100)), and Birthday
(DATE) to UserTable. Live SQL Server metadata confirmed these nullable fields
on 2026-11-11. They are included in Schema.sql. The completed incremental script
was removed; its history remains available in Git.

The generated UserTable entity and max-length mappings were refreshed from the
live database using a temporary scaffold, reviewed, and integrated. Nullable
names use string? with 100-character limits; Birthday uses DateOnly?. Build and
an EF read of the four fields passed. No migrations were run.

Student self-registration is implemented: first/last name are required; middle name
and birthday are optional. Future birthdays are rejected and Role=Student is set
server-side. See ../../docs/AUTHENTICATION.md for first-admin creation and tests.

## Verifying the application connection

From the MVC project directory (the folder containing the .csproj), run:

```powershell
dotnet run --no-launch-profile -- --check-database
```

This uses the same configured UniversityDbContext as the application. It reads
at most one row from AcademyInfoTable without change tracking, then at most one
projection of LastName, FirstName, MiddleName, and Birthday from UserTable. It
does not select password hashes or display record contents. Empty tables are
successful checks. It exits without starting the website. Exit code 0 means success; code 1
means the check failed. No database or table is created, and no data is changed.

Verified 2026-11-11 against the local Ravenclaw University Db: query passed;
AcademyInfoTable was empty and the UserTable personal-detail query succeeded.
The agent's restricted environment denied SQL access; the check passed when run
with local SQL access. This validates these reads, not every mapping in the schema.

## Tables

SeedFounder.sql contains the approved Rowena Ravenclaw biography, applied to the
previously empty founder table on 2026-11-14. It inserts her when absent or updates
only her biography when exactly one matching name exists, stopping on duplicates.
It preserves other founders, birthdays, and stored messages. This is optional
content, not an automatic startup action or a schema change.

SeedUniversityContent.sql contains the user-approved tentative About, Mission,
Vision, and four core values. It was applied to the local database on 2026-11-12.
It deliberately replaces these texts and the entire core-value list, preserving
an existing university ID and hero image; it stops if multiple university rows
exist. It is optional content only, never run automatically, and not a migration.

Optional sample program content is in SeedPrograms.sql. It adds six clearly labelled
fictional sample subjects with existing university illustrations, skipping matching
subject names case-insensitively. The user authorized applying it on 2026-11-12;
six records were inserted into the local university database. This file contains
data inserts only, is not run automatically, and is not a schema migration.
Schema.sql remains the single consolidated schema definition.

| Table | Purpose | Primary key |
| --- | --- | --- |
| AcademyInfoTable | Hero image, university description, mission, and vision | AcademyInfoId |
| ContactsTable | Address, email, telephone, and office hours | ContactId |
| AboutImagesTable | Gallery image paths and alternative text | AboutImageId |
| UserTable | Login accounts, names, birthday, roles, profile image, and active status | UserId |
| FounderTable | Founder name, birthday, biography, and message | FounderId |
| CoreValuesTable | Individually managed core value titles | CoreValueId |
| ProgramsTable | Subjects/programs, descriptions, and images | SubjectId |
| SocialLinksTable | Platform names, URLs, and optional icon classes | SocialLinkId |

All primary keys are auto-generated INT IDENTITY(1,1) values. Nullable columns
are explicitly marked NULL. There are no foreign keys or additional indexes;
SQL Server creates indexes for the primary keys.

## Content and application rules

- All content belongs to a single university, so the tables are independent.
- Keep one record each in AcademyInfoTable and ContactsTable through application logic.
  The database does not enforce this row limit.
- AcademyName and AcademyHistory are intentionally omitted.
- CoreValuesTable contains a title only; its description was removed.
- Store image paths or URLs rather than image bytes.
- IconClass stores an identifier such as `fa-brands fa-facebook`; the frontend
  must load the matching icon library and should use an approved set of icons.
- Birthday can be NULL when the founder's exact date of birth is unknown.
- PasswordHash stores a password hash generated by the authentication framework,
  never a plaintext password.
- The application must reject duplicate usernames; no unique constraint is
  included in this simplified schema.
- IsActive defaults to 1. Authentication must reject inactive accounts.
- Every website visitor must log in. Active students can view content;
  active admins can view and modify it. Enforce authorization on the server
  for all protected pages and content actions, not just navigation buttons.

## Comparison with the supplied schema

Verified 2026-11-11 against live database metadata: all eight tables, 38 columns,
column types, sizes, identity settings (seed/increment 1), nullability, primary keys,
and the IsActive default match the user's supplied definition and Schema.sql.
No foreign keys exist. Constraint names may differ because SQL Server generates them.

The user's pasted export omits the Role CHECK, but the live database contains an
enabled, trusted constraint permitting Admin or Student only. Schema.sql retains
it to reflect the actual database. CREATE DATABASE, USE, and GO provide fresh setup.
