using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public UserRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("Could not find user with given ID");
        }

        public async Task<IQueryable<User>> GetUsersBySurnameAsync(string surname)
        {
            return await Task.FromResult(_dbContext.Users.Where(u => u.Surname == surname));
        }

        public async Task<IQueryable<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_dbContext.Users);
        }

    }
}
