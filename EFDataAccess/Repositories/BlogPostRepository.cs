using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Repositories
{
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IList<BlogPost>> FindAllBlogPostsIncludingNavigationAsync()
            => await _context.BlogPosts.Include(c => c.Tags).Include(c => c.Comments).ToListAsync();

        public async Task<BlogPost?> FindByIdIncludeNavigationAsync(Guid id)
        {
            BlogPost? blogPost = await _context.BlogPosts.Include(c => c.Tags).Include(c => c.Comments).FirstOrDefaultAsync(c => c.Id == id);

            return blogPost;
        }

        public async Task<BlogPost?> FindByIdIncludeNavigationAsync(string id)
        {
            Guid parsedId = Guid.Parse(id);

            BlogPost? blogPost = await _context.BlogPosts.Include(c => c.Tags).Include(c => c.Comments).FirstOrDefaultAsync(c => c.Id == parsedId);

            return blogPost;
        }
    }
}