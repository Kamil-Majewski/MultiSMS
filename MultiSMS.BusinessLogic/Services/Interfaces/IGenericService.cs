using System.Linq.Expressions;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<T> AddEntityToDatabaseAsync(T entity);
        Task<IEnumerable<T>> AddRangeOfEntitiesToDatabaseAsync(IEnumerable<T> entities);
        Task DeleteEntityAsync(int id);
        void DetachEntity(T entity);
        Task<List<T>> GetAllEntriesAsync();
        IQueryable<T> GetAllEntriesQueryable();
        Task<T> GetByIdAsync(int id);
        Task<T> UpdateEntityAsync(T entity);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, IEnumerable<Expression<Func<T, object>>>? propertyExpressions = null);
    }
}