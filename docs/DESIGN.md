# Design direction

Authority: the user's supplied blueprint and later corrections. The official
institution and website name is Ravenclaw University; earlier naming is superseded.
Status: account layouts, the five-section university homepage, and protected content
editors are implemented. Real content and campus/program imagery still need entering.

## Identity

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
Font Awesome Free 6.7.2 loads social icons through cdnjs. Gold is primarily a decorative accent;
ensure readable text contrast rather than assuming every palette pairing works.

## Composition

Navigation -> Hero -> About -> Academics and Student Services -> Contact ->
Leadership and Administration -> Footer. The implemented version uses one
authenticated homepage with section anchors; separate detail pages remain optional.

- Full-width navy university navbar, navy admin sidebar and footer; alternate navy and ivory content sections.
- Large architectural hero image with readable overlay and left-aligned headline.
  Use Ravenclaw University consistently in the logo text, headings, navigation,
  page titles, login screen, and footer when those surfaces are implemented.
- Thin gold dividers, consistent image proportions, and subtle hover effects.
- Two-column program layouts and formal founder/profile compositions.
- On narrow screens, stack columns and provide accessible menu controls.
- Use visible field labels, keyboard focus, clear feedback, and reduced-motion support.

Use preselected images as requested. Do not generate imagery unless the user asks.
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
Desktop and mobile forms were visually inspected; later content sections must
reuse these tokens consistently.

## University content and editors

The homepage uses a split navy hero with the approved crest as a fallback visual,
ivory reading sections, thin dividers, two-column mission/vision and program layouts,
gallery images, contact details, and founder biographies/messages. Unpublished content
is labeled rather than invented. Razor encodes text and retains paragraph breaks.
Navigation wraps on smaller screens; page sections stack below 700px.

Administration uses flat divided lists and explicit Edit/Delete actions. Editors use
Lato for operational headings, visible labels, native text/date/URL inputs, textarea
controls, a supported icon selector, inline errors, and POST delete confirmation.
University/contact sections allow editing one record with no UI deletes. Unavailable
images use a text fallback; the crest is not used as fake campus photography.

## Separate administration shell

Admin and AdminContent views use _AdminLayout through folder-specific _ViewStart
files. The university navbar is not rendered in this shell. A 272px navy sidebar
contains the approved crest, Dashboard, and all seven content sections. The current
page uses a blue active background and aria-current=page, including edit/delete
and validation states. View university and POST Sign out sit below navigation.
The ivory workspace has a contextual topbar/account label and Lato headings.
Below 901px, native sidebar disclosure collapses initially with JavaScript and
remains usable without JavaScript. The dashboard provides a short publishing guide.

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
