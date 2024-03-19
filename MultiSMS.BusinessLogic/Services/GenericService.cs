using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public List<T> GetAllEntries()
        {
            return _repository.GetAllEntries().ToList();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<T> AddEntityToDatabaseAsync(T entity)
        {
            return await _repository.AddEntityToDatabaseAsync(entity);
        }

        public async Task<IEnumerable<T>> AddRangeOfEntitiesToDatabase(IEnumerable<T> entities)
        {
            return await _repository.AddRangeOfEntitiesToDatabaseAsync(entities);
        }

        public async Task<T> UpdateEntityAsync(T entity)
        {
            return await _repository.UpdateEntityAsync(entity);
        }

        public async Task DeleteEntityAsync(int id)
        {
            await _repository.DeleteEntityAsync(id);
        }
    }
}
