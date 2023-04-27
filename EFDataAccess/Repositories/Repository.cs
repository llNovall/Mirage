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
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GET Methods

        public T? Find(Expression<Func<T, bool>> predicate) => _context.Set<T>().Where(predicate).FirstOrDefault();

        public T? FindById(string id) => _context.Set<T>().Find(Guid.Parse(id));

        public T? FindById(Guid id) => _context.Set<T>().Find(id.ToString());

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();

        public async Task<T?> FindByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id.ToString());

        public async Task<T?> FindByIdAsync(string id) => await _context.Set<T>().FindAsync(Guid.Parse(id));

        public IEnumerable<T> GetAll() => _context.Set<T>();

        #endregion GET Methods

        #region ADD Methods

        public int Add(T entity)
        {
            if (entity == null)
                return -1;

            _context.Set<T>().Add(entity);
            return _context.SaveChanges();
        }

        public async Task<int> AddAsync(T entity)
        {
            if (entity == null)
                return -1;

            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public int AddRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;

            if (!entities.Any())
                return -1;

            _context.Set<T>().AddRange(entities);
            return _context.SaveChanges();
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;

            if (!entities.Any())
                return -1;

            await _context.Set<T>().AddRangeAsync(entities);
            return await _context.SaveChangesAsync();
        }

        #endregion ADD Methods

        #region UPDATE Methods

        public int Update(T entity)
        {
            if (entity == null)
                return -1;

            _context.Set<T>().Update(entity);
            return _context.SaveChanges();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            if (entity == null)
                return -1;

            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync();
        }

        public int UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            _context.Set<T>().UpdateRange(entities);
            return _context.SaveChanges();
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            _context.Set<T>().UpdateRange(entities);
            return await _context.SaveChangesAsync();
        }

        #endregion UPDATE Methods

        #region REMOVE Methods

        public int Remove(T entity)
        {
            if (entity == null)
                return -1;

            _context.Set<T>().Remove(entity);
            return _context.SaveChanges();
        }

        public async Task<int> RemoveAsync(T entity)
        {
            if (entity == null)
                return -1;

            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public int RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            _context.Set<T>().RemoveRange(entities);
            return _context.SaveChanges();
        }

        public async Task<int> RemoveRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            _context.Set<T>().RemoveRange(entities);
            return await _context.SaveChangesAsync();
        }

        #endregion REMOVE Methods
    }
}