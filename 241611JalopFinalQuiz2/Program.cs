using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("UniversityDb")
    ));

var app = builder.Build();

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

        Console.WriteLine("Database read check passed: AcademyInfoTable queried successfully.");
        Console.WriteLine(universityInfo is null
            ? "The table is empty; the database connection still works."
            : "A university information record exists. Its contents were not displayed.");
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
