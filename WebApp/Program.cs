using Azure.Identity;
using Domain.Services;
using EFDataAccess.Contexts;
using EFDataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using System.Net;
using System.Reflection;
using WebApp.Authorization;
using WebApp.Services;
using WebApp.Utils.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(c =>
{
    c.AddDebug();
    c.AddConsole();
    c.AddAzureWebAppDiagnostics();
});

if (builder.Environment.IsEnvironment("LocalDev"))
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

if (builder.Environment.IsEnvironment("LocalDev"))
{
    string dbConnectionString = builder.Configuration["db-eminence-connectionstring-test"];

    builder.Services.AddDbContext<ApplicationDbContext>(
        options =>
        {
            options.UseSqlServer(connectionString: dbConnectionString);
        }
    );
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(
        options =>
        {
            options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING"));
        }
    );
}

if (builder.Environment.IsEnvironment("LocalDev"))
{
    string? tenantId = builder.Configuration["azure-storage:AZURE_TENANT_ID"];
    string? clientId = builder.Configuration["azure-storage:AZURE_CLIENT_ID"];
    string? clientSecret = builder.Configuration["azure-storage:AZURE_CLIENT_SECRET"];
    string? azureStorageUrl = builder.Configuration["azure-storage:STORAGE_URL"];

    if (!string.IsNullOrEmpty(tenantId)
        & !string.IsNullOrEmpty(clientId)
        & !string.IsNullOrEmpty(clientSecret)
        & !string.IsNullOrEmpty(azureStorageUrl))
    {
        ClientSecretCredential clientSecretCredential = new(
        tenantId: tenantId,
        clientId: clientId,
        clientSecret: clientSecret
        );

#pragma warning disable CS8604 // Possible null reference argument.
        Uri azureStorageUri = new(azureStorageUrl);
#pragma warning restore CS8604 // Possible null reference argument.

        builder.Services.AddAzureClients(c =>
            {
                c.AddBlobServiceClient(azureStorageUri);
                c.UseCredential(clientSecretCredential);
            }
        );
    }
}
else
{
    string? azureStorageBlogConnectionString = builder.Configuration["AZURE_STORAGEBLOB_CONNECTIONSTRING"];

    if (string.IsNullOrEmpty(azureStorageBlogConnectionString))
        throw new NullReferenceException("AzureStorageBlogConnection string is null");

    builder.Services.AddAzureClients(c =>
    {
        c.AddBlobServiceClient(connectionString: azureStorageBlogConnectionString);
    });
}

#region Identity

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
}
).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

#endregion Identity

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
    options.HttpsPort = 403;
});

builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddAreaPageRoute("Identity", "/Account/ForgotPassword", "resetpassword");
    options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "signup");
    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "login");
});

#region Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CommentEdit",
        policy => policy.RequireAuthenticatedUser()
        .AddRequirements(new SameAuthorRequirement())
        .RequireRole("member"));

    options.AddPolicy("CommentDelete",
        policy => policy.RequireAuthenticatedUser()
        .AddRequirements(new SameAuthorRequirement())
        .RequireRole("member", "admin"));

    options.AddPolicy("CommentCreate",
        policy => policy.RequireAuthenticatedUser().RequireRole("member", "blogger", "admin"));

    options.AddPolicy("CommentReply",
        policy => policy.RequireAuthenticatedUser().RequireRole("member", "blogger", "admin"));

    options.AddPolicy("BlogEdit",
        policy => policy.RequireAuthenticatedUser()
        .AddRequirements(new SameAuthorRequirement())
        .RequireRole("blogger"));

    options.AddPolicy("BlogDelete",
        policy => policy.RequireAuthenticatedUser()
        .AddRequirements(new SameAuthorRequirement())
        .RequireRole("blogger", "admin"));

    options.AddPolicy("BlogCreate",
        policy => policy.RequireAuthenticatedUser()
        .RequireRole("blogger"));
});

#endregion Authorization

#region Add Services

builder.Services.AddScoped<IDBService, DBService>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorIDHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorCommentHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorBlogPostHandler>();
builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

#endregion Add Services

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.Cookie.MaxAge = TimeSpan.FromDays(1);
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.LogoutPath = "/Identiy/Account/Logout";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.EnsureIdentityDbCreated();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.EnsureIdentityDbCreated();

    if (builder.Environment.IsEnvironment("LocalDev"))
        app.SeedTestDataToDatabase();
}

app.CreateRolesAsync("member", "blogger", "admin").Wait();

string? adminUserName = "";
string? adminPassword = "";
string? adminEmail = "";

if (builder.Environment.IsEnvironment("LocalDev"))
{
    adminUserName = builder.Configuration.GetSection("admin").GetRequiredSection("Username").Value;
    adminPassword = builder.Configuration.GetSection("admin").GetRequiredSection("Password").Value;
    adminEmail = builder.Configuration.GetSection("admin").GetRequiredSection("Email").Value;
}
else
{
    adminUserName = builder.Configuration.GetSection("adminUsername").Get<string>();
    adminPassword = builder.Configuration.GetSection("adminPassword").Get<string>();
    adminEmail = builder.Configuration.GetSection("adminEmail").Get<string>();
}

if (!string.IsNullOrEmpty(adminUserName) && !string.IsNullOrEmpty(adminPassword) && !string.IsNullOrEmpty(adminEmail))
    app.CreateAdminUserAsync(username: adminUserName, password: adminPassword, email: adminEmail).Wait();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();