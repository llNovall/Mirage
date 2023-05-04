using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFDataAccess.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Author?> FindAsync(Expression<Func<Author, bool>> predicate)
            => await _context.Authors.Where(predicate).Include(c => c.Posts).Include(c => c.Tags).Include(c => c.Posts).FirstOrDefaultAsync();

        public async Task<Author?> FindByIdAsync(Guid id)
            => await _context.Authors.Where(c => c.Id == id.ToString()).Include(c => c.Posts).Include(c => c.Tags).Include(c => c.Posts).FirstOrDefaultAsync();

        public async Task<Author?> FindByIdAsync(string id) => await _context.Authors.Where(c => c.Id == id).Include(c => c.Posts).Include(c => c.Tags).Include(c => c.Posts).FirstOrDefaultAsync();

        public async Task<IList<Author>> GetAllAsync() => await _context.Authors.Include(c => c.Posts).Include(c => c.Tags).Include(c => c.Comments).ToListAsync();
    }
}