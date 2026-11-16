using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Security;

public class UniversityCookieEvents(UniversityDbContext database, ILogger<UniversityCookieEvents> logger)
    : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        try
        {
            if (int.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            {
                var account = await database.UserTables.AsNoTracking()
                    .Where(user => user.UserId == id)
                    .Select(user => new { user.IsActive, user.Role, user.Username,
                        user.FirstName, user.MiddleName, user.LastName, user.ProfileImg })
                    .SingleOrDefaultAsync();
                if (account is { IsActive: true } && account.Role is "Admin" or "Student"
                    && account.Role == context.Principal?.FindFirstValue(ClaimTypes.Role)
                    && account.Username == context.Principal?.Identity?.Name)
                {
                    var name = string.Join(" ", new[] { account.FirstName, account.MiddleName, account.LastName }
                        .Where(part => !string.IsNullOrWhiteSpace(part)).Select(part => part!.Trim()));
                    context.HttpContext.Items[typeof(AccountMenuViewModel)] = new AccountMenuViewModel(
                        string.IsNullOrWhiteSpace(name) ? account.Username : name, account.Username, account.ProfileImg);
                    return;
                }
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to validate an account session.");
        }

        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
