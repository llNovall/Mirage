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
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Tag?> FindAsync(Expression<Func<Tag, bool>> predicate) => await _context.Tags.Where(predicate).Include(c => c.BlogPosts).Include(c => c.Author).FirstOrDefaultAsync();

        public async Task<Tag?> FindByIdAsync(Guid id) => await _context.Tags.Where(c => c.Id == id.ToString()).Include(c => c.BlogPosts).Include(c => c.Author).FirstOrDefaultAsync();

        public async Task<Tag?> FindByIdAsync(string id) => await _context.Tags.Where(c => c.Id == id).Include(c => c.BlogPosts).Include(c => c.Author).FirstOrDefaultAsync();

        public async Task<IList<Tag>> GetAllAsync()
            => await _context.Tags.Include(c => c.BlogPosts).Include(c => c.Author).ToListAsync();

        public async Task<IList<BlogPost>> GetPostsByTagAsync(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                return new List<BlogPost>();

            Tag? tag = await _context.Tags.Where(c => c.TagName == tagName).Include(c => c.BlogPosts).FirstOrDefaultAsync();

            if (tag == null)
                return new List<BlogPost>();

            return tag.BlogPosts;
        }

        public async Task<int> GetTotalPostsByTagAsync(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                return 0;

            Tag? tag = await _context.Tags.Where(c => c.TagName == tagName).Include(c => c.BlogPosts).FirstOrDefaultAsync();

            if (tag == null)
                return 0;

            return tag.BlogPosts.Count;
        }
    }
}