using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity 
    {
        private readonly DataContext _context;

        public GenericRepository(DataContext context) 
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAllActiveAsync()
        {
            return await _context.Set<T>().Where(x => x.Active).ToListAsync();
        }

        public async Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T?> FindByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Active = true;

            await _context.Set<T>().AddAsync(entity);

            return entity;
        }

        public T Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            _context.Set<T>().Update(entity);

            return entity;
        }

        public T Remove(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Active = false;

            return entity;
        }

        public List<T> RemoveAll(List<T> entities)
        {
            var deletedEntities = new List<T>();
            foreach (var item in entities)
            {
                var deletedEntitie = Remove(item);
                deletedEntities.Add(deletedEntitie);
            }

            return deletedEntities;
        }
    }
}
