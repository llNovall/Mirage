using Azure.Identity;
using Domain.Services;
using EFDataAccess.Contexts;
using EFDataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using System.Net;
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

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(
        options =>
        {
            options.UseSqlServer(connectionString: builder.Configuration["db-eminence-connectionstring-test"]);
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

if (builder.Environment.IsDevelopment())
{
    ClientSecretCredential clientSecretCredential = new(tenantId: builder.Configuration["azure-storage:AZURE_TENANT_ID"],
    clientId: builder.Configuration["azure-storage:AZURE_CLIENT_ID"],
    clientSecret: builder.Configuration["azure-storage:AZURE_CLIENT_SECRET"]);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string azureStorageUrl = builder.Configuration["azure-storage:STORAGE_URL"];
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    builder.Services.AddAzureClients(c =>
        {
#pragma warning disable CS8604 // Possible null reference argument.
            c.AddBlobServiceClient(new Uri(azureStorageUrl));
#pragma warning restore CS8604 // Possible null reference argument.

            c.UseCredential(clientSecretCredential);
        }
    );
}
else
{
    builder.Services.AddAzureClients(c =>
    {
        c.AddBlobServiceClient(connectionString: builder.Configuration.GetConnectionString("AZURE_STORAGEBLOB_CONNECTIONSTRING"));
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

builder.Services.AddScoped<IDBService, DBService>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorIDHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorCommentHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorBlogPostHandler>();
builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

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
if (!app.Environment.IsDevelopment())
{
    app.EnsureIdentityDbCreated();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.EnsureIdentityDbCreated();
    app.SeedTestDataToDatabase();
}

app.CreateRolesAsync("member", "blogger", "admin").Wait();

#pragma warning disable CS8604 // Possible null reference argument.
app.CreateAdminUserAsync(builder.Configuration["admin:Username"],
    builder.Configuration["admin:Password"], builder.Configuration["admin:Email"]).Wait();
#pragma warning restore CS8604 // Possible null reference argument.

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