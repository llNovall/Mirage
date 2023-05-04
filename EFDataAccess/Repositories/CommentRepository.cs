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
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Comment?> FindAsync(Expression<Func<Comment, bool>> predicate) => await _context.Comments.Where(predicate).Include(c => c.Replies).Include(c => c.BlogPost).FirstOrDefaultAsync();

        public async Task<Comment?> FindByIdAsync(Guid id) => await _context.Comments.Where(c => c.Id == id.ToString()).Include(c => c.Replies).Include(c => c.BlogPost).FirstOrDefaultAsync();

        public async Task<Comment?> FindByIdAsync(string id) => await _context.Comments.Where(c => c.Id == id).Include(c => c.Replies).Include(c => c.BlogPost).Include(c => c.Author).FirstOrDefaultAsync();

        public async Task<IList<Comment>> GetAllAsync(Guid blogId) => await _context.Comments.Where(c => c.BlogId == blogId.ToString()).Include(c => c.Replies).Include(c => c.BlogPost).Include(c => c.Author).ToListAsync();

        public async Task<IList<Comment>> GetAllAsync() => await _context.Comments.Include(c => c.Replies).Include(c => c.BlogPost).Include(c => c.Author).ToListAsync();
    }
}