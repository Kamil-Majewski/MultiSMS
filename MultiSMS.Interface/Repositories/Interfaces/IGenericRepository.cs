namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddEntityToDatabaseAsync(T entity);
        Task DeleteEntity(int id);
        IQueryable<T> GetAllEntries();
        Task<T> GetByIdAsync(int id);
        Task<T> UpdateEntity(T entity);
    }
}