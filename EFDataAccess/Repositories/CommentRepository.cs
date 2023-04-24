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
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(Guid userId) => await _context.Comments.Where(c => c.AuthorId == userId).ToListAsync();

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId) => await _context.Comments.Where(c => c.AuthorId == Guid.Parse(userId)).ToListAsync();
    }
}