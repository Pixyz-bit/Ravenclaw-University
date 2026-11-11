# Design direction

Authority: the user's supplied blueprint and later corrections. The official
institution and website name is Ravenclaw University; earlier naming is superseded.
Status: account layouts and a minimal protected portal/admin shell are implemented;
university content pages and content editors are still planned.

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
fallbacks. Images and social icon libraries are not installed. Gold is primarily a decorative accent;
ensure readable text contrast rather than assuming every palette pairing works.

## Composition

Navigation -> Hero -> About -> Academics and Student Services -> Contact ->
Leadership and Administration -> Footer. A separate About page was discussed;
the final page/route split remains undecided.

- Navy navigation and footer; alternate navy and ivory content sections.
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

Shared account layout uses a navy editorial panel with an RU typographic monogram,
motto, and fine architectural arch; ivory form panel uses visible labels and a
navy primary action. Registration uses two columns on desktop and one on mobile.
Below 850px the story panel reduces to the wordmark; below 480px all fields stack.
Inputs/buttons have 48px minimum height, errors are inline and textual, and there
is a keyboard skip link. No images or animations were generated. Shared authenticated
navigation includes University, Admin-only Administration, and POST Sign out.
Desktop and mobile forms were visually inspected; later content sections must
reuse these tokens without treating the starter portal as finished content.
