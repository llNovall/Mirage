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

            if (!BlogPosts.Any() | !Tags.Any())
            {
                List<Tag> tags = new List<Tag>();

                for (int i = 0; i < 10; i++)
                {
                    Tag tag = new Tag
                    {
                        Id = Guid.NewGuid(),
                        TagDescription = string.Join(" ", Faker.Lorem.Words(10)),
                        AuthorId = Guid.NewGuid(),
                        TagName = Faker.Name.Last()
                    };

                    tags.Add(tag);
                }

                for (int i = 0; i < 10; i++)
                {
                    BlogPost blogPost = new BlogPost()
                    {
                        AuthorId = Guid.NewGuid(),
                        Id = Guid.NewGuid(),
                        Tags = tags.Take(random.Next(3, 10)).ToList(),
                        Title = string.Join(" ", Faker.Lorem.Words(5)),
                        BodyContent = string.Join(" ", Faker.Lorem.Words(200)),
                    };

                    int numParentComments = random.Next(1, 5);

                    for (int m = 0; m < numParentComments; m++)
                    {
                        Comment comment = new Comment();
                        comment.Id = Guid.NewGuid();
                        comment.AuthorId = Guid.NewGuid();
                        comment.BodyContent = string.Join(" ", Faker.Lorem.Words(20));
                        comment.BlogPost = blogPost;

                        int branches = random.Next(1, 3);

                        CreateFakeComments(5, 1, 5, blogPost, comment);

                        blogPost.Comments.Add(comment);
                    }

                    BlogPosts.Add(blogPost);
                }

                SaveChanges();
            }
        }

        private void CreateFakeComments(int branch, int minComment, int maxComment, BlogPost blogPost, Comment parent)
        {
            if (branch <= 0)
                return;

            Random random = new Random();
            int numComments = random.Next(minComment, maxComment);

            for (int i = 0; i < numComments; i++)
            {
                Comment comment = new Comment();
                comment.Id = Guid.NewGuid();
                comment.AuthorId = Guid.NewGuid();
                comment.BodyContent = string.Join(" ", Faker.Lorem.Words(20));
                comment.BlogPost = blogPost;

                parent.Replies.Add(comment);
                branch--;
                CreateFakeComments(branch, minComment, maxComment, blogPost, comment);
            }
        }
    }
}