using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow.ToString("u");
            entity.UpdatedAt = DateTime.UtcNow.ToString("u");
            entity.Active = true;

            await _context.Set<T>().AddAsync(entity);

            return entity;
        }

        public T Update(T entity)
        {
            entity.UpdatedAt += DateTime.UtcNow.ToString("u");

            _context.Set<T>().Update(entity);

            return entity;
        }

        public T Remove(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow.ToString("u");
            entity.Active = false;

            return entity;
        }

        //public T RemoveAll(IEnumerable<T> entities)
        //{
        //    foreach (var item in entities)
        //    {
        //        Remove(item);
        //    }
        //}
    }
}
