using System.Linq.Expressions;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddEntityToDatabaseAsync(T entity);
        Task<IEnumerable<T>> AddRangeOfEntitiesToDatabaseAsync(IEnumerable<T> entities);
        Task DeleteEntityByIdAsync(int id);
        Task DeleteEntityAsync(T entity);
        IQueryable<T> GetAllEntries();
        Task<T> GetByIdAsync(int id);
        Task<T> UpdateEntityAsync(T entity);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, IEnumerable<Expression<Func<T, object>>>? propertyExpressions = null);
        void DetachEntity(T entity);
    }
}