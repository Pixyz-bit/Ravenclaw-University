# University content and administration

The approved generated subject images are now stored in all six existing program
records: Astronomy, Charms, Transfiguration, Potions, Herbology, and Defence Against
the Dark Arts, using /images/programs/ paths. Their names/descriptions were preserved.
Academic Programs remains the editor
for future image replacements; no new database columns or rendering overrides exist.

As of 2026-11-14, the university gallery and The Hogwarts Collegiate Circle are
temporarily hidden on the public homepage. Academics leads directly to the founder
section. Their partials, assets, styles, scripts, and gallery editor/data remain
intact; the homepage script includes and footer Gallery link are also hidden.

The authenticated homepage has hero, About, academics, a drifting gallery, contact,
and founder information. Navigation links jump to
the corresponding sections. Admin image uploads and account management are now
implemented. No enrollment, inquiry form, or extra tables were added. Schema.sql
is the sole consolidated schema and includes the approved heading/office-time additions.

The gallery's latest treatment is flat and page-scroll driven across a 200dvh
section, with a viewport-sized sticky stage below navigation. Column offsets follow
scroll progress rather than elapsed time; idle scrolling tracks do not advance.
Perspective, rotation, and pointer parallax are removed. Existing image sources,
dialogs, keyboard access, and reduced-motion/static fallbacks remain.

The standalone gallery follows Academics and reuses AboutImagesTable through the
existing About gallery editor. When no safe gallery images exist, the approved
university illustrations appear. The original DriftWall reference is adapted to
Razor, Tailwind, and vanilla JavaScript with the latest scroll-driven treatment;
selecting an image opens its full-size preview.
Keyboard and reduced-motion browsing use an ordinary image grid. No new database
table, controller, or React runtime is needed for this read-only section.

The Hogwarts Collegiate Circle follows the gallery. It is a static illustrated
five-pennant display with Hogwarts in the center, not an additional database table
or editor. The five new generated assets are in wwwroot/images/collegiate; their
prompts and provenance are in docs/GENERATED_ASSETS.md.

Contact is the full-width navy footer after Leadership, replacing the standalone
contact section and small university footer. Desktop has three unequal columns:
university identity, Explore (Home/About/Academics/Founder/Contacts), and
contact details. The large motto and Our University links have been removed.
Copyright and Back to top sit below one divider. Columns stack at 1000px and narrower in identity, Explore, Contact order. The Contacts editor supplies the published values. _UniversityContact.cshtml owns markup and sections/contact.css
its Tailwind styling; no database changes were needed.

## Using the editors

Administration opens Hero and About directly; the dashboard and content inventory
were removed at the user's request. Only Academics has a local draft side preview
(stacked on smaller screens). Other editors use full-width grouped forms with
related fields in responsive columns and inline upload previews. All group legends
are visually hidden but accessible. Edit-status lines and the account-limit/office-hours
hints are removed. Typing and image selection remain local until saved. Use Save to persist through the existing
controller. Saved program/gallery entries show thumbnails; text collections show
short summaries. Dirty edits warn before navigating away, searching accounts, or
signing out; Keep editing/Escape retains the draft, while Discard changes continues.
Refresh/close uses the browser warning. Inline validation and server-side permissions,
antiforgery, upload checks, and delete confirmations remain in force.

The approved Rowena Ravenclaw biography was saved on 2026-11-14 through the existing
FounderTable content fields. It describes her canonical Hogwarts/Ravenclaw House
background, then connects her legacy to this fictional university. The public
section uses an ivory portrait/text composition, with a responsive upper-body CSS
crop. Founder editing covers the name and biography only. Founder birthday and
message/quote fields are removed from the application and live database. The fixed
quote “Wit beyond measure is man's greatest treasure” appears beneath the university
brand in the contact footer, rather than beneath the founder biography.
The founder uses the approved static portrait regardless of the saved name or
biography. Its alt text describes the image without depending on the editable name.
Schema.sql and database definitions are unchanged.
The optional founder content script was removed on 2026-11-18 at the user's request.
Manage the saved founder through the admin editor.

The approved Ravenclaw-inspired About, Mission, and Vision copy was saved to the
local database on 2026-11-12. Core values are Intelligence, Wisdom, Creativity,
and A Lifelong Love of Learning. About now starts directly with “A place for curious
minds”; the separate quote/header and admin notice have been removed. The closing
sentence beginning “Here, every perspective” was removed from the approved copy.
These are tentative university website contents.
The optional university content script was removed on 2026-11-18 at the user's
request. Manage these saved values through the admin editor.

Sign in as an Admin and open Administration. The sidebar contains
Hero and About, Academics, Founder, Contacts, and Account Management, in that order. The crest also opens
Hero and About. Old /Admin links redirect there. The admin layout is separate from the university navbar, highlights
the current section, and has expandable navigation on phones.

