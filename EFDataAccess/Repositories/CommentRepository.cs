using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Linq.Expressions;

namespace EFDataAccess.Repositories
{
    public class CommentRepository : Repository<Comment, CommentRepository>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context, ILogger<CommentRepository> logger) : base(context, logger)
        {
        }

        public async Task<Comment?> FindAsync(Expression<Func<Comment, bool>> predicate)
        {
            try
            {
                return await _context.Comments.Where(predicate).Include(c => c.Replies).Include(c => c.BlogPost).FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Comment", ex.Message);
            }

            return null;
        }

        public async Task<Comment?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.Comments.Where(c => c.Id == id.ToString())
                    .Include(c => c.Replies)
                    .Include(c => c.BlogPost)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Comment", ex.Message);
            }

            return null;
        }

        public async Task<Comment?> FindByIdAsync(string id)
        {
            try
            {
                return await _context.Comments.Where(c => c.Id == id)
                    .Include(c => c.Replies)
                    .Include(c => c.BlogPost)
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Comment", ex.Message);
            }

            return null;
        }

        public async Task<IList<Comment>> GetAllAsync(Guid blogId)
        {
            try
            {
                return await _context.Comments.Where(c => c.BlogId == blogId.ToString())
                    .Include(c => c.Replies)
                    .Include(c => c.BlogPost)
                    .Include(c => c.Author)
                    .ToListAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Comment", ex.Message);
            }

            return new List<Comment>();
        }

        public async Task<IList<Comment>> GetAllAsync()
        {
            try
            {
                return await _context.Comments.Include(c => c.Replies)
                    .Include(c => c.BlogPost)
                    .Include(c => c.Author)
                    .ToListAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Comment", ex.Message);
            }

            return new List<Comment>();
        }

        public override async Task<int> RemoveAsync(Comment entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Comments.Remove(entity);
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