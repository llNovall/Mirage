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
            catch (DbException)
            {
            }

            return null;
        }

        public async Task<Author?> FindByIdAsync(Guid id)
            => await _context.Authors.Where(c => c.Id == id.ToString()).Include(c => c.Posts).Include(c => c.Tags).Include(c => c.Posts).FirstOrDefaultAsync();

        public async Task<Author?> FindByIdAsync(string id) => await _context.Authors.Where(c => c.Id == id).Include(c => c.Posts).Include(c => c.Tags).Include(c => c.Posts).FirstOrDefaultAsync();

        public async Task<IList<Author>> GetAllAsync() => await _context.Authors.Include(c => c.Posts).Include(c => c.Tags).Include(c => c.Comments).ToListAsync();
    }
}