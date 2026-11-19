using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.Security;
using _241611JalopFinalQuiz2.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("UniversityDb")
    ));
builder.Services.AddScoped<IPasswordHasher<UserTable>, PasswordHasher<UserTable>>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<UniversityContentService>();
builder.Services.AddSingleton<ImageUploadService>();
builder.Services.AddScoped<UniversityCookieEvents>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "RavenclawUniversity.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = false;
        options.EventsType = typeof(UniversityCookieEvents);
    });
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("account-forms", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 30, Window = TimeSpan.FromMinutes(1), QueueLimit = 0
        }));
    options.OnRejected = async (context, cancellationToken) =>
        await context.HttpContext.Response.WriteAsync(
            "Too many account requests. Wait a minute, then try again.", cancellationToken);
});

var app = builder.Build();

if (args.Contains("--create-admin", StringComparer.OrdinalIgnoreCase))
{
    Environment.ExitCode = await FirstAdminCommand.RunAsync(app.Services);
    await app.DisposeAsync();
    return;
}

// Explicit local diagnostic: query through the registered context, then exit.
// This does not start the website or change database records.
if (args.Contains("--check-database", StringComparer.OrdinalIgnoreCase))
{
    try
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
        var universityInfo = await context.AcademyInfoTables
            .AsNoTracking()
            .OrderBy(info => info.AcademyInfoId)
            .FirstOrDefaultAsync();

        Console.WriteLine("AcademyInfoTable read passed.");
        Console.WriteLine(universityInfo is null
            ? "The table is empty; the database connection still works."
            : "A university information record exists. Its contents were not displayed.");

        // Verify the profile columns without selecting passwords or displaying data.
        await context.UserTables
            .AsNoTracking()
            .OrderBy(user => user.UserId)
            .Select(user => new
            {
                user.LastName,
                user.FirstName,
                user.MiddleName,
                user.Birthday
            })
            .Take(1)
            .ToListAsync();

        Console.WriteLine("UserTable personal-detail columns read passed. No account data was displayed.");
        await context.ContactsTables
            .AsNoTracking()
            .OrderBy(contact => contact.ContactId)
            .Select(contact => new { contact.TimeIn, contact.TimeOut })
            .Take(1)
            .ToListAsync();
        Console.WriteLine("ContactsTable office-time columns read passed. No contact data was displayed.");
        Console.WriteLine("Database read check passed.");
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"Database read check failed: {exception.Message}");
        Environment.ExitCode = 1;
    }

    await app.DisposeAsync();
    return;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePages(async status =>
{
    var context = status.HttpContext;
    if (context.Response.StatusCode != StatusCodes.Status404NotFound) return;

    var page = new ViewResult
    {
        ViewName = "~/Views/Shared/NotFound.cshtml",
        StatusCode = StatusCodes.Status404NotFound,
        ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
    };
    await page.ExecuteResultAsync(new ActionContext(context, new RouteData(), new ActionDescriptor()));
});
var imageUploads = app.Services.GetRequiredService<ImageUploadService>();
Directory.CreateDirectory(imageUploads.RootPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(imageUploads.RootPath),
    RequestPath = "/images/uploads",
    OnPrepareResponse = context => context.Context.Response.Headers["X-Content-Type-Options"] = "nosniff"
});
app.UseRouting();

// An unknown URL has no endpoint to authorize.
app.Use((context, next) => context.GetEndpoint() is null
    ? Results.NotFound().ExecuteAsync(context)
    : next(context));

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
