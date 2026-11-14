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
Font Awesome Free 6.7.2 loads social icons through cdnjs. Gold is primarily a decorative accent;
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
the existing navbar behavior. While any part of the contact footer is in view,
the homepage navbar slides out and becomes inert (unfocusable); it returns when
the footer leaves view. Back to top restores the transparent hero state. Reduced
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
The user selected only the revised Ravenclaw common-room image. Use it as a static background; no CTA buttons, image selectors, extra slides, or rotation. This supersedes the previous gallery. A ResizeObserver tracks navbar height so hero and navbar fit together. Heading size and spacing scale with width and height. CSS supplies navbar-height fallbacks without JavaScript. The hero may grow on short screens/zoom rather than clip text. Subsequent content retains the centered
1200px reading width and ivory sections, thin dividers, mission/vision and program layouts,
gallery images, contact details, and founder biographies/messages. Unpublished content
is labeled rather than invented. Razor encodes text and retains paragraph breaks.
Navigation wraps on smaller screens; page sections stack below 700px.

## About and Academics reference (2026-11-12)

About begins directly with “A place for curious minds.” The separate About label,
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
snap rail and dots retain one image above each text chapter. Uploaded About photos
take priority; approved illustrations
provide a visual fallback without inventing university facts. No-JS content remains
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
text. Its contextual link pairs a bold label with a bordered circular arrow; hover
and keyboard focus fill the arrow in blue. Typography and spacing scale with height
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
2026-11-12; their descriptions explicitly identify them as sample content.

Academics shares one viewport with the sticky navbar, using 100dvh minus its
measured height. A two-row grid reserves space for the heading and gives the
remaining height to the carousel. Padding, heading size, and gaps scale with
viewport height; cards use an 8:11 ratio up to 550px tall and stay at least 220px
wide. Captions show up to two title lines; opening a card displays the full title
and a scrollable description. The section anchor aligns directly below the navbar.
A 360px section minimum preserves readable content in very short/zoomed windows,
where normal vertical scrolling remains available instead of clipping controls.

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

Founder descriptions remain editable database content. Only Rowena's matching
record uses her static portrait/motto; other founder records keep their own text
without borrowing her identity. Empty/missing-biography states remain supported.

The founder section now reserves 100dvh minus the measured navbar height. Its
heading occupies an automatic grid row and the portrait/biography fills the rest.
Top padding is halved to clamp(8px, 1.5dvh, 16px) per the latest user refinement.
Spacing and portrait width scale with viewport height; short desktop windows use
tighter paragraph leading and a smaller motto. The leadership anchor cancels the
global 16px breathing room, aligning the section directly below the navbar rather
than adding a second navbar offset. Content remains in normal flow: long CMS text,
multiple founders, phones, and extreme zoom may grow the section instead of hiding
or clipping text. The approved desktop biography fits at 1280×600 and taller.

## Contact footer

The approved reference-inspired contact footer replaces both the standalone contact
section and the university page's small footer. It follows Leadership outside main,
with a full-width navy surface, ivory text, gold accents, and generous responsive
padding. The left roughly 70% contains the crest/name, the motto “Wit beyond measure
is man's greatest treasure,” and Explore / Our University navigation. The right
roughly 30% has Contact us, address, email, telephone, office hours, and published
social links, separated by a fine vertical rule. Copyright and Back to top sit
below a full-width horizontal divider. Below 901px columns stack with a horizontal
divider. All links have visible focus and at least 44px height.

Mission & Vision and Core Values jump to their About chapter with the existing
desktop scroll/mobile rail behavior; without JS they fall back to About. Contact
retains id=contact, mailto/tel actions, approved icons, HTTPS-only social URLs, and
accessible new-tab labels. Unpublished fields remain labeled. There is no search
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

Admin uses _AdminLayout through its folder-specific _ViewStart. Each dedicated
content section selects _AdminLayout in its own Index view. The university navbar
is not rendered in this shell. A 272px navy sidebar
contains the approved crest, Dashboard, and all seven content sections. The current
page uses a blue active background and aria-current=page, including edit/delete
and validation states. View university and POST Sign out sit below navigation.
The ivory workspace uses Lato headings and opens directly to the page content;
the user requested removal of the contextual topbar/account label. Sidebar links
are one flat list, with no numeric prefixes or University content divider label.
Account management follows the seven content links using the same active style.
Below 901px, native sidebar disclosure collapses initially with JavaScript and
remains usable without JavaScript. The dashboard provides a short publishing guide.

Each content section has its own view, form model, and controller. University and
contact settings open directly to a single save form. The other five sections show
an add/edit form followed by published entries on the same page. Delete confirmation
uses that section's view; no shared conditional editor switches between sections.
Keep the established field, button, status, sidebar, and responsive form styling.
Image fields use native file pickers, format/size guidance, current-image previews,
and selected-file previews. Account management reuses these fields and flat divided
account rows with search and an inline edit form; no password fields are displayed.

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
