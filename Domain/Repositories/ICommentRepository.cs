using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(Guid userId);

        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId);
    }
}