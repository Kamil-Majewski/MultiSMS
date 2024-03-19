namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<T> AddEntityToDatabaseAsync(T entity);
        Task<IEnumerable<T>> AddRangeOfEntitiesToDatabase(IEnumerable<T> entities);
        Task DeleteEntityAsync(int id);
        List<T> GetAllEntries();
        Task<T> GetByIdAsync(int id);
        Task<T> UpdateEntityAsync(T entity);
    }
}