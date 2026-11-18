# Design direction

Authority: the user's supplied blueprint and later corrections. The official
institution and website name is Ravenclaw University; earlier naming is superseded.
Status: account layouts, the five-section university homepage, and protected content
editors are implemented. Real content and campus/program imagery still need entering.

## Identity

Application styling uses Tailwind CSS v4. Reusable @apply components are separated
under Styles/base, Styles/components, Styles/layouts, and Styles/sections inside
the MVC project. Styles/app.css imports them; base/theme.css owns color/font tokens.
Follow Styles/README.md for builds. Edit these sources, never compiled site.css.
The conversion preserves the existing palette, typography, and responsive behavior.
Tailwind Preflight is omitted to retain established browser defaults.

Use a modern dark-academia institutional style with historic architectural cues,
clear academic content, generous spacing, and restrained ornamentation.

| Token | Value |
| --- | --- |
| Institution name | Ravenclaw University |
| Midnight navy | #101D34 |
| Royal blue | #244468 |
| Antique gold | #C3A568 |
| Ivory | #F4F0E6 |
| Charcoal | #343B45 |
| Heading font | Cormorant Garamond (500/600), Georgia fallback |
| Body font direction | Lato |
| Content width | Approximately 1200px |
| Motto | Where Wisdom Takes Flight. |

Cormorant Garamond and Lato load through Google Fonts, with local serif/sans-serif
fallbacks. The approved university crest is installed; content imagery remains pending.
Social links and their icon CDN were removed on 2026-11-16. Gold is primarily a decorative accent;
ensure readable text contrast rather than assuming every palette pairing works.

## Shared public typography (2026-11-12)

Public content uses reusable roles in Styles/components/typography.css and scale
tokens in Styles/base/theme.css. Use the same roles for future university sections:

| Role | Treatment |
| --- | --- |
| Hero h1 | Existing Cormorant Garamond display/italic motto, 44–96px with viewport scaling |
| Section h2 | university-section-title: Cormorant Garamond 500, 36–56px, 1.12 line height, -0.025em tracking |
| Content h3 | university-subtitle: Cormorant Garamond 600, 24–32px, 1.2 line height |
| Main body | Lato 400, 17–18px, 1.75 line height, at most 65ch; About's narrow column retains 40ch |
| Small labels | Lato 700, 12px, 1.5 line height, 0.14em tracking |

The section-title scale responds to both viewport width and height so About and
Academics remain usable on short screens. Explicit leading in the shared classes
prevents inherited Tailwind line-height variables from overriding the role tokens.
All public section h2s share the role, including both About variants, Academics,
Gallery, Collegiate Circle, and Leadership. Footer Contact uses the secondary
24–32px scale. Program-card controls, footer navigation, forms, account pages,
and operational admin headings retain their compact Lato UI hierarchy.

Section labels use a consistent 12px gap before titles, with 20px between ordinary
section titles and supporting copy. About retains its responsive panel spacing.
No section height, animation, content, data source, or route was replaced.

## Composition

Navigation -> Hero -> About -> Academics -> Gallery -> Collegiate Circle ->
Leadership -> Contact footer. The implemented version uses one
authenticated homepage with section anchors; separate detail pages remain optional.

Gallery and The Hogwarts Collegiate Circle are temporarily hidden at the user's
request (2026-11-14). The visible flow now goes directly from Academics to Leadership.
Their partials, styles, scripts, assets, and gallery editor/data are retained.
Homepage script includes and the footer Gallery link are hidden with the sections;
restore them together when requested.

The public navbar order is Home, About, Academics, Founder, Contact. Founder keeps
the existing #leadership anchor; desktop and the collapsed phone menu share this
same markup and order.

- Full-width navy university navbar, navy admin sidebar and footer; alternate navy and ivory content sections.
- Large architectural hero image with readable overlay and left-aligned headline.
  Use Ravenclaw University consistently in the logo text, headings, navigation,
  page titles, login screen, and footer when those surfaces are implemented.
