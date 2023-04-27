using EFDataAccess.Contexts;
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
    }
}