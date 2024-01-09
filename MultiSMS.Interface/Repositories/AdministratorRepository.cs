using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public AdministratorRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Administrator> GetAdministratorByEmailAsync(string email)
        {
            return await _dbContext.AspNetUsers.FirstOrDefaultAsync(a => a.Email == email) ?? throw new Exception("Could not find user with provided Email");
        }
    }
}
