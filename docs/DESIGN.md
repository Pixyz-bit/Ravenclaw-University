# Design direction

Authority: the user's supplied Ravenclaw Academy blueprint and later corrections.
Status: planning reference; a custom interface has not yet been implemented.

## Identity

Use a modern dark-academia institutional style with historic architectural cues,
clear academic content, generous spacing, and restrained ornamentation.

| Token | Value |
| --- | --- |
| Midnight navy | #101D34 |
| Royal blue | #244468 |
| Antique gold | #C3A568 |
| Ivory | #F4F0E6 |
| Charcoal | #343B45 |
| Heading font direction | Cormorant Garamond or Cinzel; select one consistently |
| Body font direction | Lato |
| Content width | Approximately 1200px |
| Motto | Where Wisdom Takes Flight. |

Fonts and image/icon libraries are proposed, not installed dependencies. Check
availability before implementation. Gold is primarily a decorative accent;
ensure readable text contrast rather than assuming every palette pairing works.

## Composition

Navigation -> Hero -> About -> Academics and Student Services -> Contact ->
Leadership and Administration -> Footer. A separate About page was discussed;
the final page/route split remains undecided.

- Navy navigation and footer; alternate navy and ivory content sections.
- Large architectural hero image with readable overlay and left-aligned headline.
- Thin gold dividers, consistent image proportions, and subtle hover effects.
- Two-column program layouts and formal founder/profile compositions.
- On narrow screens, stack columns and provide accessible menu controls.
- Use visible field labels, keyboard focus, clear feedback, and reduced-motion support.

Use preselected images as requested. Do not generate imagery unless the user asks.
Preserve established tokens and reusable components across student and admin
screens; admin screens should prioritize clear editing tasks. Later explicit user
choices override this planning reference and should be recorded here.