Hero and About edits the saved hero image, About Heading/Description, Mission,
Vision, and all Core Values together. Enter one value per line (up to 100 characters
each); blank lines are ignored, removing lines removes values, and clearing the
textarea clears the list. Values remain individual records and render as bullets
on desktop/mobile. The entire save is transactional. Text remains HTML encoded.
An empty heading uses the established “A place for curious minds” fallback.

Contacts uses separate Time In/Time Out inputs: supply both or neither. The footer
formats the saved pair, labels overnight ranges, and falls back to retained legacy
office-hours text when both are empty. About uses fixed university illustrations;
the About images disclosure is removed from Hero and About. Its text remains editable. Existing
dedicated editor URLs remain usable. Founder edits update only the name and biography.

| Editor | Database table | Behavior |
| --- | --- | --- |
| Hero and About | AcademyInfoTable / CoreValuesTable | Create/edit university details and replace the value list in one save |
| Contacts | ContactsTable | Create/edit contact details and paired office times |
| Core values (legacy route) | CoreValuesTable | Individual title editor retained for existing links |
| About gallery | AboutImagesTable | Add/edit/delete image paths and descriptions |
| Academic programs | ProgramsTable | Add/edit/delete subjects, descriptions, and images |
| Founder information | FounderTable | Edit the one existing founder name and biography |

Each section now has one named controller, one form ViewModel, and one section-named view.
The matching database-first EF entity remains in Models/. Every controller requires
the Admin role. Fields map explicitly to that section's entity. Save/delete require
POST and antiforgery validation. GET delete displays a confirmation without writing.

| Section | Controller / ViewModel / view folder prefix |
| --- | --- |
| University information | UniversityInformation |
| Contact information | ContactInformation |
| Core values | CoreValues |
| About gallery | AboutGallery |
| Academic programs | AcademicPrograms |
| Founder information | FounderInformation |

For example, follow Controllers/UniversityInformationController.cs to
ViewModels/UniversityInformationViewModel.cs and Views/UniversityInformation/UniversityInformation.cshtml.
That form edits Models/AcademyInfoTable.cs. University information directly displays
all four content inputs; the identity ID is managed internally. Other collection
pages put the add/edit form and published entries in their own section-named view.
Old /AdminContent?section=University links redirect to /UniversityInformation;
old generic POST endpoints have been retired.

The university/contact editors enforce one record through a serializable check-and-
save transaction. Existing externally created duplicates are not automatically deleted;
the homepage and singleton editors use the lowest ID.
These two singleton sections cannot be deleted through the UI. Other deletes remove
only the database entry, never a physical image file.

## Images and links

The hero uses only the user-selected Ravenclaw common-room image at
wwwroot/images/hero/ravenclaw-common-room.png, copied byte-for-byte from the latest
attachment. The user superseded the gallery decision: there are no image selectors,
extra slides, or automatic rotation. HeroImg uploads remain stored but do not override
this explicitly selected background. Other generated scenes are retained as unused
assets. No live database updates are needed.

Hero, gallery, program, and account-profile fields use multipart file uploaders
with local previews. Choose PNG, JPEG, or WebP files up to 5 MB. The server checks
extensions, file signatures, and size; SVG/HTML and mismatched signatures are rejected.
Existing images are preserved when no replacement is selected. New gallery entries
require a file. Image-path form values cannot override server-managed stored paths.
Existing safe local/HTTPS image references still render, but the editors no longer
ask the admin to enter paths. Gallery descriptions provide alternative text.

ImageUploadService saves UUID-named files in App_Data/Uploads, exposed under
/images/uploads with a nosniff response header. Files are public website media.
ImageUploads:RootPath can point to persistent storage; back up that directory along
with the database. Uploads are excluded from Git. The database retains only paths,
so no SQL change is required. Failed database writes discard newly created files;
successful replacements/deletions retain old files to avoid removing shared media.
Disposable verification overrides the upload root to its own temporary directory.

Social links were removed from the application and the empty live table on 2026-11-16. Their controller, model, view, service query, validation, icons, and footer UI no longer exist.

Text is rendered with Razor encoding and preserved line breaks, not Html.Raw.
Validation follows SQL lengths, required content titles, email/phone formats,
and nonfuture account birthdays using Asia/Manila's date.

## Reading and errors

About now follows the reference's sticky desktop composition: four chapters show
the stored introduction, mission, vision, and core value titles. Four fixed approved
university illustrations supply its images, independent of gallery uploads. On
desktop there is one image per chapter. On smaller screens, the image sits above
the text in a sticky stage; ordinary vertical page scrolling changes chapters in
both directions, without a swipe rail or dots. Long prose remains scrollable and
keyboard accessible. No-JS mobile content uses a readable vertical chapter list.

