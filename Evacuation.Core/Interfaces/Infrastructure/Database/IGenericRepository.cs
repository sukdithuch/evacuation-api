using Evacuation.Domain.Entities;
using System.Linq.Expressions;

namespace Evacuation.Core.Interfaces.Infrastructure.Database
{
    public interface IGenericRepository<T> where T : IBaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllActiveAsync();
        Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindByIdAsync(int id);        
        Task<T> AddAsync(T entity);
        T Update(T entity);
        T Remove(T entity);
        List<T> RemoveAll(List<T> entities);
    }
}
