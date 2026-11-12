# Current status

Last verified: 2026-11-12.

## Completed

- Removed DriftWall's blue image overlays, canvas fade masks, and section-wide navy
  fade at the user's request. Images now render at full opacity and original color;
  a localized neutral-charcoal fade behind the heading preserves readability.
  Removed unused overlay markup/styles. Tailwind compilation, JavaScript syntax,
  source checks for removed tint/mask rules, and diff whitespace checks passed.

- Preserved the user's exact full Hogwarts crest as images/collegiate/hogwarts-crest.png
  (800×800 RGBA, transparent background); verified byte-for-byte matching SHA-256.
  The image service rejected both a standalone recreation and an edit replacing the
  center banner's H-only shield. No generated replacement or banner change was made.

- Added The Hogwarts Collegiate Circle after Gallery, adapting the supplied hanging
  flags layout with a curved gold rope and five newly generated matching pennants.
  Order is Gryffindor, Slytherin, Hogwarts, Ravenclaw, Hufflepuff; Hogwarts is centered
  and slightly larger. Its original quartered H-monogram crest matches the set.
  All five PNGs are 793×1983 RGBA with transparency, stored under images/collegiate.
  GENERATED_ASSETS.md records the built-in image-generation prompts and provenance.
- Made Gallery fill 100dvh minus the sticky navbar, with heading overlay and columns
  scaled to its full width. Keyboard image browsing remains a visible static grid.
  Browser checks at 1366×768 and 375×667 verified matching section/wall bounds from
  the navbar bottom to viewport bottom, without horizontal document overflow.
- All 126 isolated checks and build passed with zero warnings/errors. Browser QA
  verified all five banner images load, central Hogwarts placement, phone centering
  and native arrow-key scrolling, plus the full-height gallery's keyboard fallback.
  Subsequent image dimension metadata was corrected to the verified PNG dimensions.

- Added a standalone DriftWall gallery after Academics, adapted from the user's
  supplied reference to Razor, Tailwind, and vanilla JavaScript. It uses About
  gallery uploads or approved illustration fallbacks, five drifting columns on
  desktop/three on phones, perspective/parallax, edge fading, and hover lift.
  Native image dialogs support Close/Escape; keyboard focus reveals a static
  image grid. Reduced motion disables enhancement; offscreen/hidden tabs stop RAF.
- Gallery build and all 126 isolated checks passed with zero warnings/errors.
  Browser checks at 1366×768 and 375×667 verified column counts, changing track
  transforms, no horizontal overflow, loaded full-size image dialogs, Close/Escape,
  and keyboard Tab/Enter access through the visible grid. The final clip-path reset
  and image visibility refinements were checked by Tailwind build and browser QA.

- Sized Academics to 100dvh minus the sticky navbar using a two-row grid. Heading,
  padding, gaps, and carousel cards scale to available height; card titles clamp to
  two lines and full titles/descriptions remain available when opened. Corrected
  mobile fragment timing by synchronizing navbar/hero height before scrolling.
  Browser checks at 1366×600, 1280×480, and 375×667 verified the section starts at
  the navbar bottom and ends at the viewport bottom (within subpixel rounding),
  with no horizontal document overflow. Phone description opening also passed.
  Tailwind build and 126 isolated checks passed; the subsequent mobile timing fix
  was verified by browser checks and JavaScript syntax validation.

- Made the public navbar sticky at the viewport top on desktop and mobile. Shared
  measured header height now supplies section scroll padding and About's minimum
  sticky offset. The keyboard skip link stays above the navbar. No scroll snapping
  was added. Browser checks at 1366×600 and 390×844 verified the navbar remains at
  top=0, desktop About images/text clear the header, section links remain usable,
  and the mobile menu closes after navigation without horizontal overflow.
  Tailwind build, JavaScript syntax check, and all 126 isolated checks passed.

- Converted application styles to Tailwind CSS v4 with reusable @apply files in
  Styles/base, components, layouts, and sections. Added locked npm dependencies,
  CSS build/watch commands, automatic dotnet build integration, and a source guide.
  Compiled site.css is generated/ignored; unused starter scoped CSS was removed.
- Scaled the full desktop About image pair and text against the dynamic viewport,
  accounting for the lower inset. Verified fit at 1366×600 and 1280×480 before
  conversion, and 1366×600 after conversion. Long prose retains internal scrolling.
- Removed the visible Pause motion control at the user's request. Academics keeps
  automatic looping, hover/focus/open-description stops, drag/swipe, and the system
  reduced-motion preference. Browser measurements confirmed the track advances.
