# Ravenclaw University design

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