- Thin gold dividers, consistent image proportions, and subtle hover effects.
- Two-column program layouts and formal founder/profile compositions.
- On narrow screens, stack columns and provide accessible menu controls.
- Use visible field labels, keyboard focus, clear feedback, and reduced-motion support.

Use admin-selected image uploads for editable imagery. Do not generate imagery unless the user asks.
Preserve established tokens and reusable components across student and admin
screens; admin screens should prioritize clear editing tasks. Later explicit user
choices override this planning reference and should be recorded here.

## Implemented account surface (2026-11-11)

Shared account layout uses a navy editorial panel with the approved university crest,
motto, and fine architectural arch; ivory form panel uses visible labels and a
navy primary action. Below 850px the story panel reduces to the wordmark.
Registration retains compact two-column name/password rows on phones; below
350px those rows stack for readability. Login controls are at least 48px high;
registration controls are at least 44px high. Errors are inline and textual, and
there is a keyboard skip link. The university navbar uses the established identity:
brand at left, Home/About/Academics/Contact/Leadership links at right, and a circular
account control at right. Its dropdown has the username, Admin-only Administration
link, and POST Sign out. Use a full-width midnight-navy background, ivory text,
and a fine gold bottom rule. Align the inner content to the 1200px page container.
Below 1001px, a native hamburger/Menu disclosure shows the navigation links.
On the university homepage, the public navbar is fixed over the full-height hero
with a transparent background and no bottom rule. Links and branding remain visible.
As another section reaches the navbar (including anchor navigation), it becomes
solid navy with its gold rule; returning Home restores transparency. An expanded
phone menu keeps a solid navy surface for readability. Other pages/no-JS retain
the existing navbar behavior. Once at least 50% of the contact footer's height is visible,
the homepage navbar slides out and becomes inert (unfocusable); it returns when
less than 50% remains visible. Back to top restores the transparent hero state. Reduced
motion disables the slide transition. Without JS, keep
the solid sticky navbar. Its measured height supplies anchor scroll padding and the minimum About
sticky offset so headings and images remain visible beneath it. Mobile menu height
changes are observed; page scrolling remains natural without section snapping.
Desktop and mobile forms were visually inspected; later content sections must
reuse these tokens consistently.

## University content and editors

The homepage opens with a full-width 100dvh hero beneath the transparent overlaid
navbar, so both fit in one viewport. Without JavaScript it retains the solid-navbar
height fallback. The selected common-room image covers the background; a navy-black overlay fades
from dark on the left to lighter on the right. A left-aligned hero header and short
description sit over the image, vertically centered in a 1200px inner container.
Keep the established serif/italic ivory-and-gold motto and Lato supporting text.
The hero uses the image saved in Hero and About, falling back to the approved
Ravenclaw common-room image when no safe saved image exists. It remains a single
static background, with no CTA buttons, extra slides, or rotation. A ResizeObserver tracks navbar height so hero and navbar fit together. Heading size and spacing scale with width and height. CSS supplies navbar-height fallbacks without JavaScript. The hero may grow on short screens/zoom rather than clip text. Subsequent content retains the centered
1200px reading width and ivory sections, thin dividers, mission/vision and program layouts,
gallery images, contact details, and founder biographies/messages. Unpublished content
is labeled rather than invented. Razor encodes text and retains paragraph breaks.
Navigation wraps on smaller screens; page sections stack below 700px.

## About and Academics reference (2026-11-12)

Approved academic-card imagery (updated 2026-11-16): all six subjects use their
generated /images/programs/ assets via
the existing database ImagePath field. Existing cover cropping and hover behavior
remain. Herbology uses the mandrake greenhouse image; Transfiguration uses the
wand/parchment still life, both matched to the established muted cinematic style.

About begins directly with the saved About Heading, falling back to “A place for
curious minds” when empty. Core Values uses a semantic bulleted list on desktop
and phones, with the same Lato body scale and spacing. The separate About label,
quote introduction, and student-facing admin notice have been removed. There is no
introductory top gap. The section uses the full page width, with edge-to-edge images
on desktop and mobile and responsive inner padding for text.

