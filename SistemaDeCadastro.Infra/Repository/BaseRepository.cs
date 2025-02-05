using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class BaseRepository<T>: IBaseRepository<T> where T : class
    {
        private readonly DbContext _context;

        public BaseRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Create(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> FindBy(Expression<Func<T, bool>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        } 
        public async Task DeleteRange(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public async Task Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
