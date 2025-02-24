using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class EmployeeService : GenericService<Employee>, IEmployeeService
    {
        public EmployeeService(IGenericRepository<Employee> repository) : base(repository)
        {
        }

        public async Task<Employee> GetEmployeeByNameAsync(string name)
        {
            ValidationHelper.ValidateString(name, nameof(name));

            return await GetAllEntriesQueryable().FirstOrDefaultAsync(e => e.Name == name) ?? throw new Exception($"Could not find employee with provided name: {name}");
        }

        public async Task<(List<Employee>, bool)> PaginateEmployeeDataAsync(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            ValidationHelper.ValidateId(firstId, nameof(firstId));
            ValidationHelper.ValidateId(lastId, nameof(lastId));
            ValidationHelper.ValidateId(pageSize, nameof(pageSize));

            IQueryable<Employee> query = GetAllEntriesQueryable().OrderBy(e => e.EmployeeId);

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
