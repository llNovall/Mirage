using Domain.Services;
using EFDataAccess.Contexts;
using EFDataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Net;
using WebApp.Utils.Extentions;
using WebApp.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseSqlServer(connectionString: builder.Configuration["DbMirage"]);
    }
);

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Stores.ProtectPersonalData = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}
).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
    options.HttpsPort = 403;
});

builder.Services.AddMvc();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CommentEdit",
        policy => policy.RequireAuthenticatedUser().AddRequirements(
            new SameAuthorRequirement()));

    options.AddPolicy("CommentDelete",
        policy => policy.RequireAuthenticatedUser().AddRequirements(
            new SameAuthorRequirement()));

    options.AddPolicy("CommentCreate", policy => policy.RequireAuthenticatedUser());

    options.AddPolicy("CommentReply", policy => policy.RequireAuthenticatedUser());

    options.AddPolicy("BlogEdit",
        policy => policy.RequireAuthenticatedUser().AddRequirements(
            new SameAuthorRequirement()));

    options.AddPolicy("BlogDelete",
        policy => policy.RequireAuthenticatedUser().AddRequirements(
            new SameAuthorRequirement()));

    options.AddPolicy("BlogCreate", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddScoped<IDBService, DBService>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorIDHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorCommentHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SameAuthorBlogPostHandler>();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.LogoutPath = "/Identiy/Account/Logout";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.EnsureIdentityDbCreated();
    app.SeedTestDataToDatabase();
}

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