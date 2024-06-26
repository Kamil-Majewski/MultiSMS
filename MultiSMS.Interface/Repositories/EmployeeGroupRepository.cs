﻿using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class EmployeeGroupRepository : IEmployeeGroupRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public EmployeeGroupRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddGroupMemberAsync(int groupId, int employeeId)
        {
            try
            {
                await _dbContext.AddAsync(new EmployeeGroup { EmployeeId = employeeId, GroupId = groupId });
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Error occured while trying to add group member");
            };
        }

        public IQueryable<int> GetAllEmployeesIdsForGroupQueryable(int groupId)
        {
            return _dbContext.EmployeeGroups.Where(eg => eg.GroupId == groupId).Select(eg => eg.EmployeeId);
        }

        public IQueryable<int> GetAllGroupIdsForEmployeeQueryable(int employeeId)
        {
            return _dbContext.EmployeeGroups.Where(eg => eg.EmployeeId == employeeId).Select(eg => eg.GroupId);
        }

        public async Task RemoveGroupMember(int groupId, int employeeId)
        {
            var groupMember = await _dbContext.EmployeeGroups.FirstOrDefaultAsync(eg => eg.GroupId == groupId && eg.EmployeeId == employeeId) ?? throw new Exception("Could not find the group with provided id that contains employee with given id");
            _dbContext.EmployeeGroups.Remove(groupMember);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<string> GetAllGroupNamesForEmployeeQueryable(int employeeId)
        {
            return _dbContext.EmployeeGroups.Where(eg => eg.EmployeeId == employeeId).Include(eg => eg.Group).Select(g => g.Group.GroupName);
        }

        public IQueryable<string> GetAllPhoneNumbersForGroupQueryable(int groupId)
        {
            return _dbContext.EmployeeGroups.Where(eg => eg.GroupId == groupId).Include(eg => eg.Employee).Select(e => e.Employee.PhoneNumber);
        }

        public IQueryable<string> GetAllActiveEmployeesPhoneNumbersForGroupQueryable(int groupId)
        {
            return _dbContext.EmployeeGroups.Where(eg => eg.GroupId == groupId).Include(eg => eg.Employee).Where(e => e.Employee.IsActive == true).Select(e => e.Employee.PhoneNumber);
        }

        public IQueryable<Employee> GetAllEmployeesForGroupQueryable(int groupId)
        {
            return _dbContext.EmployeeGroups.Where(eg => eg.GroupId == groupId).Include(eg => eg.Employee).Select(e => e.Employee);
        }

        public Dictionary<int, IEnumerable<string>> GetDictionaryOfEmployeeIdAndGroupNames()
        {
            var result = _dbContext.EmployeeGroups
                .Include(eg => eg.Group)
                .GroupBy(eg => eg.EmployeeId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(eg => eg.Group.GroupName));

            return result;
        }
    }
}
