using Evacuation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
