using Domain.Entities;
using Domain.Services;
using EFDataAccess.Contexts;
using EFDataAccess.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Utils.Extentions
{
    public static class IApplicationBuilderExtentions
    {
        /// <summary>
        /// Creates DbContext if required
        /// </summary>
        /// <param name="builder"></param>
        public static void EnsureIdentityDbCreated(this IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Database.Migrate();
            }
        }

        /// <summary>
        /// Seeds database with test data. Use this only for development.
        /// </summary>
        /// <param name="builder"></param>
        public static void SeedTestDataToDatabase(this IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.SeedTestData();
            }
        }

        /// <summary>
        /// Seeds database with test data. Use this only for development.
        /// </summary>
        /// <param name="builder"></param>
        public static void SeedDataToDatabase(this IApplicationBuilder builder, string adminUsername)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (dbContext == null)
                    throw new NullReferenceException(nameof(dbContext));

                UserManager<IdentityUser>? userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                if (userManager == null)
                    throw new NullReferenceException(nameof(userManager));

                IdentityUser? identityUser = userManager.FindByNameAsync(adminUsername).Result;

                if (identityUser == null)
                    throw new NullReferenceException(nameof(identityUser));

                Author? author = dbContext.Authors.Where(c => c.Id == identityUser.Id).FirstOrDefault();

                if (author == null)
                {
                    author = new Author
                    {
                        Id = identityUser.Id,
                        Username = identityUser.UserName
                    };
                }

                dbContext.Authors.Add(author);

                dbContext.SeedTagData(author);
            }
        }

        public static async Task CreateRolesAsync(this IApplicationBuilder builder, params string[] roles)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                RoleManager<IdentityRole>? roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (roleManager == null)
                    return;

                for (int i = 0; i < roles.Length; i++)
                {
                    if (await roleManager.RoleExistsAsync(roles[i]))
                        continue;

                    IdentityRole role = new(roles[i]);
                    var result = await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task CreateAdminUserAsync(this IApplicationBuilder builder, string username, string password, string email)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                UserManager<IdentityUser>? userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                if (userManager == null)
                    return;

                IDBService? dBService = serviceScope.ServiceProvider.GetService<IDBService>();

                if (dBService == null)
                    return;

                IdentityUser? admin = await userManager.FindByNameAsync(username);

                if (admin != null)
                {
                    Author? adminAuthor = await dBService.AuthorRepository.FindByIdAsync(Guid.Parse(admin.Id));
                    if (adminAuthor != null)
                        return;
                }

                admin = new IdentityUser(username);

                await userManager.SetEmailAsync(admin, email);
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "admin");

                Author author = new()
                {
                    Id = admin.Id,
                    Username = username
                };

                await dBService.AuthorRepository.AddAsync(author);
            }
        }
    }
}