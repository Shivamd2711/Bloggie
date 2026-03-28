using System;
using System.IO;
using Bloggie.Web.Data;
using Bloggie.Web.Models.EmailModels;
using Bloggie.Web.Repositories;
using Bloggie.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DATABASE configuration: respect DATABASE_URL (Supabase) or fall back to local SQL Server.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    // Parse DATABASE_URL (postgres://user:pass@host:port/dbname)
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

    var npgsql = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = userInfo.Length > 0 ? userInfo[0] : string.Empty,
        Password = userInfo.Length > 1 ? userInfo[1] : string.Empty,
        Database = uri.AbsolutePath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    };

    var connString = npgsql.ToString();

    builder.Services.AddDbContext<BloggieDbContext>(options =>
        options.UseNpgsql(connString));

    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(connString));
}
else
{
    // Local development (SQL Server)
    builder.Services.AddDbContext<BloggieDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("BloggieDbConnectionString")));

    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("BloggieAuthDbConnectionString")));
}

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Repositories / services
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IBlogPostLikeRepository, BlogPostLikeRepository>();
builder.Services.AddScoped<IBlogPostCommentRepository, BlogPostCommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.Configure<MailSettingModel>(
    builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IMailService, MailService>();

var app = builder.Build();

// Apply migrations and seed using the SQL file (only when DB has no posts)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var blogDb = services.GetRequiredService<BloggieDbContext>();
        var authDb = services.GetRequiredService<AuthDbContext>();

        // Apply migrations
        blogDb.Database.Migrate();
        authDb.Database.Migrate();

        // If no blog posts exist, run the SQL seed file (if present)
        if (!blogDb.BlogPosts.Any())
        {
            var seedFilePath = Path.Combine(AppContext.BaseDirectory, "seed", "dummydata.sql");
            if (File.Exists(seedFilePath))
            {
                var sql = File.ReadAllText(seedFilePath);
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    // Execute raw SQL against Postgres (works for multiple statements)
                    await blogDb.Database.ExecuteSqlRawAsync(sql);
                }
            }
        }
    }
    catch (Exception ex)
    {
        // For demo: write to console for visibility. In production log properly.
        Console.WriteLine($"Error applying migrations/seeding: {ex}");
        throw;
    }
}

// Configure pipeline (unchanged)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();