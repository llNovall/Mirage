namespace Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
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