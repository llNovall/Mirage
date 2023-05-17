using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Linq.Expressions;

namespace EFDataAccess.Repositories
{
    public class AuthorRepository : Repository<Author, AuthorRepository>, IAuthorRepository
    {
        public AuthorRepository(ApplicationDbContext context, ILogger<AuthorRepository> logger) : base(context, logger)
        {
        }

        public async Task<Author?> FindAsync(Expression<Func<Author, bool>> predicate)
        {
            try
            {
                return await _context.Authors.Where(predicate)
                    .Include(c => c.Posts)
                    .Include(c => c.Tags)
                    .Include(c => c.Posts)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Author", ex.Message);
            }

            return null;
        }

        public async Task<Author?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.Authors.Where(c => c.Id == id.ToString())
                    .Include(c => c.Posts)
                    .Include(c => c.Tags)
                    .Include(c => c.Posts).FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Author", ex.Message);
            }

            return null;
        }

        public async Task<Author?> FindByIdAsync(string id)
        {
            try
            {
                return await _context.Authors.Where(c => c.Id == id)
                    .Include(c => c.Posts)
                    .Include(c => c.Tags)
                    .Include(c => c.Posts)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Author", ex.Message);
            }

            return null;
        }

        public async Task<IList<Author>> GetAllAsync()
        {
            try
            {
                return await _context.Authors.Include(c => c.Posts)
                    .Include(c => c.Tags)
                    .Include(c => c.Comments)
                    .ToListAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error - Author", ex.Message);
            }

            return new List<Author>();
        }

        public override async Task<int> RemoveAsync(Author entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Authors.Remove(entity);
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