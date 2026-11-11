using System.ComponentModel.DataAnnotations;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Services;

public static class FirstAdminCommand
{
    public static async Task<int> RunAsync(IServiceProvider services)
    {
        if (Console.IsInputRedirected)
        {
            Console.Error.WriteLine("Run --create-admin in an interactive terminal. Passwords are entered privately.");
            return 1;
        }
        Console.WriteLine("Create the first Ravenclaw University admin. This command refuses if an admin already exists.");
        Console.Write("Username: ");
        var username = Console.ReadLine() ?? "";
        Console.Write("First name: ");
        var firstName = Console.ReadLine() ?? "";
        Console.Write("Last name: ");
        var lastName = Console.ReadLine() ?? "";
        Console.Write("Password (8–128 characters): ");
        var password = ReadPassword();
        Console.Write("Confirm password: ");
        var confirmation = ReadPassword();
        var model = new RegisterViewModel
        {
            Username = username, FirstName = firstName, LastName = lastName,
            Password = password, ConfirmPassword = confirmation
        };
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(model, new ValidationContext(model), errors, true))
        {
            foreach (var error in errors) Console.Error.WriteLine(error.ErrorMessage);
            return 1;
        }
        try
        {
            await using var scope = services.CreateAsyncScope();
            var accounts = scope.ServiceProvider.GetRequiredService<AccountService>();
            var created = await accounts.CreateAsync(new UserTable
            {
                Username = username, FirstName = firstName.Trim(), LastName = lastName.Trim()
            }, password, firstAdmin: true);
            Console.WriteLine(created ? "Admin created. You can now sign in." : "Not created: an admin or this username already exists.");
            return created ? 0 : 1;
        }
        catch (Exception)
        {
            Console.Error.WriteLine("Admin creation failed. Check database access and try again.");
            return 1;
        }
    }

    private static string ReadPassword()
    {
        var password = new System.Text.StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace && password.Length > 0) password.Length--;
            else if (!char.IsControl(key.KeyChar)) password.Append(key.KeyChar);
        }
        Console.WriteLine();
        return password.ToString();
    }
}
