using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Repositories
{
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<BlogPost?> FindAsync(Expression<Func<BlogPost, bool>> predicate) => await _context.BlogPosts.Where(predicate).FirstOrDefaultAsync();

        public async Task<BlogPost?> FindByIdAsync(Guid id) => await _context.BlogPosts.Where(c => c.Id == id.ToString()).FirstOrDefaultAsync();

        public async Task<BlogPost?> FindByIdAsync(string id) => await _context.BlogPosts.Where(c => c.Id == id).Include(c => c.Comments).Include(c => c.Tags).Include(c => c.Author).FirstOrDefaultAsync();

        public async Task<IList<BlogPost>> GetAllAsync() => await _context.BlogPosts.Include(c => c.Tags).Include(c => c.Comments).Include(c => c.Author).ToListAsync();
    }
}