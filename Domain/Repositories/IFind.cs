using Domain.Entities;
using System.Linq.Expressions;

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