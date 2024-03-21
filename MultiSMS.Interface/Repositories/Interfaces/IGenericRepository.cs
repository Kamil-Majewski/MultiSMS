namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddEntityToDatabaseAsync(T entity);
        Task<IEnumerable<T>> AddRangeOfEntitiesToDatabaseAsync(IEnumerable<T> entities);
        Task DeleteEntityAsync(int id);
        IQueryable<T> GetAllEntries();
        Task<T> GetByIdAsync(int id);
        Task<T> UpdateEntityAsync(T entity);
        void DetachEntity(T entity);
    }
}