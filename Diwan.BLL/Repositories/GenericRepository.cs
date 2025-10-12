using Diwan.BLL.Interfaces;
using Diwan.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DiwanDbContext _diwanDbContext;

        public GenericRepository(DiwanDbContext diwanDbContext)
        {
            _diwanDbContext = diwanDbContext;
        }
        public async Task AddAsync(T entity)
        {
            await _diwanDbContext.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _diwanDbContext.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>[]? includes = null)
        {
            IQueryable<T> query = _diwanDbContext.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }
        
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includes = null)
        {
            IQueryable<T> query = _diwanDbContext.Set<T>().Where(predicate);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }
        public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includes = null)
        {
            IQueryable<T> query = _diwanDbContext.Set<T>().Where(predicate);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync();
        }
        public void Update(T entity)
        {
            _diwanDbContext.Update(entity);
        }
    }
}
