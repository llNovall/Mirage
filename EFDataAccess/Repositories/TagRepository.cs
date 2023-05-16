using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Linq.Expressions;

namespace EFDataAccess.Repositories
{
    public class TagRepository : Repository<Tag, TagRepository>, ITagRepository
    {
        public TagRepository(ApplicationDbContext context, ILogger<TagRepository> logger) : base(context, logger)
        {
        }

        public async Task<Tag?> FindAsync(Expression<Func<Tag, bool>> predicate)
        {
            try
            {
                return await _context.Tags.Where(predicate)
                    .Include(c => c.BlogPosts)
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Tag", ex.Message);
            }

            return null;
        }

        public async Task<Tag?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.Tags.Where(c => c.Id == id.ToString())
                    .Include(c => c.BlogPosts)
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Tag", ex.Message);
            }

            return null;
        }

        public async Task<Tag?> FindByIdAsync(string id)
        {
            try
            {
                return await _context.Tags.Where(c => c.Id == id)
                    .Include(c => c.BlogPosts)
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Tag", ex.Message);
            }

            return null;
        }

        public async Task<IList<Tag>> GetAllAsync()
        {
            try
            {
                return await _context.Tags.Include(c => c.BlogPosts)
                    .Include(c => c.Author)
                    .ToListAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Tag", ex.Message);
            }

            return new List<Tag>();
        }

        public async Task<IList<BlogPost>> GetPostsByTagAsync(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                return new List<BlogPost>();

            try
            {
                Tag? tag = await _context.Tags.Where(c => c.TagName == tagName).Include(c => c.BlogPosts).FirstOrDefaultAsync();

                return tag?.BlogPosts ?? new List<BlogPost>();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Tag", ex.Message);
            }

            return new List<BlogPost>();
        }

        public async Task<int> GetTotalPostsByTagAsync(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                return 0;

            try
            {
                Tag? tag = await _context.Tags.Where(c => c.TagName == tagName)
                    .Include(c => c.BlogPosts)
                    .FirstOrDefaultAsync();

                return tag?.BlogPosts?.Count ?? 0;
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Tag", ex.Message);
            }

            return 0;
        }
    }
}