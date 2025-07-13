using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MsCore.Framework.Repository.Interfaces;
using MsCore.Framework.Repository.Models;
using System.Linq.Expressions;

namespace MsCore.Framework.Repository.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> AnyAsync()
        {
            return await _dbSet.AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task BulkDeleteAsync(IEnumerable<TEntity> entities)
        {
            await _context.BulkDeleteAsync(entities.ToList());
        }

        public async Task BulkDeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            await _context.BulkDeleteAsync(_dbSet.Where(predicate).ToList());
        }

        public async Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            await _context.BulkInsertAsync(entities.ToList());
        }

        public async Task BulkUpdateAsync(IEnumerable<TEntity> entities)
        {
            await _context.BulkUpdateAsync(entities.ToList());
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<int> ExecuteSqlAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> FromSqlAsync(string sql, params object[] parameters)
        {
            return await _dbSet.FromSqlRaw(sql, parameters).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var totalRecords = await _dbSet.CountAsync();
            var data = await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<TEntity>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            var totalRecords = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<TEntity>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TKey>>? orderBy = null, bool descending = false)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            var totalRecords = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<TEntity>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TKey>>? orderBy = null, bool descending = false, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            var totalRecords = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<TEntity>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync<TKey>(int pageNumber, int pageSize, IQueryable<TEntity> query, Expression<Func<TEntity, TKey>>? orderBy = null, bool descending = false)
        {
            if (orderBy != null)
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

                var totalRecords = await query.CountAsync();

                var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PagedResult<TEntity>(data, totalRecords, pageNumber, pageSize);
        }

        public IQueryable<TEntity> GetQueryable(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return query;
        }

        public async Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await _context.Set<TEntity>().MaxAsync(selector);
        }

        public async Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await _context.Set<TEntity>().MinAsync(selector);
        }

        public async Task<IEnumerable<TResult>> ProjectToAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return await _dbSet.AsNoTracking().Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> ProjectToAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return await _dbSet.AsNoTracking().Where(predicate).Select(selector).ToListAsync();
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task RemoveByIdAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
                _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task RemoveRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            _dbSet.RemoveRange(await _dbSet.Where(predicate).ToListAsync());
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.SingleOrDefaultAsync();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return query;
        }
    }
}