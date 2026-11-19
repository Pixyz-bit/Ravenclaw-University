# Accounts and access

Implemented: student self-registration, login/logout, cookie sessions, and an
Admin-only editing area opening Hero and About. Content editing and the university homepage are documented
in CONTENT.md.
No schema changes were needed; Schema.sql remains the single schema source.

## Run locally

From the MVC project directory, run `dotnet run`, then open the URL printed in
the terminal. Launch profiles select Development and enable local static assets.
Account routes are /Account/Register and /Account/Login. The protected university
portal is / and administration opens at /UniversityInformation.

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
- The dedicated content controllers and /AccountManagement use server-side
  Admin authorization. The legacy /AdminContent controller is removed. Students can read
  university content but cannot access editors or POST content mutations.
- Passwords use ASP.NET Core PasswordHasher. Hashes are upgraded on login when needed.
- Usernames are trimmed and compared case-insensitively with explicit SQL collation.
  Duplicate checks/inserts use a serializable transaction because the agreed
  simplified schema has no unique index. Existing ambiguous duplicate names fail login.
- Cookies are HttpOnly, SameSite=Lax, nonpersistent, and expire after 30 minutes
  without sliding renewal. Production cookies require HTTPS; local Development
  follows the request scheme. Use a published app and HTTPS when deploying.
- Every authenticated request rechecks the database account's active status, role,
  and username. Deactivation, deletion, or a role/name change rejects the session.
  Database validation errors fail closed. Self-service password changes sign out
  the current session; recovery/reset and session-wide revocation remain outside scope.
- Login/registration/password-change POSTs share a 30-request/minute per-client-IP fixed-window
  limit; rejection returns HTTP 429. This is in-process, not a distributed limiter.
  Configure trusted proxies deliberately if deploying behind one.
- All account POSTs use anti-forgery checks. Logout is POST-only. Login return URLs
  must be local. Dedicated view models prevent role/hash/active-state overposting.
- Login errors are generic. Database exception details remain in server logs.
  Protected pages and account responses are marked no-store.

## Verification

Personal information is available at /Profile to authenticated Admin and Student
accounts. The dedicated form edits only the signed-in account's names, optional
birthday and profile image. Identity comes from the cookie, never a posted ID.
Username, role, IsActive and password remain unchanged by the personal-details POST.
Writes require antiforgery and an active account with the same role.
Reads project personal fields only; uploads reuse existing validation/storage.
Saving names preserves the session because the login username does not change.
The public account menu links to this page; the admin sidebar omits personal editing.

The Change password tab posts a dedicated ChangePasswordViewModel to
/Profile/ChangePassword. It requires authentication, antiforgery, the current password,
a different 8–128-character new password and matching confirmation. The registered
ASP.NET Core PasswordHasher verifies and hashes secrets. An atomic conditional update
checks the original hash, active state, username and role before changing only the
signed-in account's hash. Posted IDs/roles/usernames are ignored. Password values are
cleared from models and attempted-value state before rendering validation errors.
Success signs out this browser session and redirects to login with a confirmation.
No schema changes or email/reset flows were added; other existing sessions are not
revoked by this operation. Personal details and passwords use independent forms.

Every web password input has a shared eye button, using the existing password-manager
autocomplete modes and 128-character limits. Visibility changes remain local and
never submit/copy/store the password. Labels and pressed state identify Show/Hide;
reset and restored pages remask the inputs. No-JS forms retain masked native inputs.
Personal Cancel restores saved values rather than rejected POST values; its ordinary
GET link also works without JavaScript.

Verified 2026-11-18: all 176 disposable HTTP/SQL checks passed, including missing CSRF,
anonymous/GET rejection, incorrect current password, length/confirmation validation,
unchanged-password rejection, unchanged hashes after failures, ownership tampering,
success/sign-out, old-password rejection and new-password sign-in. Existing profile
upload and content/account checks also passed. No university accounts were modified.

Follow-up verification: 182 isolated HTTP/SQL checks passed after adding shared eye
controls, saved-value Cancel metadata and removal of the unused Privacy route.
Sixteen browser cases covered the requested UI regressions, including keyboard eyes,
no unintended password-form submissions and Cancel with/without JavaScript.

Account management rows offer only Activate/Deactivate and View information.
Details is an Admin-only, read-only profile page with no password/hash fields.
SetActive is an antiforgery-protected Admin POST that changes only IsActive,
preserves search, and retains the serializable own-account/last-active-Admin
safeguards. Account Management totals count all registered accounts, enabled accounts,
and disabled accounts; these are account states, not online-session analytics.

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
