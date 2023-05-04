using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IFind<T> where T : BaseEntity
    {
        Task<T?> FindByIdAsync(Guid id);

        Task<T?> FindByIdAsync(string id);

        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        Task<IList<T>> GetAllAsync();
    }
}