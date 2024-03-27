using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class EmployeeService : GenericService<Employee>, IEmployeeService
    {
        private readonly IGenericRepository<Employee> _repository;
        public EmployeeService(IGenericRepository<Employee> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<Employee> GetEmployeeByNameAsync(string name)
        {
            return await _repository.GetAllEntries().FirstOrDefaultAsync(e => e.Name == name) ?? throw new Exception($"Could not find employee with provided name: {name}");
        }

        public async Task<(List<Employee>, bool)> PaginateEmployeeDataAsync(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            IQueryable<Employee> query = _repository.GetAllEntries().OrderBy(e => e.EmployeeId);

            List<Employee> paginatedList;
            bool hasMorePages;

            if(moveForward == null)
            {
                query = query.Where(e => e.EmployeeId >= firstId);

            }
            else if (moveForward == true)
            {
                query = query.Where(e => e.EmployeeId > lastId);
            }
            else
            {
                paginatedList = await query.Reverse().Where(e => e.EmployeeId < firstId).Take(pageSize).Reverse().ToListAsync();
                hasMorePages = await query.AnyAsync(e => e.EmployeeId > paginatedList.Last().EmployeeId);

                return (paginatedList, hasMorePages);
            }

            paginatedList = await query.Take(pageSize).ToListAsync();
            hasMorePages = await query.Skip(pageSize).AnyAsync();

            return (paginatedList, hasMorePages);
        }
    }
}
