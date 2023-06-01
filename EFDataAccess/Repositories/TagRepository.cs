using Domain.DataTransferClasses;
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

        public async Task<IList<BlogPost>> GetPostsByTagIdAsync(string tagId)
        {
            if (string.IsNullOrEmpty(tagId))
                return new List<BlogPost>();

            try
            {
                Tag? tag = await _context.Tags.Where(c => c.Id == tagId).Include(c => c.BlogPosts).ThenInclude(c => c.Author).FirstOrDefaultAsync();

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

        /// <summary>
        ///
        /// </summary>
        /// <returns>A list with data objects containing tagId, tag name and number of blog posts related to the tag.</returns>
        public async Task<List<TagBlogPostCountData>> GetTagsPostsCountDataList()
        {
            List<TagBlogPostCountData> tagBlogPostCountDatas = new();

            try
            {
                List<Tag> tags = await _context.Tags.Include(c => c.BlogPosts).ToListAsync();

                for (int i = 0; i < tags.Count; i++)
                {
                    Tag tag = tags[i];

                    tagBlogPostCountDatas.Add(new TagBlogPostCountData(tag.Id, tag.TagName, tag.BlogPosts.Count));
                }

                return tagBlogPostCountDatas;
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Tag", ex.Message);
            }

            return tagBlogPostCountDatas;
        }

        public override async Task<int> RemoveAsync(Tag entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Tags.Remove(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException or DbUpdateException or DbUpdateConcurrencyException)
                    _logger.LogCritical("DB Error", ex.InnerException);
            }

            return -1;
        }
    }
}