# Accepted decisions

Recorded 2026-11-11 from the project conversation.

| Decision | Reason / consequence |
| --- | --- |
| Keep ASP.NET Core MVC and SQL Server | User's chosen stack |
| Official institution name: Ravenclaw University | Explicit user correction; use consistently in branding and future work |
| Approved transparent Ravenclaw crest is the main university logo | User approved the imagegen-cleaned supplied crest and authorized website integration after preview |
| First content version is one authenticated page with section anchors | Implements the original five sections; separate detail pages can be added if requested |
| Admin area has a separate sidebar layout | Explicit user request; Dashboard and each content section have dedicated navigation with an active state |
| University navbar uses the established navy design | User rejected the literal rounded outlined sketch implementation. Use a full-width navy bar, ivory text, fine gold divider, constrained aligned inner content, and a circular account control |
| University and contact records are singletons in editor logic | Serializable creation prevents duplicates; existing rows are edited |
| Use Font Awesome Free 6.7.2 for optional social icons | Supported classes are allowlisted; platform labels remain readable without the CDN |
| Database name: Ravenclaw University Db | Explicit user naming request |
| Single university with independent content tables | User requested removing AcademyId relationships |
| Retain existing Academy-prefixed SQL identifiers | Public branding changes without altering the database schema |
| No extra indexes or unnecessary tables | Keep the first version simple |
| Remove AcademyName and AcademyHistory columns | Explicit user correction |
| Core values store titles only | User removed the description column |
| Optional SocialLinksTable.IconClass | Allows selection of frontend social icons |
| Store image paths or URLs | Preselected imagery; no image-byte storage needed |
| One UserTable with Admin/Student roles | Every visitor logs in; only admins edit |
| Student self-registration | Explicit user choice; server assigns Student and never exposes admin role selection |
| Add LastName, FirstName, MiddleName, Birthday to UserTable | User-requested account details; nullable storage preserves existing accounts |
| Require first/last name in new registration; optional middle name/birthday | Initial form policy; applied database fields still need an EF model refresh |
| Maintain one consolidated Schema.sql only | Explicit user request; include all applied changes and remove completed incremental scripts |
| Restrict Role with a CHECK constraint | Restores the previously agreed role constraint |
| Reject duplicate usernames in application logic for now | No unique constraint in the simplified schema |
| Database scripts belong in the MVC project's Database/ folder | Keep them outside wwwroot |
| Use relevant skills and maintain established design consistency | Explicit ongoing user preference |
| Commit each completed, verified unit of progress | Explicit ongoing user preference; no automatic push |

Update or supersede a decision when the user changes it. Do not reintroduce
earlier discarded requirements silently.
