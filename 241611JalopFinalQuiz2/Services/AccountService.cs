using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.Services;

public class AccountService(UniversityDbContext database, IPasswordHasher<UserTable> hasher)
{
    // Explicit collation makes login names case-insensitive even on a CS database.
    private const string UsernameCollation = "Latin1_General_100_CI_AS";

    public async Task<UserTable?> AuthenticateAsync(string username, string password)
    {
        var matches = await database.UserTables
            .Where(user => EF.Functions.Collate(user.Username, UsernameCollation) == username.Trim())
            .Take(2).ToListAsync();
        // Fail closed if legacy data contains duplicate usernames.
        if (matches.Count != 1) return null;
        var user = matches[0];
        if (!user.IsActive || user.Role is not ("Student" or "Admin")) return null;

        PasswordVerificationResult result;
        try { result = hasher.VerifyHashedPassword(user, user.PasswordHash, password); }
        catch (FormatException) { return null; }
        if (result == PasswordVerificationResult.Failed) return null;
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = hasher.HashPassword(user, password);
            await database.SaveChangesAsync();
        }
        return user;
    }

    public async Task<bool> CreateAsync(UserTable user, string password, bool firstAdmin = false)
    {
        user.Username = user.Username.Trim();
        // Callers cannot use this operation to create additional admins.
        user.Role = firstAdmin ? "Admin" : "Student";
        user.IsActive = true;
        user.PasswordHash = hasher.HashPassword(user, password);

        // The agreed schema has no unique index: serialize duplicate checks and inserts.
        await using var transaction = await database.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if (firstAdmin && await database.UserTables.AnyAsync(existing => existing.Role == "Admin"))
            return false;
        if (await database.UserTables.AnyAsync(existing =>
            EF.Functions.Collate(existing.Username, UsernameCollation) == user.Username))
            return false;

        database.UserTables.Add(user);
        await database.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
}
