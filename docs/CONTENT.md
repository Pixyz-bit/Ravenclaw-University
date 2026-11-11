# University content and administration

The first content implementation is one authenticated homepage with five sections:
hero, About, academics, contact, and founder information. Navigation links jump to
the corresponding sections. No enrollment, inquiry form, uploads, or extra tables
were added. Schema.sql remains unchanged and is the sole consolidated schema.

## Using the editors

Sign in as an Admin and open Administration. The sidebar links to seven sections.
The admin area has its own sidebar layout, separate from the university navbar.
Dashboard and the seven content pages are linked persistently, with the current
section highlighted. On phones the navigation can be expanded from its disclosure.

| Editor | Database table | Behavior |
| --- | --- | --- |
| University information | AcademyInfoTable | Create once, then edit description, mission, vision, and hero image |
| Contact information | ContactsTable | Create once, then edit address, email, telephone, and office hours |
| Core values | CoreValuesTable | Add/edit/delete individual titles |
| About gallery | AboutImagesTable | Add/edit/delete image paths and descriptions |
| Academic programs | ProgramsTable | Add/edit/delete subjects, descriptions, and images |
| Founder information | FounderTable | Add/edit/delete names, optional birthdays, biographies, and messages |
| Social links | SocialLinksTable | Add/edit/delete platform names, HTTPS URLs, and supported icons |

Each section now has one named controller, one form ViewModel, and one Index view.
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
| Social links | SocialLinks |

For example, follow Controllers/UniversityInformationController.cs to
ViewModels/UniversityInformationViewModel.cs and Views/UniversityInformation/Index.cshtml.
That form edits Models/AcademyInfoTable.cs. University information directly displays
all four content inputs; the identity ID is managed internally. Other collection
pages put the add/edit form and published entries in their own single Index view.
Old /AdminContent?section=University links redirect to /UniversityInformation;
old generic POST endpoints have been retired.

The university/contact editors enforce one record through a serializable check-and-
save transaction. Existing externally created duplicates are not automatically deleted;
the homepage and singleton editors use the lowest ID.
These two singleton sections cannot be deleted through the UI. Other deletes remove
only the database entry, never a physical image file.

## Images and links

Choose a path under /images/ or a complete HTTPS image URL. The approved crest is
available in the image field's suggestions. No other campus/program photographs
have been supplied or generated. Administrators can reference preselected assets
placed under wwwroot/images. Local path traversal and backslashes are rejected.
Image descriptions provide alternative text; unavailable images get a readable
fallback. Remote images are loaded by the browser, not fetched by the server.

Social links require HTTPS and use an allowlist of optional Font Awesome Free
6.7.2 icon classes (Facebook, Instagram, YouTube, LinkedIn, X, website). The shared
layout loads the stylesheet from cdnjs; platform names remain readable if the CDN
is unavailable. No arbitrary HTML or icon markup is accepted.

Text is rendered with Razor encoding and preserved line breaks, not Html.Raw.
Validation follows SQL lengths, required content titles, email/phone formats,
supported icons, and nonfuture founder birthdays using Asia/Manila's date.

## Reading and errors

UniversityContentService reads all seven content tables using AsNoTracking and
sequential queries on one DbContext. It never queries user accounts for content.
HomeController renders UniversityPageViewModel with clear unpublished/empty states.
No founder dates, campus history, statistics, or programs were seeded into the
university database. The approved crest is the fallback hero visual.

Empty content remains a valid page. Database-operation errors show generic retry
feedback and HTTP 503 rather than exception details. Authentication continues to
fail closed when account validation cannot reach the database.

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
Verified 2026-11-11: 104 checks passed after splitting all seven MVC editors, with
zero build warnings/errors. The checks exercise every dedicated route, student and
anonymous rejection, creation/editing, singleton updates, and safe delete behavior.
University and program forms were inspected on desktop/390px phones without
horizontal overflow. Final full accessibility and database-outage QA remain pending.

Use -Preview to keep the disposable server available for visual inspection after
checks. It prints a test-only URL and fixture login; press Enter afterward to stop
the server and drop its GUID-named test database. In redirected/noninteractive
execution, it prints PREVIEW_STOP_FILE; create that exact file to request cleanup.
Never use those fixture credentials
as university credentials. The university database is not modified by verification.
