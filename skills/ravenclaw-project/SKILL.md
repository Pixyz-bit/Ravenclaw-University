---
name: ravenclaw-project
description: Maintain Ravenclaw Academy project context, established design, and Git progress when implementing or documenting this repository. Use for work in this project, not unrelated applications.
---

# Ravenclaw project workflow

Locate the repository root containing AGENTS.md and docs/README.md. Read the
documentation index and current status, then the requirements, architecture,
design, or database references relevant to the task.

- Use every available skill necessary for the requested work. Read its SKILL.md
  before applying it; do not invoke unrelated skills or expand the task to fit a skill.
- Follow docs/DESIGN.md and established UI components. Maintain typography,
  colors, spacing, imagery, and interaction consistency across pages. Record
  explicit user design changes so later work follows the new direction.
- Preserve the simplified single-academy schema unless the user requests changes.
  Exact SQL and database rules live in the application's Database/ folder.
- Distinguish proposed behavior from implemented and verified behavior. Document
  decisions and update docs/STATUS.md as work progresses.
- Commit each completed, meaningfully verified unit of progress to Git. Review
  status and staged diffs; stage only task-related changes. Exclude secrets,
  build output, and unrelated user work. Use descriptive commit messages.
- Do not rewrite history or push without a request. If a commit or verification
  is blocked, explain the limitation and retain the completed local work.

The latest explicit user instructions override older documented decisions.
This skill does not authorize unrelated external changes or live database resets.