Academics uses a dark looping row of portrait image cards from ProgramsTable,
with edge fades, image-zoom/arrow hover feedback, drag/swipe navigation, and native
description disclosures. Motion pauses on hover/focus/open cards, manual pause, and
reduced-motion preference. Duplicate visual laps are hidden from screen readers.
Program images disable native browser dragging so mouse drags scroll the rail.
Moving more than five pixels suppresses the following click; ordinary clicks still
open descriptions. Release, pointer cancellation, window blur and a released-button
move end the drag. Touch scrolling retains the browser's native behavior.
The user separately authorized six sample programs in the university database on
2026-11-12. At the user's request on 2026-11-16, the "Sample program:" prefix was
removed from all six saved descriptions and the then-existing seed; the remaining copy was preserved.
Subjects can be edited or deleted in Admin. The optional program seed and image-update
scripts were removed on 2026-11-18 at the user's request. -IncludeContent -Preview
adds clearly labelled programs to its disposable database after integration checks,
so the visual preview can exercise scrolling even when production programs are empty.

UniversityContentService reads the six content tables using AsNoTracking and
sequential queries on one DbContext. It never queries user accounts for content.
HomeController renders UniversityPageViewModel with clear unpublished/empty states.
No founder dates, campus history, statistics, or programs were seeded into the
university database. The saved hero image is used when safe, with the approved
common room as fallback.

Empty content remains a valid page. Database-operation errors show generic retry
feedback and HTTP 503 rather than exception details. Authentication continues to
fail closed when account validation cannot reach the database.

## Account management

The directory fills the admin workspace and uses flat divided rows with profile
photos/initials, name/username hierarchy, role text, labeled status dots, and aligned
Activate/Deactivate and View information actions. The signed-in admin's Your account
badge appears beside their name, with no self-deactivation action. Redundant page
captions and analytics headings are removed; the global totals remain above search.

/AccountManagement is Admin-only, with its own controller, form ViewModel, and
section-named view. Search by username or name (up to 100 results), then edit names,
username, optional birthday, role, active status, or upload a profile image.
Accounts are created through student self-registration/the existing first-admin
command. This screen does not create/delete accounts or reset passwords.
Password hashes never enter the view model or rendered HTML and remain unchanged
when editing. Duplicate usernames are checked case-insensitively in a serializable
transaction. Admins cannot deactivate/demote themselves; at least one active Admin
must remain. Changed roles/status invalidate existing sessions through the existing
cookie validation. All updates require POST and antiforgery validation.

## Verification

From the repository root, run:

```powershell
.\scripts\Verify-Authentication.ps1 -IncludeContent
```

This builds into a separate temporary output folder, creates a disposable database
from Schema.sql, and starts a temporary local server without locking the user's
normal build. In addition to the 28 authentication checks, it verifies content CRUD,
student rejection on GET/POST, singleton protection, required fields, HTTPS links,
image-path validation, missing IDs/sections, antiforgery, and stored-text escaping.
Verified 2026-11-11: 126 checks passed after adding uploads and Account management,
with zero build warnings/errors. Checks exercise every dedicated route, student and
anonymous rejection, creation/editing, singleton updates, safe delete behavior,
multipart uploads and preservation, size/type rejection, account updates, search,
role/status rules, self-lockout protection, and unchanged password hashes.
Account list/edit, sidebar, and hero/gallery/program upload forms were inspected on
desktop/390px phones without horizontal overflow. Selecting a local profile file
displayed a loaded image preview without submitting it. Final full accessibility
and database-outage QA remain pending.

Use -Preview to keep the disposable server available for visual inspection after
checks. It prints a test-only URL and fixture login; press Enter afterward to stop
the server and drop its GUID-named test database. In redirected/noninteractive
execution, it prints PREVIEW_STOP_FILE; create that exact file to request cleanup.
Never use those fixture credentials
as university credentials. The university database is not modified by verification.
Cleanup removes that run's temporary upload directory as well as its SQL database;
the resolved upload target is checked against the GUID-named test directory first.

Founder is edit-only: its default GET loads the existing record, and POST requires that exact ID. Creation, deletion and collection controls are absent. Empty storage shows an explanatory state; duplicate records fail closed. The public view model holds one Founder instead of a list. No live founder data was changed.

My profile has a desktop identity/photo column on the left and Personal details /
Change password tabs on the right, stacking on phones. Personal details edits names,
birthday and the selected photo; password changes use a separate form with current,
new and confirmation fields. Tabs retain local drafts. Each form has its own Cancel
and save action. Personal Cancel restores persisted values after failed validation;
password Cancel clears and remasks only its own fields. All password inputs have
shared eye controls. Captions, instructions and draft-state text are removed; inline
validation and navigation/sign-out/refresh protection remain. Password behavior and
its isolated verification are documented in AUTHENTICATION.md.

Admin identities consistently use the shared navy/ivory ADMIN badge in the account
menu, Profile, account directory and account information. Students have no admin
badge. Profile reads the authenticated role; account views use the displayed role.
