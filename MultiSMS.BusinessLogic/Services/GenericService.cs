﻿using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MultiSMS.BusinessLogic.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<List<T>> GetAllEntriesAsync() => await _repository.GetAllEntries().ToListAsync();

        public virtual IQueryable<T> GetAllEntriesQueryable() => _repository.GetAllEntries();

        public virtual async Task<T> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public virtual async Task<T> AddEntityToDatabaseAsync(T entity) => await _repository.AddEntityToDatabaseAsync(entity);

        public virtual async Task<IEnumerable<T>> AddRangeOfEntitiesToDatabaseAsync(IEnumerable<T> entities) => await _repository.AddRangeOfEntitiesToDatabaseAsync(entities);

        public virtual async Task<T> UpdateEntityAsync(T entity) => await _repository.UpdateEntityAsync(entity);

        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, IEnumerable<Expression<Func<T, object>>>? propertyExpressions = null)
        {
            return await _repository.UpdateRangeAsync(entities, propertyExpressions);
        }

        public virtual async Task DeleteEntityByIdAsync(int id) => await _repository.DeleteEntityByIdAsync(id);

        public virtual void DetachEntity(T entity) => _repository.DetachEntity(entity);

    }
}
