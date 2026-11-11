# Requirements

Build a dynamic ASP.NET Core MVC website for the fictional Ravenclaw University.
The SQL Server database is named Ravenclaw University Db. Use Ravenclaw University
for all public branding. Existing Academy-prefixed SQL identifiers are retained
for compatibility with the documented schema.

## Access

- All visitors must log in before viewing website content.
- Active students may view content only.
- Active admins may view, add, edit, and delete applicable website content.
- Inactive users must be denied access. Enforce roles on the server.
- Login must be reachable without authentication. Registration and account
  provisioning behavior have not been specified.
- Store framework-generated password hashes, not plaintext passwords.

## Content

- Hero: preselected background image.
- About: university description, mission, vision, independently managed core value
  titles, and university photographs.
- Academics: subjects/programs with names, descriptions, and images.
- Founder: name, optional birthday, description, and founder message.
- Contact: address, email, telephone, office hours, and editable social links.
- Social links may specify an icon class; the matching library must be available.
- User accounts include username, role, optional profile image, and active status.

AcademyName, AcademyHistory, and core value descriptions were deliberately
removed from the database. The static Ravenclaw University name appears in branding.
Do not invent factual founder dates or credentials. Earlier program examples,
history, statistics, and imagery are planning material, not verified content.

## Scope boundaries

No enrollment records, payments, student grades, or contact inquiry storage were
requested in the final simplified schema. Earlier inquiry-form ideas are optional
future work, not a committed database feature. Do not add tables implicitly.
