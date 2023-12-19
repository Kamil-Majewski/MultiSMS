using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext dbcontext)
        {
            _dbContext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            this._dbSet = dbcontext.Set<T>();
        }

        public IQueryable<T> GetAllEntries()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id) ?? throw new Exception($"Could not find entity {nameof(T)} with given Id");
        }

        public async Task<T> AddEntityToDatabaseAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException($"Provided entity {nameof(T)} was null");
            }

            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateEntityAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException($"Provided entity {nameof(T)} was null");
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteEntityAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id) ?? throw new Exception($"Could not find entity {nameof(T)} with given Id");
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
