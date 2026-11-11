# Ravenclaw University implementation checklist

Updated: 2026-11-11. Checked items represent completed work, not just plans.
Work through the remaining stages in order. See STATUS.md for verification limits.

## 1. Planning and project foundation

- [x] Establish Ravenclaw University as the official name.
- [x] Define the required hero, about, academic programs, founder, and contact content.
- [x] Plan layouts and About-page content with text wireframes and online references.
- [x] Record the navy/gold/ivory design direction, typography options, and motto.
- [x] Establish admin editing, student viewing, and mandatory login requirements.
- [x] Inspect the ASP.NET Core MVC net9.0 starter and existing folder structure.
- [x] Create project context, requirements, architecture, design, decisions, and status documents.
- [x] Create AGENTS.md and a project skill requiring relevant skills, design consistency, and commits.
- [ ] Finalize a single landing page versus separate About/program pages before creating routes.

## 2. Database and EF Core

- [x] Set database name to Ravenclaw University Db.
- [x] Document the simplified eight-table schema with no foreign keys or extra indexes.
- [x] Remove AcademyName, AcademyHistory, and core-value description columns from the documented schema.
- [x] Include optional SocialLinksTable.IconClass and the Admin/Student role CHECK.
- [x] Save Schema.sql and database usage documentation outside wwwroot.
- [x] Confirm the default local SQL Server and database are accessible during verification.
- [x] Add EF Core SQL Server/Design references and the local EF CLI tool manifest.
- [x] Configure ConnectionStrings:UniversityDb and correct the JSON structure.
- [x] Generate all eight models and UniversityDbContext from the existing database.
- [x] Register DbContext after builder creation and fix duplicate MVC registration.
- [x] Build successfully with zero warnings/errors.
- [x] Add and run --check-database through the registered DbContext.
- [x] Verify AcademyInfoTable can be queried; it was empty and no data was changed.
- [ ] Verify the remaining seven table mappings against the documented schema; report mismatches before changes.
- [x] Implement and verify case-insensitive duplicate-username prevention.
- [ ] Enforce single academy-info/contact records in content-management logic.

## 3. Login and access control

- [x] Choose student self-registration; admin provisioning remains separate.
- [x] User applied the personal fields; live metadata comparison verified the updated table definitions.
- [x] Consolidate all applied changes into Schema.sql and remove the completed incremental script.
- [x] Refresh/verify the generated UserTable entity and mapping for the four new fields.
- [x] Build and verify an EF read of personal-detail columns without displaying account data or changing records.
- [x] Create registration view/model/action with names, optional birthday, username, password, and confirmation.
- [x] Force Student role server-side; validate inputs, reject duplicate usernames, hash passwords, and protect registration POSTs.
- [x] Implement an explicit, controlled first-admin creation flow using framework password hashing.
- [ ] Manually verify interactive first-admin password entry and create the intended university admin.
- [x] Add LoginViewModel and AccountController with login/logout actions.
- [x] Create labeled, responsive login/registration views matching the documented design.
- [x] Verify usernames/password hashes and reject inactive accounts with generic failure messages.
- [x] Configure cookie authentication and include user ID and role claims.
- [x] Add authentication middleware before authorization and protect content routes by default.
- [x] Restrict the current admin controller to Admin on the server; extend this to future mutation controllers.
- [x] Implement POST logout, anti-forgery checks, local return-URL validation, and access-denied behavior.
- [x] Define session expiration and make deactivated-account access consistent with the requirement.
- [x] Verify logged-out, active student, active admin, inactive, and invalid-login behavior.
- [x] Add and verify account POST rate limiting.

## 4. Shared interface and student-facing content

- [x] Build a consistent university header, navigation, footer, page titles, and logout control.
- [x] Obtain approval for the cleaned transparent crest and use it as the main logo in shared layouts and favicon.
- [x] Remove the student-access caption and verify normal login/signup forms fit desktop and phone viewports without scrolling.
- [x] Load Cormorant Garamond/Lato with fallback fonts for the implemented account layouts.
- [ ] Select and load the social icon library for content pages.
- [ ] Prepare approved preselected images, descriptive alternative text, and missing-image fallbacks.
- [ ] Render the hero background and established university branding.
- [ ] Render description, mission, vision, core value titles, and About image gallery.
- [ ] Render subjects/programs with descriptions and images.
- [ ] Render founder name, optional birthday, biography, and message using verified or clearly fictional content.
- [ ] Render address, email, telephone, office hours, and social links with icons.
- [ ] Add clear empty-content and database-error states without exposing account data or connection details.

## 5. Admin content management

- [ ] Build an admin navigation/dashboard linking to the content editors.
- [ ] Edit the single university-info record, including the hero image selection.
- [ ] Add, edit, and delete core value titles.
- [ ] Add, edit, and delete gallery image references and alternative text.
- [ ] Add, edit, and delete programs with descriptions and image selection.
- [ ] Manage founder details and the founder message.
- [ ] Edit the single contact-information record.
- [ ] Add, edit, and delete social platforms, safe URLs, and supported icon identifiers.
- [ ] Use dedicated form/view models, server validation, anti-forgery protection, and POST mutations.
- [ ] Handle missing IDs, delete confirmations, success/error feedback, and persistence after refresh.

## 6. Content and quality verification

- [ ] Populate approved university content after explicitly authorizing the data changes; no invented statistics.
- [ ] Test admin CRUD and student read-only behavior across all content types.
- [x] Test direct access to the current protected portal/admin routes and account form requests.
- [x] Test registration, optional profile fields, future birthdays, role tampering,
  password handling, duplicate usernames, inactive sessions, logout, and safe redirects.
- [ ] Test missing/invalid inputs, URL schemes, empty tables, and unavailable database behavior.
- [ ] Verify responsive layouts, mobile navigation, keyboard focus, labels, contrast, and reduced motion.
- [ ] Check image paths, icon rendering, links, official naming, and safe text rendering.
- [ ] Run the final build and meaningful authorization/data-access tests; record results and limitations.

## 7. Git, documentation, and delivery

- [x] Initialize Git and exclude IDE/build output, local secrets, and database data files.
- [x] Connect origin to Pixyz-bit/Ravenclaw-University and use main.
- [x] Push the initial MVC starter, schema, context documents, and project skill to GitHub.
- [x] Commit subsequent naming, configuration, EF Core setup, and diagnostic progress locally.
- [ ] Continue updating context/checklist documents and committing each completed, verified unit.
- [ ] Push later commits when requested; later local commits are not yet uploaded.
- [x] Document first-admin creation and repeatable authentication verification commands.
- [ ] Complete the fresh-checkout setup guide with tool restore and environment configuration.
- [ ] Verify a fresh checkout can restore/build and connect to a separately prepared database.
- [ ] Prepare a demonstration of student viewing and admin editing for submission.
- [ ] If publishing is requested: choose hosting, configure environment-specific secrets/database access,
  deploy after appropriate authorization, and verify the deployed application.

## Scope decisions, not automatic additions

- Repositories are optional; DbContext can be used directly for this small project.
- Enrollment, payments, grades, inquiry storage/email, uploads, password recovery,
  and additional account-management screens are not yet approved requirements.
- Preselected image selection does not require an upload system.
- Keep existing SQL identifiers and the simplified schema unless the user requests changes.
