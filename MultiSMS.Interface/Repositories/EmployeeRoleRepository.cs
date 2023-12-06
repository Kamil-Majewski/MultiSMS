using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class EmployeeRoleRepository : IEmployeeRoleRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public EmployeeRoleRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IQueryable<EmployeeRole>> GetAllEmployeeRoles()
        {
            return await Task.FromResult(_dbContext.EmployeeRoles);
        }

        public async Task<EmployeeRole> GetEmployeeRoleByIdAsync(int roleId)
        {
            return await _dbContext.EmployeeRoles.FirstOrDefaultAsync(r => r.RoleId == roleId) ?? throw new Exception("Could not find role with given ID");
        }

        public async Task<EmployeeRole> CreateEmployeeRoleAsync(EmployeeRole role)
        {
            await _dbContext.EmployeeRoles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            return role;
        }

        public async Task<EmployeeRole> UpdateEmployeeRoleAsync(EmployeeRole updatedRole)
        {
            var roleFromDb = await GetEmployeeRoleByIdAsync(updatedRole.RoleId);
            roleFromDb = updatedRole;
            await _dbContext.SaveChangesAsync();
            return roleFromDb;
        }

        public async Task DeleteEmployeeRoleAsync(EmployeeRole roleToBeDeleted)
        {
            _dbContext.EmployeeRoles.Remove(roleToBeDeleted);
            await _dbContext.SaveChangesAsync();
        }

    }
}
