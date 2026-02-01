using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includes = null);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includes = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>[]? includes = null);

        Task AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
