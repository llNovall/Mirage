using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace EFDataAccess.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public void SeedTestData()
        {
            Random random = new Random();

            if (!Tags.Any())
            {
                for (int i = 0; i < 10; i++)
                {
                    string description = "Et ipsum sed et dolores in sadipscing ipsum lobortis no at sanctus imperdiet et et tempor justo dolore dolore eos voluptua doming gubergren enim tempor dolor in lorem zzril exerci minim dolor rebum erat magna elitr dolores nonumy sea rebum et kasd aliquip commodo sea consequat sed no dolore vero sadipscing sit diam in elitr suscipit duis feugiat sit eos ut diam gubergren lorem erat sed gubergren amet amet diam justo aliquyam facilisis duo sed ea consetetur kasd nisl sanctus nisl illum magna sed ea ipsum ea et in te ipsum consequat sadipscing qui facilisis elitr vero ipsum justo esse invidunt gubergren eu clita est clita laoreet invidunt et diam kasd luptatum at exerci dolor aliquam esse facilisis dolores delenit et assum kasd consectetuer elitr labore accusam consetetur et dolore nonumy et quis accusam et eum dolor no invidunt ipsum voluptua erat takimata clita gubergren diam kasd labore magna consetetur iusto ea dolor ut no consequat et dolor justo accusam elitr sed sed duo eirmod consectetuer diam lorem facilisis erat erat dolore vulputate et eirmod dolor adipiscing duo dolor nonumy sanctus invidunt dolor consetetur clita ipsum at est clita et amet est eos rebum sea ut kasd eirmod vero zzril";

                    int startIndex = random.Next(20, 100);
                    int endIndex = random.Next(startIndex + 5, startIndex + 100);
                    Tag tag = new Tag()
                    {
                        Id = Guid.NewGuid(),
                        TagName = $"Tag - {i}",
                        TagDescription = $"{description.Substring(startIndex, endIndex)}",
                        AuthorId = Guid.NewGuid(),
                    };

                    Tags.Add(tag);
                }

                SaveChanges();
            }
        }
    }
}