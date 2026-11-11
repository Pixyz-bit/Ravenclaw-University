# Ravenclaw project instructions

The official institution and website name is Ravenclaw University. Use that name
in UI, content, documentation, and future work. Existing SQL identifiers such as
AcademyInfoTable are retained for compatibility; they are not the public brand.

Before working, read docs/README.md and docs/STATUS.md, then the documents
relevant to the request. Read skills/ravenclaw-project/SKILL.md for the project
workflow. This applies to the entire repository.

- Use all skills necessary for the actual task. Read their instructions; do not
  invoke unrelated skills just because they are installed.
- Follow docs/DESIGN.md and the existing interface when implementing or refining
  UI. Maintain consistent typography, colors, spacing, components, and behavior.
  Explicit user changes take priority; update the documentation to reflect them.
- Keep changes simple and within the requested scope. Do not treat planned
  functionality as implemented or run database scripts merely to document them.
- After each completed, meaningfully verified unit of progress, update affected
  documentation and commit that progress to Git with a descriptive message.
  Inspect the staged diff first. Do not commit secrets, generated output, or
  unrelated changes. Do not amend/rewrite history or push unless requested.
- If verification or committing is blocked, report the exact limitation without
  claiming success. Record unfinished implementation work in docs/STATUS.md.

Database definitions live in 241611JalopFinalQuiz2/Database/Schema.sql; database
rules are explained in its adjacent README.md. Keep these as the schema source
of truth rather than duplicating column lists in other documentation.
Keep only one consolidated Schema.sql covering all applied changes; do not retain
incremental schema scripts. Verify differences before changing an existing database.
