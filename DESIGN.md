# Ravenclaw University design

2026-11-18: Mobile About now uses vertical scroll-driven sticky chapters, image
above text, without a horizontal rail/dots. Mobile footer keeps Explore and Contact
side by side below the brand. Public/admin hamburger menus open navy drawers from
the left, with focus trapping, backdrop/Escape closing and background scroll lock.

2026-11-18: The fixed university quote sits beneath the footer brand. Founder
editing and public content now include name and biography only; founder birthday
and message/quote fields have been removed from the live database and models.

2026-11-18: Account information uses an unboxed identity/access column left and
personal fields right, stacking on smaller screens. Keep the Back to accounts
button beside the title. Directory actions use navy View information buttons,
soft red outlined Deactivate and soft green outlined Activate controls.

2026-11-18: Shared eye/eye-off controls in every web password field, retaining the
existing form style and keyboard focus rules. Profile Cancel restores persisted
values after validation failures and preserves other-tab drafts. Mobile About height
correction adjusts only window scroll, preserving the requested horizontal jump.

2026-11-18: Always use the approved compact navy/ivory ADMIN badge as the admin
indicator, especially beside the Profile name. Reuse _AdminBadge and .admin-badge
in Profile, account menu, account rows and details; never show it for students.

2026-11-18: My profile uses an unboxed identity/photo column left and Personal details /
Change password tabs right. Keep navy/ivory/gold, existing serif heading and Lato forms.
Two-column personal fields, separate password form, concise labels and no helper/draft
captions. Stack below 801px. See docs/DESIGN.md and docs/AUTHENTICATION.md.

2026-11-18: Admin sidebar View university is an outlined button-style link.
Removed its Edit personal information link; personal editing stays in the public account menu.

2026-11-18: All admin fieldset legends are visually hidden but accessible. Removed
edit-status lines, office-hours/account-list hints, and About image-management controls.
About text stays editable; its images use the fixed approved university illustrations.

2026-11-18: Removed the specified admin introductory captions and redundant editor
subheadings; first fieldset legends remain accessible but visually hidden. Account
rows now fill the available width, with flat dividers, aligned actions, plain roles,
labeled status dots, and Your account beside the name. Actual account totals remain.

The canonical design reference is [docs/DESIGN.md](docs/DESIGN.md).
Read it before changing UI; maintain its established tokens and account layouts.

Last refined: 2026-11-12. The canonical reference documents the unified public
section palette and shared typography roles.

Later refinements: title-only program cards with hover/keyboard reveal and mobile
tap toggles; flat, scroll-driven gallery across two viewport heights. See the
canonical reference for the current interaction rules.

2026-11-14: approved ivory founder portrait/text composition with a responsive
upper-body crop, database-backed biography, and no public message dropdown.

Founder sizing follows the viewport minus the measured navbar height, with
height-responsive spacing and a corrected anchor offset. Desktop now uses a
bounded sticky stage with a short scroll runway, keyboard-scrollable biographies,
and release before Contact. Phones and very short windows use natural flow.
See docs/DESIGN.md for the exact breakpoints and overflow behavior.

2026-11-16: Academics now uses the same bounded desktop sticky stage as Founder.
Heading and carousel share responsive gutters, with height-based vertical spacing
and natural-flow fallbacks on phones and very short screens. Existing card motion
and reveals remain intact; see the canonical reference for exact sizing.

2026-11-16: Dashboard followed by five admin sections; combined Hero and About editing with
dynamic heading/hero image and bulleted core values; paired office-time controls.
See docs/DESIGN.md for field grouping, compatibility fallbacks, and validation.

2026-11-16: Founder is a single existing record with edit-only controls; social
links are removed throughout the application. Subject uploads stay visible.
Account rows use aligned columns and navy View actions. The refined navbar menu
links to Personal information for the signed-in account. All browser tab titles
are Ravenclaw University. See the canonical reference for layout and behavior.

The account menu follows the compact photo/name reference: saved full name with
the Admin badge below the name (without the redundant admin username), then matching Edit personal information
and Admin-only Manage website rows. No invented email address or extra identity label.

Profile now uses a full-width editor with responsive 24–96px side padding, a full-name/photo identity strip,
Change photo with immediate preview, ordered name fields, a single phone column,
and Save changes / Cancel with draft-state and discard protection. See the
canonical reference for validation, fallback and keyboard behavior.

2026-11-18: removed the Profile width cap to use the available screen space.
Fields use four columns on wide desktops, two on tablets and one on phones.

2026-11-16: compact three-column footer: university/socials, Explore, Contact.
Removed the large motto and Our University links; columns stack on phones.

2026-11-18: Dashboard and content inventory removed. Administration opens Hero and
About directly; five editor/account links remain. Old /Admin links redirect there.

2026-11-18: Only Academics retains its side preview. Other admin editors use the full available width, related fields in responsive columns, inline image previews and draft status above Save. Unsaved-change protection remains.
