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

        public DbSet<Author> Authors { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogPost>()
                .HasMany(c => c.Comments)
                .WithOne(c => c.BlogPost)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<BlogPost>()
                .Navigation(c => c.Author).AutoInclude();

            //builder.Entity<Comment>()
            //    .HasMany(c => c.Replies)
            //    .WithMany(c => c.)
            //     .UsingEntity(
            //    "CommentReply",
            //    l => l.HasOne(typeof(Comment)).WithMany().OnDelete(DeleteBehavior.NoAction),
            //    r => r.HasOne(typeof(Comment)).WithMany().OnDelete(DeleteBehavior.NoAction));

            builder.Entity<Comment>()
                .HasMany(c => c.Replies)
                .WithOne(c => c.ParentComment)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Comment>()
                .Navigation(c => c.Author).AutoInclude();
            //builder.Entity<Comment>()
            //    .HasMany<Comment>()
            //    .WithOne(c => c.ParentComment)
            //    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Tag>()
                .HasMany(c => c.BlogPosts)
                .WithMany(c => c.Tags)
                .UsingEntity(
                "BlogPostTag",
                l => l.HasOne(typeof(BlogPost)).WithMany().OnDelete(DeleteBehavior.NoAction),
                r => r.HasOne(typeof(Tag)).WithMany().OnDelete(DeleteBehavior.NoAction));
        }

        public void SeedTestData()
        {
            Random random = new Random();

            int numOfPostsToCreate = 20;
            int numOfAuthorToCreate = 20;
            int numOfTagsToCreate = 20;

            if (!BlogPosts.Any() | !Tags.Any())
            {
                List<Author> authors = new List<Author>();

                for (int i = 0; i < numOfAuthorToCreate; i++)
                {
                    Author author = new Author
                    {
                        Id = Guid.NewGuid().ToString(),
                        Username = Faker.Name.FullName(Faker.NameFormats.Standard)
                    };

                    authors.Add(author);
                }

                List<Tag> tags = new List<Tag>();

                for (int i = 0; i < numOfTagsToCreate; i++)
                {
                    Tag tag = new Tag
                    {
                        Id = Guid.NewGuid().ToString(),
                        TagDescription = string.Join(" ", Faker.Lorem.Words(10)),
                        Author = authors[random.Next(0, authors.Count-1)],
                        TagName = Faker.Name.Last()
                    };

                    tags.Add(tag);
                }

                for (int i = 0; i < numOfPostsToCreate; i++)
                {
                    BlogPost blogPost = new BlogPost()
                    {
                        Author = authors[random.Next(0, authors.Count-1)],
                        Id = Guid.NewGuid().ToString(),
                        Tags = tags.Take(random.Next(3, tags.Count -1)).ToList(),
                        Title = string.Join(" ", Faker.Lorem.Words(5)),
                        BodyContent = string.Join(" ", Faker.Lorem.Words(200)),
                        PostedOn = new DateTime(random.Next(2000, 2023), random.Next(1, 12), random.Next(1, 28))
                    };

                    int numParentComments = random.Next(1, 5);

                    for (int m = 0; m < numParentComments; m++)
                    {
                        Comment comment = new Comment();
                        comment.Id = Guid.NewGuid().ToString();
                        comment.Author = authors[random.Next(0, authors.Count - 1)];
                        comment.BodyContent = string.Join(" ", Faker.Lorem.Words(20));
                        comment.BlogPost = blogPost;
                        comment.ParentComment = null;

                        int branches = random.Next(1, 10);

                        CreateFakeComments(branches, 1, 5, blogPost, comment, authors);

                        blogPost.Comments.Add(comment);
                    }

                    BlogPosts.Add(blogPost);
                }

                SaveChanges();
            }
        }

        private void CreateFakeComments(int branch, int minComment, int maxComment, BlogPost blogPost, Comment parent, List<Author> authors)
        {
            if (branch <= 0)
                return;

            Random random = new Random();
            int numComments = random.Next(minComment, maxComment);

            for (int i = 0; i < numComments; i++)
            {
                Comment comment = new Comment();
                comment.Id = Guid.NewGuid().ToString();
                comment.Author = authors[random.Next(0, authors.Count-1)];
                comment.BodyContent = string.Join(" ", Faker.Lorem.Words(20));
                comment.BlogPost = null;
                comment.ParentComment = parent;
                parent.Replies.Add(comment);
                branch--;
                CreateFakeComments(branch, minComment, maxComment, blogPost, comment, authors);
            }
        }
    }
}