The user supplied Digital Original's Creator showcase and How it works sections,
and explicitly requested their layout/scroll/hover treatment. The 2026-11-12
visual-unification request supersedes their separate warm-white/charcoal palette:
About uses the university ivory/ink tokens and Academics uses midnight navy,
ivory text, and gold accents. Secondary text on navy uses the shared on-navy
token (#D0D5DF), including the gallery. Image colors remain unchanged. About
retains the user's full-width equal split and mobile edge-to-edge images.

About presents four database-backed chapters: introduction, mission, vision, and
core values. On desktop, a full-width 50/50 composition keeps text on the left and
one edge-to-edge image on the right. Both panels remain sticky below the navbar as
vertical scrolling crossfades the active chapter. Each image fills its entire half
using object-fit:cover; there is no inset/overlapping photo. Below 1024px, a swipeable
snap rail and dots retain one image above each text chapter. About uses the four
fixed approved university illustrations; gallery uploads do not override them.
Text remains editable through Hero and About. No-JS content remains
readable; reduced motion disables transition effects.

The desktop story spans the full available page width with equal columns and no
gap. Panel height is 100dvh minus the measured navbar height, and sticky top equals
that navbar height. The story starts immediately after the hero.
Smaller desktop heights scale text spacing; long prose scrolls inside its text area
rather than pushing controls below the viewport. Chapter count, scroll markers,
crossfades, mobile dots, and reduced-motion behavior are retained.

The chapter numbers, total count, and decorative gold line have been removed on
desktop and mobile. Invisible scroll markers remain for chapter transitions.
The About text panel uses ivory, larger navy serif titles, and comfortable Lato body
text. About chapter action links were removed on desktop and mobile at the user's
request on 2026-11-16. Mobile slide-navigation dots remain. Typography and spacing scale with height
so the panel remains usable on short desktop screens. Existing content stays unchanged.
On phones the rail height follows the active chapter; switching from long copy to
a shorter chapter keeps the selected content in view instead of leaving blank space.

Academics uses portrait program-image cards on a full-width dark section, with
bottom titles, descriptions on activation, 5% image hover zoom over 700ms, and
light circular arrow hover feedback. Its track loops slowly while visible, pauses
on hover, focus, open descriptions, or reduced-motion preference,
and supports drag/swipe. Duplicate visual laps are hidden from assistive technology.
The user requested automatic motion without a visible pause control. All program data
continues coming from ProgramsTable. The user authorized six sample subjects on
2026-11-12; on 2026-11-16 the user requested removing the "Sample program:" prefix
from every description. The live descriptions and optional seed now omit it.

Academics shares one viewport with the sticky navbar, using 100dvh minus its
measured height. Above 700px wide and 500px tall, academics-stage sticks below
the navbar while its outer section provides the same clamp(160px, 30dvh, 320px)
scroll runway as Founder. It releases at the following section. A two-row grid
reserves space for the heading and gives the remaining height to the carousel.
The heading and rail share clamp(24px, 5vw, 96px) horizontal gutters. Vertical
padding is clamp(20px, 3dvh, 36px), with a clamp(16px, 2.5dvh, 28px) row gap;
phones use 24px vertical padding and a 20px gap. The former heading width cap
no longer offsets it from the carousel. Cards retain the 8:11 ratio, 550px height
cap, and 220px minimum width. Captions show up to two title lines; activation
reveals the full title and scrollable description. Looped-card navigation and
resize alignment read the actual rail padding instead of a fixed 24px offset.
Phones and windows at most 500px tall use natural vertical flow with a
clamp(340px, 58dvh, 550px) carousel. The section anchor retains its navbar offset.
Automatic motion, hover/focus/touch reveal, drag/swipe and reduced-motion behavior
remain established. Sticky positioning adds no scroll interception.

### Program card reveal (2026-11-12)

The latest user request supersedes the old arrow, View description label, and
separate description panel. Resting cards show only the subject name. Hover,
keyboard focus, or the native touch/details toggle raises the name and description
together over a bottom-heavy navy gradient fading upward. Caption transforms use
500ms ease-out; description/gradient opacity fades smoothly. Wrapped title heights
are measured for originals and carousel copies so the resting name stays visible.
Long descriptions scroll, can receive keyboard focus, and remain database-backed.
Reduced motion removes these transitions. Automatic program browsing is retained.
On mobile/touch, tapping the image toggles the native details open state to reveal
the description; tapping again closes it. Focus-based reveals use focus-visible
so ordinary touch focus does not hold a closed card open. Keyboard users still
reveal the caption when navigating to the summary or description.

## Founder portrait (2026-11-14)

The approved ivory portrait reference replaces the previous plain founder list.
Leadership retains its navigation anchor and sits between Collegiate Circle and
the contact footer. A shared section-title “Our founder” sits alone above a fine
gold divider; the adjacent university wordmark was removed. Desktop places the
portrait left and database-backed name, Founder role, biography, and Rowena's
existing motto right. The biography uses the shared Lato body scale; the name
uses the shared Cormorant Garamond subtitle. Phones stack portrait above text.

The approved transparent Rowena portrait is preserved unchanged in
wwwroot/images/founders/rowena-ravenclaw.png. Desktop uses shared grid rows so
the portrait begins level with Rowena's name and extends to the section bottom.
The image is cropped from its top inside this flexible frame; phones retain the
wider stacked crop, keeping her face, hands, and book visible.
There is no full-body requirement and no message dropdown or public message block.
Stored founder messages remain available in the existing admin editor; a separate
message section is not implemented as part of this approved revision.

Founder descriptions remain editable database content for the one saved founder.
The Rowena match uses her static portrait/motto; a differently named founder would
retain its own text without borrowing her identity. Empty/missing-biography states remain supported.

The founder section now reserves 100dvh minus the measured navbar height. Its
heading occupies an automatic grid row and the portrait/biography fills the rest.
Its horizontal padding matches the footer exactly: clamp(24px, 5vw, 96px). The
former 1152px centered width cap is removed, so the heading/divider and founder
composition use the footer's full available content width. Biography line length
remains capped for reading, and the bottom-aligned portrait retains its responsive
height. The navbar itself retains its established content width.
Top padding is halved to clamp(8px, 1.5dvh, 16px) per the latest user refinement.
Spacing and portrait width scale with viewport height; short desktop windows use
tighter paragraph leading and a smaller motto. The leadership anchor cancels the
global 16px breathing room, aligning the section directly below the navbar rather
than adding a second navbar offset.

2026-11-16 refinement: above 700px wide and 500px tall, the Founder composition
sticks below the measured navbar plus its existing top inset. The stage ends at
the viewport floor; a short clamp(160px, 30dvh, 320px) scroll runway holds it before
releasing naturally at Contact. Heading and biography rows use a bounded grid.
Long biographies scroll in a keyboard-focusable labeled region. The public page
receives one founder record. Portrait, colors, and content stay
unchanged. Phones and windows 500px tall or shorter use natural document flow,
allowing content to grow without clipping. No scroll interception or new animation
script is used. Reusable Tailwind rules remain in sections/founders.css.

## Contact footer

The approved compact footer follows Leadership outside main, retaining its
full-width navy surface, ivory text, gold accents, and responsive padding. Three
unequal desktop columns show university identity, Explore, and Contact
us. The university column is widest; its name is slightly smaller to leave room
for navigation. Explore and Contact headings share the same serif scale and top
alignment. The large motto and Our University navigation group are removed.

Social links and Follow us are removed. Explore lists Home, About, Academics, Founder,
and Contacts, linking to the existing page anchors.
Contact retains address, email, telephone, office hours, and existing empty states.
Copyright and Back to top sit below one full-width divider; the vertical contact
divider is removed. At 1000px and narrower, columns stack in identity,
Explore, Contact order. All links retain visible focus and at least 44px height.

Contact retains id=contact and mailto/tel actions. Unpublished fields remain labeled. There is no search
bar, new route, database column, or inquiry form. _UniversityContact.cshtml and
Styles/sections/contact.css own markup and Tailwind styles. The layout's optional
Footer section avoids duplicate footers; unavailable-content and other pages keep
the small fallback footer.

## Scroll-driven university gallery

The user's latest request supersedes the angled, automatically drifting treatment.
Gallery occupies exactly 200dvh, with a sticky gallery-stage filling the viewport
below the measured navbar. Its plane is flat, using translation only with no
perspective, rotation, pointer parallax, or hover lift. Alternating column offsets
follow clamped page-scroll progress; scrolling back reverses the effect, and idle
columns remain fixed. Scroll events coalesce into a single requested frame rather
than a self-scheduled animation loop. Resize rebuilds at the current progress.

Images still come from About gallery uploads, then the approved illustrations.
The heading keeps its localized top navy gradient and images retain their colors.
Selecting an image opens the native full-image dialog, with Close/Escape support.
Keyboard browsing reveals the accessible static grid; reduced motion and no-JS
retain the same static fallback. Styles live in sections/gallery.css and behavior
in drift-wall.js. All image sources and MVC/EF content paths remain established.

## The Hogwarts Collegiate Circle

This new section follows Gallery and uses the supplied curved-rope/pennant layout
reference. The term "Collegiate Circle" replaces "sister universities" in the UI.
Five freshly generated, transparent 793×1983 PNG banners share flat heraldic
linework, geometric starburst fields, pointed bottoms, blackletter names, double
borders, and print grain. Order: Gryffindor, Slytherin, Hogwarts, Ravenclaw,
Hufflepuff. Hogwarts is centered and slightly larger, with an original H monogram
on a quartered shield; the main university crest remains the approved original.

A gold curved rope and small pegs connect the banners. Gentle spring-based sway
reacts to the pointer and stops for reduced motion, offscreen content, or hidden
tabs. Below 760px the row scrolls horizontally and starts centered on Hogwarts;
native arrow-key scrolling and a short hint keep all five accessible. No external
links or claimed real-world partnerships are introduced. Asset provenance and
final built-in image-generation prompts are recorded in GENERATED_ASSETS.md.

Administration uses flat divided lists and explicit Edit/Delete actions. Editors use
Lato for operational headings, visible labels, native text/date/URL inputs, textarea
controls, a supported icon selector, inline errors, and POST delete confirmation.
University/contact sections allow editing one record with no UI deletes. Unavailable
images use a text fallback; the crest is not used as fake campus photography.

## Separate administration shell

The legacy /Admin route redirects to Hero and About. Each dedicated
content section selects _AdminLayout in its own Index view. The university navbar
is not rendered in this shell. A 272px navy sidebar
contains the approved crest and five links in this order: Hero and About,
Academics, Founder, Contacts, Account Management. The crest also links to Hero and About. The current
page uses a blue active background and aria-current=page, including edit/delete
and validation states. View university is a full-width outlined button-style link
with a small arrow, hover/focus feedback, and a 48px minimum height. It sits above
POST Sign out on desktop; the two controls share a row on phones. Edit personal
information is removed from this sidebar; the public account menu retains access.
The ivory workspace uses Lato headings and opens directly to the page content;
the user requested removal of the contextual topbar/account label. Sidebar links
are one flat list, with no numeric prefixes or University content divider label.
Account Management uses the same active style.
Below 901px, native sidebar disclosure collapses initially with JavaScript and
remains usable without JavaScript. The dashboard and content inventory are removed.
Account analytics belong above the directory in Account Management. Counts are
withheld when the database is unavailable; there are no invented analytics.

Hero and About combines the hero upload, About Heading, About Description, Mission,
Vision, and Core Values in one save form. Enter one value per textarea line; the
public chapter uses bullets. About imagery is fixed, and its image-management
disclosure has been removed. Contacts has University Address, Email,
Telephone, and paired native Time In/Time Out controls. Blank times retain the legacy office-hours fallback;
overnight hours are labeled as ending the next day. Native time controls use
minute precision without an explicit step attribute, which conflicts with the
bundled jQuery validator. Academics shows Subject Name and Subject Description,
with an always-visible image uploader. Founder opens its one existing record with
Founder Name, Founder Info, and Founder Message; no add/delete/list controls exist.
Existing birthdays are preserved. Empty storage shows a setup message rather than
an add form. The public page receives one founder rather than a collection.

Each content section retains its view, form model, and controller. Singleton
settings open directly to a save form. Legacy collection editors remain available
for old direct links; collections show
an add/edit form followed by published entries on the same page. Delete confirmation
uses that section's view; no shared conditional editor switches between sections.
Keep the established field, button, status, sidebar, and responsive form styling.
Image fields use native file pickers, format/size guidance, current-image previews,
and selected-file previews. Account management reuses these fields and flat divided
account rows with search and an inline edit form; no password fields are displayed.

### Shared editing workspace (2026-11-16)

2026-11-18 simplification: Hero and About, Academics, Founder, and Contacts start
directly with their labeled fields after the page heading. Remove the redundant
Page content/Add entry/Edit founder/Contact details subheads, introductory captions,
and visible group legends. Keep all fieldset legends screen-reader-only.
Account Management has no
Administration eyebrow, introductory caption, Account analytics heading, or totals
explanation. Keep its actual metrics and their labels.

Only Academics retains the two-column form and sticky draft preview; below 1101px
they stack. All other editors use the full available width without a side preview,
including legacy gallery/core-value editors. Related fields use responsive columns:
mission/vision, founder biography/message, contact email/telephone and office times.
Columns stack on phones. Upload previews stay beside their fields. The edit-status
line, office-hours explanatory hint, and account-list limit hint are removed.
The navy sidebar still collapses below 901px. Use the existing Lato,
navy, ivory and gold tokens, fine dividers, 6px input/button corners and a restrained
12px preview outline. Program previews show the portrait crop; image selection and
text fields update locally. Previews summarize stored fields rather than claiming
to reproduce every public layout. They do not autosave.

Saved collections use divided rows, thumbnails where relevant, short descriptions,
entry counts, explicit Edit, and quieter red Delete links to the existing confirmation
page. Account rows show their profile image or initials derived from the saved name.
Save actions stay visible within long forms; success messages remain on the returned
page. Inline errors keep Save usable and focus the first invalid field. Saving state
is shown only after native/unobtrusive validation permits submission.

Dirty tracking has no visible status line. Internal navigation/search/sign-out opens a native
HTML dialog with Keep editing / Discard changes, a focus trap and Escape support.
Refresh/close uses beforeunload. Reverting fields restores the clean state; failed
POSTs retain a dirty draft. No-JS forms retain server validation and ordinary saves.
Reusable behavior is in admin-editor.js, shared markup in _AdminContentPreview.cshtml,
and Tailwind styles in layouts/admin-editors.css. Dedicated controllers/form models/
views remain intact. No topbar or sidebar numeric labels were reintroduced.

### Account directory and analytics (2026-11-16)

Account Management analytics show Total Registered, Active Users, and Deactivated
Users from saved account records. Active means enabled to sign in, not currently
online. Use one divided metric strip above the directory, with no invented activity
history. Totals cover all accounts regardless of search or the 100-row directory
limit; omit the strip when account data is unavailable.

Account management uses flat horizontal rows with fine dividers: circular profile
photo or pale-blue initials, name above username, plain role text (bold for Admin),
and a colored status dot with a text label. Avoid repeating a username when no
saved name exists. The Your account badge sits beside the signed-in admin's name.
The only row actions are Activate/Deactivate and View information. Omit registration
timestamps. View information opens a dedicated read-only page with saved profile
fields; no email is invented because the account schema has none. On smaller
screens actions wrap below identity/status. Keep the navy/ivory palette and 44px
minimum controls. The current administrator has a quiet Your account label instead of a Deactivate action;
server-side own-account and last-active-Admin safeguards also apply.
Reusable Tailwind styling lives in layouts/admin-accounts.css.

2026-11-18 refinement: the account page fills the available workspace even when
no edit form is open. Rows share fixed role/status/action columns and a muted
column header; omit the rounded enclosing panel. Use quiet View information actions
with a small arrow and aligned activation controls; hover/focus adds a subtle ivory
tint. Actions form a consistent second row on narrower screens, with 44px minimum
targets. Content inventory and Dashboard are removed; account totals remain.

### Account navigation and personal information (2026-11-16)

Public navbar spacing is responsive within a 1440px container, using bold Lato
links with gold underline feedback and a 48px account toggle. Preserve transparent
hero styling, solid surfaces over other sections, and footer hiding. The ivory
account dropdown has a fine gold outline, modest radius, and compact identity
layout based on the supplied reference. Show the saved 52px round profile photo
(name initials when absent), full first/middle/last name with the Admin badge below it. Hide the redundant
username line for admins; students retain their username beneath the name. No email field exists, so do not invent an email address.
The dropdown is 260px wide with 16px padding. Below the identity divider, matching rows offer Edit personal information and
Manage website (Admin only). Sign out stays a separated POST action. Use internal
chevrons rather than external-link arrows. Existing outside-click/Escape closing remains.
The admin sidebar also provides the personal-information link.

Personal information uses the full available page width without a centered cap,
with clamp(24px, 5vw, 96px) horizontal padding and responsive field gaps. Four
columns share wide screens from 1200px; tablets use two and phones below 501px
use one. This supersedes the earlier 640px centered form. It retains
public branding, a quiet Back to university link and compact serif heading.
An identity strip shows the saved photo/full name/username, with Change photo
beside them on desktop and below them on phones. Selection immediately previews
in the same circular frame; it is persisted only on Save changes. Native upload
controls remain available without JavaScript. Name fields follow first/middle/last
order, then birthday; required/optional labels are explicit. A fine gold divider separates the compact
actions: draft state, navy Save changes and quiet Cancel. Use the navy/ivory/gold
palette, existing status messages/inline errors, and 44px controls.
profile-editor.js tracks changes locally, warns on navigation/sign-out/refresh,
and provides Keep editing / Discard changes with Escape support. Invalid forms
keep Save usable. Reverted fields restore the clean state; failed POSTs remain
dirty; cached-page restoration resets Save and recreates selected-photo previews.
Username is displayed only. Browser tabs show exactly Ravenclaw University in
all layouts. Shared page styles live in layouts/profile.css.

## Approved main logo (2026-11-11)

The user approved the cleaned transparent crest preview as the main university logo
and authorized website integration after reviewing it. Canonical asset:
`241611JalopFinalQuiz2/wwwroot/images/branding/ravenclaw-university-crest.png`.

The 1123 × 1401 RGBA PNG is an imagegen cleanup of the user-supplied Ravenclaw crest:
tightly cropped, transparent surrounding background, enhanced edge clarity, blue
shield and bird, helmet/mantling, and original gold RAVENCLAW ribbon preserved.
The website name beside it remains Ravenclaw University. Do not redraw, recolor,
stretch, or replace it without an explicit request. No opaque backdrop or shadow.

All layouts use this asset beside the university name with object-fit:contain.
Account layouts use 64 × 80px on desktop and 48 × 60px below 850px; the university
navbar uses 44 × 56px, or 36 × 46px below 481px; the admin sidebar uses 44 × 56px. It also
serves as the PNG favicon. Explicit dimensions reserve space. The linked wordmark
gets its accessible name from the adjacent text, so the image has empty alt text;
standalone instances must provide descriptive alternative text. On viewports
720px high or shorter, the account crest reduces to 48 × 60px, or 40 × 50px on phones.

## Account viewport fit (2026-11-11)

Removed the "Ravenclaw University · Student access" caption at the user's request.
Account layouts use balanced padding and a compact registration form; mobile shell
rows are auto/1fr so the brand header does not consume excess vertical space.
Short-height breakpoints reduce heading/spacing while retaining 44px touch targets.
No body overflow is hidden: validation messages, zoom, and an on-screen keyboard
may expand the document so content stays accessible.

Verified normal login/signup document dimensions match the viewport with no scroll
at 1280 × 720, 1280 × 600, 390 × 844, and 375 × 667. Crest image loaded at its
original 1123 × 1401 resolution, preserving alpha; its workspace copy matches the
approved preview byte-for-byte.