- Tailwind build and all 126 isolated checks passed with zero build warnings/errors.
  Browser checks verified hero/navbar height, About composition, mobile About rail,
  desktop/mobile admin editors, and registration. No horizontal overflow was found;
  registration exactly matched 1280×720 and 375×667 viewport dimensions.
  Release publish succeeded and includes the generated CSS. The main local app
  and stylesheet both returned HTTP 200 at localhost:5182 after restart.

- At the user's explicit request, added six sample programs to the local university
  database on 2026-11-12: Astronomy, Charms, Transfiguration, Potions, Herbology, and
  Defence Against the Dark Arts. The table was empty; SQL confirmed six inserts
  with sample descriptions and existing illustration paths. SeedPrograms.sql is an
  optional idempotent content script; Schema.sql and table definitions are unchanged.
  Shared-memory prelogin timed out; the local named-pipe connection succeeded.
- Implemented reference-based About and Academics layouts after inspecting the live
  Digital Original site and pasted markup. About uses four sticky scroll-switched
  chapters with overlapping image pairs and mobile swipe/dots. Academics uses a
  looping portrait-card strip, hover zoom/arrow feedback, edge fades, drag/swipe,
  hover/focus/reduced-motion handling, and program-description disclosures.
- Build and 126 isolated checks passed. Browser checks verified About chapter/image
  changes, mobile dot navigation, program scrolling/descriptions, and no horizontal
  overflow at 1280px/390px. Minor final spacing/edge-fade and resize alignment changes
  were verified by a subsequent build. Preview fixture content is disposable only.
- Replaced the hero gallery with only the latest user-attached common-room image,
  verified byte-for-byte by SHA-256. Removed slideshow controls/logic/styles and
  kept the navbar-height observer in hero-layout.js. Existing database records and
  uploaded files were unchanged; the selected static image takes precedence.
  Build passed with zero warnings/errors. Prior SQL-related visual verification
  limitations remain; this update was checked by build/source/asset verification.
- Integrated the four approved generated hero scenes, excluding the first rejected
  common-room version. Manual selectors, lazy image loading, pressed/status feedback,
  keyboard navigation, and reduced-motion fades preserve the established hero copy.
  A custom admin upload remains an additional slide. No database changes were made.
- Verified all four images load and switching/keyboard navigation works; desktop
  and phone gallery checks had no horizontal overflow. All 126 checks passed before
  the subsequent combined-navbar/hero sizing adjustment.
- Updated hero sizing to subtract the measured navbar height from 100dvh, with
  ResizeObserver updates, responsive heading size/spacing, and reserved control space.
  Final build passed with no warnings/errors and the app was started on localhost:5182.
  Final viewport measurements remain unverified: fresh disposable previews failed
  on a SQL login timeout and a later 20-second HTTP/database timeout. The latter
  preview cleaned up its database/files; no university records were changed.
- Updated the hero to the supplied reference composition: full-width, minimum
  100dvh height, uploaded background, left-dark/right-light overlay, left-aligned
  hero header and description, and no CTA buttons. The navbar is unchanged.
  Subsequent sections retain their centered reading width; no schema changes.
- Build and all 126 checks passed. Browser measurements verified 720px/844px/480px
  hero heights at matching desktop/phone/short viewports, no horizontal overflow,
  and zero hero links/buttons. Disposable preview cleanup completed.
- Completed desktop (1280px) and phone (390px) visual checks of Account management
  list/edit, sidebar navigation, and hero/gallery/program image uploaders. All nine
  admin links remain visible on desktop; the sidebar disclosure works on phones.
  No horizontal overflow or removed topbar/number prefixes/divider were present.
  Selecting the approved crest in the profile picker displayed a loaded local
  preview; it was not saved to university data. All 126 isolated checks passed again.
- Extended disposable verification cleanup to remove its guarded upload directory.
  The follow-up 28 account checks passed, and cleanup removed the test database and
  upload directory. The prior preview process/database/files were also removed.
- Built the latest uploads/account-management changes with zero warnings/errors
  and started the local development app on localhost:5182 on 2026-11-12.
  Login returned HTTP 200.
- Removed admin sidebar numbers and University content divider label, and removed
  the admin-topbar header at the user's request. Navigation retains active states.
- Added file uploaders/current and selected previews for hero, gallery, programs,
  and account profiles. Existing images persist without replacements. Server checks
  PNG/JPEG/WebP extensions, signatures, and 5 MB size; paths are server-managed.
- Added a dedicated Admin-only Account management MVC page with search, names,
  username/birthday edits, profile uploads, role/active management, duplicate-name
  validation, and self/last-admin protection. Password hashes remain unchanged.
- Build and all 126 isolated account/content/upload checks passed. All account,
  content, and upload mutations were confined to disposable test data/storage.
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

- Replace the Hogwarts center banner's H-only emblem with the full uploaded crest.
  The original transparent logo is saved, but both built-in generation/edit attempts
  were rejected. The existing center banner is still the H-monogram version.

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
