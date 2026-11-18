# Database schema

Database: `Ravenclaw University Db` (Microsoft SQL Server).

The official institution and website name is Ravenclaw University. Existing
Academy-prefixed SQL table and column names remain unchanged for compatibility.

`Schema.sql` is the single consolidated schema, including all applied changes.
Do not keep separate incremental schema/update files. It belongs beside
Controllers, Models, and Views, outside the publicly served wwwroot folder.
The application does not execute this file automatically.

## Running the script

Current structure: seven tables and 35 columns. On 2026-11-18 the user requested
removal of FounderTable's birthday and message columns. Live dependencies were
inspected first; a guarded transaction removed only those two columns and checked
that the founder's retained ID, name and biography were unchanged. Models, editor,
public view, optional founder seed and consolidated Schema.sql reflect this state.
Account birthdays remain supported. The footer quote is fixed presentation text.
No incremental schema file is retained; optional seed/image scripts are content
operations, not additional schema definitions.

Open Schema.sql in SSMS or the MSSQL extension, connect to the intended SQL
Server instance, and execute the whole script once to create a fresh database
and its seven tables. GO is a client batch separator, not a SQL statement to
send directly through application code.

If the database or tables already exist, do not run this creation script.
Do not rerun it against the current database. Future database changes must also
be incorporated into this file so it always describes the complete current schema.
Saving the schema file does not execute SQL or modify a live database.

### Verified About heading and office-time update (2026-11-16)

Added an optional About heading and separate optional office time-in/time-out
values to the existing live tables. Existing office-hours text is retained for
compatibility; no existing content was converted or overwritten. The two affected
EF entity classes were regenerated from the live database, and their mappings
were incorporated into UniversityDbContext. Exact definitions remain in Schema.sql.

The consolidated schema was executed against a disposable database. All eight
tables matched live column metadata, including types, lengths, precision,
nullability, and identities. The disposable database was removed. Build and the
extended read-only EF diagnostic passed. The admin controls and public rendering
now use these new fields; legacy office-hours text remains a read-only fallback.

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

From the repository root, `pwsh -File scripts/Check-Database.ps1` performs a
read-only local SQL metadata/memory check. It requires sqlcmd and permission to
read SQL Server memory diagnostics. It uses encrypted Windows authentication and
the same trusted-development-certificate policy as the local application. Login
and query timeouts are bounded, with a 20-second overall process limit. It never
restarts SQL Server or changes data/configuration. Optional Server/Database
parameters select the intended local development target.

From the MVC project directory (the folder containing the .csproj), run:

```powershell
dotnet run --no-launch-profile -- --check-database
```

This uses the same configured UniversityDbContext as the application. It reads
at most one row from AcademyInfoTable without change tracking, then at most one
projection of LastName, FirstName, MiddleName, and Birthday from UserTable. It
does not select password hashes or display record contents. Empty tables are
successful checks. It also reads the separate office-time fields from ContactsTable.
The university-info read includes the optional About heading. It exits without
starting the website. Exit code 0 means success; code 1
means the check failed. No database or table is created, and no data is changed.

Verified 2026-11-11 against the local Ravenclaw University Db: query passed;
AcademyInfoTable was empty and the UserTable personal-detail query succeeded.
The agent's restricted environment denied SQL access; the check passed when run
with local SQL access. This validates these reads, not every mapping in the schema.

## Tables

All six approved generated subject images are the defaults in SeedPrograms.sql.
Astronomy, Charms, Potions, and Defence Against the Dark Arts were applied locally
on 2026-11-14; Herbology and Transfiguration followed on 2026-11-16. Existing matching
subjects are still preserved by that seed script. ApplyProgramImages.sql is a
separate, deliberate CONTENT update for those existing subjects: it changes only
their ImagePath values and requires exactly one record per subject and either its
documented old placeholder or its approved new image. Unexpected/custom paths stop
the transaction for review. It does not change schema, names, descriptions, or
other subjects. Neither content script is executed automatically by the app.

SeedFounder.sql contains the approved Rowena Ravenclaw biography, applied to the
previously empty founder table on 2026-11-14. It inserts her when absent or updates
only her biography when exactly one matching name exists, stopping on duplicates.
It preserves other founders and the existing matching founder's ID/name. This is optional
content, not an automatic startup action or a schema change.

SeedUniversityContent.sql contains the user-approved tentative About, Mission,
Vision, and four core values. It was applied to the local database on 2026-11-12.
It deliberately replaces these texts and the entire core-value list, preserving
an existing university ID and hero image; it stops if multiple university rows
exist. It is optional content only, never run automatically, and not a migration.

Optional sample program content is in SeedPrograms.sql. It adds six fictional
subjects with the approved images, skipping matching
subject names case-insensitively. The user authorized applying it on 2026-11-12;
six records were inserted into the local university database. This file contains
data inserts only, is not run automatically, and is not a schema migration.
The user requested removing "Sample program:" from all six descriptions on
2026-11-16; both the live descriptions and seed now omit that prefix.
Schema.sql remains the single consolidated schema definition.

| Table | Purpose | Primary key |
| --- | --- | --- |
| AcademyInfoTable | Hero image, university description, mission, and vision | AcademyInfoId |
| ContactsTable | Address, email, telephone, and office hours | ContactId |
| AboutImagesTable | Gallery image paths and alternative text | AboutImageId |
| UserTable | Login accounts, names, birthday, roles, profile image, and active status | UserId |
| FounderTable | Founder name and biography | FounderId |
| CoreValuesTable | Individually managed core value titles | CoreValueId |
| ProgramsTable | Subjects/programs, descriptions, and images | SubjectId |

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
- PasswordHash stores a password hash generated by the authentication framework,
  never a plaintext password.
- The application must reject duplicate usernames; no unique constraint is
  included in this simplified schema.
- IsActive defaults to 1. Authentication must reject inactive accounts.
- Every website visitor must log in. Active students can view content;
  active admins can view and modify it. Enforce authorization on the server
  for all protected pages and content actions, not just navigation buttons.

## Comparison with the supplied schema

Compared 2026-11-18: the latest supplied definition matches Schema.sql's seven
tables and 35 columns, including types, lengths, time precision, intended
nullability, primary keys, identity seed/increment and IsActive default. The paste
omits the Role CHECK permitting Admin or Student, which Schema.sql retains from
the previously verified live database. It also omits CREATE DATABASE, USE and GO
for fresh setup. Constraint names and extra default parentheses do not affect
these definitions. Use explicit NULL declarations, as in Schema.sql, to avoid
depending on session/database nullability defaults. Remove the pasted backslash
separators if they are literal characters in a script.

Current live verification was attempted but did not complete: the sandbox
diagnostic reported Named Pipes access denied/login timeout, and the local-access
metadata retry returned Timeout expired without metadata. This review confirms
the repository comparison, not the current live database state. No database
changes were made.

## Social-links removal and Founder policy (2026-11-16)

Inspected live data and dependencies before removal: SocialLinksTable had no records and no referencing foreign keys. A guarded transaction aborted if either condition changed; the empty table was dropped successfully. Schema.sql and EF mappings/entities now contain the remaining seven tables. No other data or columns changed. The existing Rowena Ravenclaw record was preserved. Founder is enforced as edit-only in its controller using a serializable single-record query; no admin create/delete routes remain. This is an application policy, not a new database singleton constraint. Direct SQL must preserve the one-founder policy.
