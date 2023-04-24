using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        #region GET Methods

        public T? FindById(Guid id);

        public T? FindById(string id);

        public T? Find(Expression<Func<T, bool>> predicate);

        public Task<T?> FindByIdAsync(Guid id);

        public Task<T?> FindByIdAsync(string id);

        public Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        public IEnumerable<T> GetAll();

        #endregion GET Methods

        #region ADD Methods

        public int Add(T entity);

        public Task<int> AddAsync(T entity);

        public int AddRange(IEnumerable<T> entities);

        public Task<int> AddRangeAsync(IEnumerable<T> entities);

        #endregion ADD Methods

        #region UPDATE Methods

        public int Update(T entity);

        public Task<int> UpdateAsync(T entity);

        public int UpdateRange(IEnumerable<T> entities);

        public Task<int> UpdateRangeAsync(IEnumerable<T> entities);

        #endregion UPDATE Methods

        #region REMOVE Methods

        public int Remove(T entity);

        public Task<int> RemoveAsync(T entity);

        public int RemoveRange(IEnumerable<T> entities);

        public Task<int> RemoveRangeAsync(IEnumerable<T> entities);

        #endregion REMOVE Methods
    }
}