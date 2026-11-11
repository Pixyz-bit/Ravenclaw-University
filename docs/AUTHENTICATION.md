# Accounts and access

Implemented: student self-registration, login/logout, cookie sessions, and an
Admin-only starter page. Content editing and university content pages are pending.
No schema changes were needed; Schema.sql remains the single schema source.

## Run locally

From the MVC project directory, run `dotnet run`, then open the URL printed in
the terminal. Launch profiles select Development and enable local static assets.
Account routes are /Account/Register and /Account/Login. The protected university
portal is / and the protected administration starter is /Admin.

Registration requires first/last name, a 3–100 character username (letters,
numbers, periods, underscores, hyphens), an 8–128 character password, and matching
confirmation. Middle name and birthday are optional. Future birthdays are rejected
using the current date in Asia/Manila. Role is always Student and IsActive is true;
neither is accepted from the form. Successful registration redirects to login.

## First admin

In an interactive terminal in the MVC project directory, run:

```powershell
dotnet run --no-launch-profile -- --create-admin
```

The local command prompts for username, first/last name, password, and confirmation.
Password entry is hidden and credentials are not passed as command-line arguments.
It hashes the password and creates the first Admin only if no admin and no matching
username exist. It does not start the web server and refuses redirected input.
There is no default admin password or public admin-registration endpoint.
This command has not been used to create an account in the university database.
Its interactive terminal/password-entry path still requires a manual check.

## Enforcement

- Fallback authorization requires login for MVC content by default. Only login,
  registration, the generic error handler, and public static assets are anonymous.
- /Admin uses server-side Admin authorization. Its starter view explicitly says
  content editors are pending; it does not perform database editing yet.
- Passwords use ASP.NET Core PasswordHasher. Hashes are upgraded on login when needed.
- Usernames are trimmed and compared case-insensitively with explicit SQL collation.
  Duplicate checks/inserts use a serializable transaction because the agreed
  simplified schema has no unique index. Existing ambiguous duplicate names fail login.
- Cookies are HttpOnly, SameSite=Lax, nonpersistent, and expire after 30 minutes
  without sliding renewal. Production cookies require HTTPS; local Development
  follows the request scheme. Use a published app and HTTPS when deploying.
- Every authenticated request rechecks the database account's active status, role,
  and username. Deactivation, deletion, or a role/name change rejects the session.
  Database validation errors fail closed. Password-reset/session-wide revocation
  is outside the implemented account scope.
- Login/registration POSTs share a 30-request/minute per-client-IP fixed-window
  limit; rejection returns HTTP 429. This is in-process, not a distributed limiter.
  Configure trusted proxies deliberately if deploying behind one.
- All account POSTs use anti-forgery checks. Logout is POST-only. Login return URLs
  must be local. Dedicated view models prevent role/hash/active-state overposting.
- Login errors are generic. Database exception details remain in server logs.
  Protected pages and account responses are marked no-store.

## Verification

From the repository root in PowerShell, with SQL Server and sqlcmd available:

```powershell
.\scripts\Verify-Authentication.ps1
```

The script builds the project, creates a GUID-named disposable SQL database from
Schema.sql, starts a hidden local Development server with a connection override,
and exercises real HTTP requests, cookies, forms, and database persistence. Its
finally block stops that server and drops only its own disposable database.
It does not use or alter Ravenclaw University Db records. Temporary logs and the
temporary schema copy remain under the system temp folder, outside the repository.

Verified 2026-11-11: build passed with zero warnings/errors and 28 checks passed,
covering anonymous routes/assets, registration validation, password hashing,
role/active-state tampering, duplicate names, login, safe redirects, student/admin
separation, logout/CSRF, inactive accounts, stale sessions, and rate limiting.
The isolated admin-role test promotes a test fixture with SQL; it does not test
the interactive first-admin terminal command. No production accounts were created.
Login/registration were visually inspected at desktop and 390px mobile widths.
