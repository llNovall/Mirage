using EFDataAccess.Contexts;
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
                IdentityUser? admin = await userManager.FindByNameAsync(username);

                if (admin == null)
                    return;

                admin = new IdentityUser(username);

                await userManager.SetEmailAsync(admin, email);
                await userManager.AddToRoleAsync(admin, "admin");
                await userManager.CreateAsync(admin, password);
            }
        }
    }
}