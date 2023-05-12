﻿using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Repositories
{
    public class Repository<T, V> : IRepository<T> where T : class where V : Repository<T, V>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<V> _logger;

        public Repository(ApplicationDbContext context, ILogger<V> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region ADD Methods

        public int Add(T entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Set<T>().Add(entity);
                return _context.SaveChanges();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public async Task<int> AddAsync(T entity)
        {
            if (entity == null)
                return -1;

            try
            {
                await _context.Set<T>().AddAsync(entity);
                return await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public int AddRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;

            if (!entities.Any())
                return -1;

            try
            {
                _context.Set<T>().AddRange(entities);
                return _context.SaveChanges();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;

            if (!entities.Any())
                return -1;

            try
            {
                await _context.Set<T>().AddRangeAsync(entities);
                return await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        #endregion ADD Methods

        #region UPDATE Methods

        public int Update(T entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Set<T>().Update(entity);
                return _context.SaveChanges();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public async Task<int> UpdateAsync(T entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Set<T>().Update(entity);
                return await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public int UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            try
            {
                _context.Set<T>().UpdateRange(entities);
                return _context.SaveChanges();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            try
            {
                _context.Set<T>().UpdateRange(entities);
                return await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        #endregion UPDATE Methods

        #region REMOVE Methods

        public int Remove(T entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Set<T>().Remove(entity);
                return _context.SaveChanges();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public async Task<int> RemoveAsync(T entity)
        {
            if (entity == null)
                return -1;

            try
            {
                _context.Set<T>().Remove(entity);
                return await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public int RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            try
            {
                _context.Set<T>().RemoveRange(entities);
                return _context.SaveChanges();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        public async Task<int> RemoveRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return -1;
            if (!entities.Any())
                return -1;

            try
            {
                _context.Set<T>().RemoveRange(entities);
                return await _context.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogCritical("DB Error", ex.Message);
            }

            return -1;
        }

        #endregion REMOVE Methods
    }
}