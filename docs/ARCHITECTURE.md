# Architecture

## Verified project structure

- Solution: 241611JalopFinalQuiz2.sln.
- Application: 241611JalopFinalQuiz2/241611JalopFinalQuiz2.csproj.
- Framework: ASP.NET Core MVC targeting net9.0; nullable reference types enabled.
- Root namespace: _241611JalopFinalQuiz2.
- Controllers handle HTTP requests; Models hold application data; Views hold Razor UI.
- UniversityContentService assembles the read-only homepage. Each admin content
  section has its own named controller, ViewModel, and Views/<Section>/<Section>.cshtml.
  Each controller maps its form fields directly to its existing EF entity and uses
  UniversityDbContext. No repository layer is needed.
- Main Razor views use their section names instead of Index.cshtml: Home, Profile,
  AccountManagement, UniversityInformation, AcademicPrograms,
  CoreValues, FounderInformation and ContactInformation. Controllers explicitly
  select these views on GET, validation failure and unavailable responses; action
  names remain Index so existing routes, links and form actions continue to work.
- AdminContentController retains GET redirects for old bookmarks only. Shared
  ContentSections contains sidebar labels/routes; AdminContentValidation contains
  reusable input validators and the sidebar section interface, not CRUD behavior.
- AdminController retains an Admin-only redirect to UniversityInformation for
  old /Admin links. The dashboard view, model, and content-count queries are removed.
  AccountManagementController loads global account totals
  separately from its filtered, capped directory query. These totals are read-only.
  AdminPreviewViewModel and
  _AdminContentPreview are shared presentation helpers; dedicated editor form models,
  controllers and views remain separate. admin-editor.js handles local previews,
  unsaved-change guards and validated saving feedback; it makes no network writes.
- The project excludes nested TestResults/** from default MSBuild items so ignored
  test source copies cannot be compiled/published as application controllers or views.
- Views/Admin selects _AdminLayout through _ViewStart. Each dedicated content view
  selects that same layout explicitly. University views use _Layout; account views
  use _AccountLayout.
- wwwroot contains public static assets, never database scripts or private docs.
- Styles/ contains Tailwind CSS v4 source split into base tokens, reusable components,
  layouts, and university sections. Shared semantic classes use @apply, preserving
  MVC markup and JavaScript hooks. Styles/README.md documents the folder structure.
- navigation-drawer.js shares mobile public/admin drawer behavior over native details:
  breakpoint state, focus trap, inert background, scroll lock and close handling.
  Navigation/sign-out release this state before admin-editor.js prompts for drafts.
  about-story.js selects the same marker-driven sticky chapters at all widths;
  mobile CSS places the image above a bounded, scrollable text region.
- The root package.json/package-lock.json pin the npm build dependencies. Normal
  dotnet builds run the Tailwind CLI; missing dependencies are installed with npm ci.
  wwwroot/css/site.css is generated, ignored by Git, and included in publish output.
  All three layouts load that one local asset; Node/npm are needed at build time.
  The unused starter _Layout.cshtml.css and Bootstrap distribution were removed.
  Only the loaded minified jQuery/validation distributions and their licenses remain.
- ImageUploadService stores runtime uploads outside source assets in App_Data/Uploads
  (or ImageUploads:RootPath), publicly served under /images/uploads. Database image
  columns continue storing paths. AccountManagement has its own controller/ViewModel/
  section-named view and uses existing UserTable; no schema changes are needed.

## Database

The retired gallery has no controller, view model, view or homepage query.
AboutImagesTable and its generated EF mapping remain for existing-database
compatibility only. No gallery records or uploaded files were deleted.

Use Microsoft SQL Server. Database/Schema.sql inside the application is the
fresh-database creation source of truth. It is not executed automatically.
Schema.sql is the only maintained schema file and contains every applied change.
Do not retain incremental schema scripts. Never rerun CREATE DATABASE or drop data
to update the existing database. After a future authorized database change, update
the consolidated schema and regenerate affected models. The agent's verification
uses read-only queries; the user applied the personal-details change.

Seven independent tables serve one university. There are no foreign keys or extra
indexes; SQL Server creates primary-key indexes. Keep one university-info row and
one contact row through application logic. See Database/README.md for details.

The public name is Ravenclaw University. Existing Academy-prefixed table and
column names remain unchanged; a branding correction does not require a database
migration or a SQL identifier rename.

The user selected EF Core database-first scaffolding from existing SQL tables.
SQL Server and Design package references (9.*) are present. Data/ contains the
generated UniversityDbContext and Models/ contains seven entities. A separate
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

The university-info read now includes the editable About heading. A further
read-only contact projection verifies the separate office time-in/time-out fields.
Existing office-hours text is retained for compatibility; the new fields are
nullable. Hero and About now saves the heading and core-value list transactionally;
Contacts saves the paired time values. The homepage reads these saved fields and
falls back to the established heading/legacy office-hours text when empty.

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

## Personal information and content simplification (2026-11-16)

The existing cookie-validation projection also reads the signed-in account's
personal names and profile image, populating a request-scoped AccountMenuViewModel
only after the account passes the unchanged role/active/username checks. The menu
partial uses this fresh identity without another database query or cookie claims.
Admin badge and Manage website rendering still depend on the authorized role.

Profile's presentation and draft behavior remain separate from persistence:
Views/Profile/Profile.cshtml, layouts/profile.css and profile-editor.js provide the
responsive identity column and tabbed forms, shared image-upload preview integration,
dirty tracking, native discard dialog and validated saving state. ProfileController
owns both personal updates and the separate password POST. ChangePasswordViewModel
validates current/new/confirmation independently of ProfileViewModel. PasswordHasher
and a conditional ExecuteUpdate change only the current user's matched hash; success
signs out that browser session. Passwords never enter view values. JavaScript-free
links select the forms server-side; ordinary upload/save remains available.
Invalid personal-detail POSTs keep a separate projected SavedProfile in ViewData;
encoded per-field saved values provide the local Cancel baseline without including
passwords. Cancel GET links provide the same persisted state without JavaScript.
_PasswordToggle and password-visibility.js are shared by login, registration and
profile password fields through _ValidationScriptsPartial. The unused Home/Privacy
action and starter view have been removed.

ProfileController/ProfileViewModel/Views/Profile/Profile.cshtml form a dedicated authenticated editor. Reads project only personal fields; writes target the cookie identity through ExecuteUpdate and update only personal names, birthday and image. Posted IDs, username, role and active flags are not accepted. Uploads reuse ImageUploadService and all saves require antiforgery. A changed/deactivated account cannot be updated through a stale session. Founder editing uses a serializable update of its single existing record, without create/delete actions. UniversityPageViewModel now holds one Founder. Social-link controllers, entity/mapping, form model/view, validators and read queries were removed alongside the empty live table; consolidated Schema.sql now has seven tables.
