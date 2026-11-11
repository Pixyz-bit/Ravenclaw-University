# Accepted decisions

Recorded 2026-11-11 from the project conversation.

| Decision | Reason / consequence |
| --- | --- |
| Keep ASP.NET Core MVC and SQL Server | User's chosen stack |
| Database name: Ravenclaw University Db | Explicit user naming request |
| Single academy with independent content tables | User requested removing AcademyId relationships |
| No extra indexes or unnecessary tables | Keep the first version simple |
| Remove AcademyName and AcademyHistory columns | Explicit user correction |
| Core values store titles only | User removed the description column |
| Optional SocialLinksTable.IconClass | Allows selection of frontend social icons |
| Store image paths or URLs | Preselected imagery; no image-byte storage needed |
| One UserTable with Admin/Student roles | Every visitor logs in; only admins edit |
| Restrict Role with a CHECK constraint | Restores the previously agreed role constraint |
| Reject duplicate usernames in application logic for now | No unique constraint in the simplified schema |
| Database scripts belong in the MVC project's Database/ folder | Keep them outside wwwroot |
| Use relevant skills and maintain established design consistency | Explicit ongoing user preference |
| Commit each completed, verified unit of progress | Explicit ongoing user preference; no automatic push |

Update or supersede a decision when the user changes it. Do not reintroduce
earlier discarded requirements silently.
