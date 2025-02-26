using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MultiSMS.Interface.Repositories
{
    public class LocalGenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly LocalDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public LocalGenericRepository(LocalDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._dbSet = dbContext.Set<T>(); ;
        }

        public IQueryable<T> GetAllEntries()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id) ?? throw new ArgumentNullException($"Could not find entity {nameof(T)} with given Id");
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

        public async Task<IEnumerable<T>> AddRangeOfEntitiesToDatabaseAsync(IEnumerable<T> entities)
        {
            if (entities is null)
            {
                throw new ArgumentNullException($"Provided IEnumerable entities {nameof(T)} was null");
            }

            await _dbSet.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
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

        public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, IEnumerable<Expression<Func<T, object>>>? propertyExpressions = null)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentNullException("Provided entities collection was null or empty.");
            }

            foreach (var entity in entities)
            {
                _dbContext.Attach(entity);

                if (propertyExpressions != null)
                {
                    foreach (var propertyExpression in propertyExpressions)
                    {
                        _dbContext.Entry(entity).Property(propertyExpression).IsModified = true;
                    }
                }
                else
                {
                    _dbContext.Entry(entity).State = EntityState.Modified;
                }
            }

            await _dbContext.SaveChangesAsync();

            return entities;
        }

        public async Task DeleteEntityByIdAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id) ?? throw new ArgumentNullException($"Could not find entity {nameof(T)} with given Id");
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteEntityAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void DetachEntity(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }
    }
}
