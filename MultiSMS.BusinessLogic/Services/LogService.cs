using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class LogService : GenericService<Log>, ILogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository, IGenericRepository<Log> repository) : base(repository)
        {
            _logRepository = logRepository;
        }

        public async Task<List<Log>> PaginateLogDataAsync(int lastId, int pageSize)
        {
            return await _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId > lastId).Take(pageSize).ToListAsync();
        }
    }
}
