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

        public async Task<(List<Log>, bool)> PaginateLogDataAsync(int firstId, int lastId, int pageSize, bool moveForward)
        {
            List<Log> paginatedList;
            bool hasMorePages;

            if (moveForward)
            {
                var query = _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId > lastId);
                paginatedList = await query.Take(pageSize).ToListAsync();
                hasMorePages = query.Count() > pageSize;
            }
            else
            {
                paginatedList = await _logRepository.GetAllEntries().OrderBy(l => l.LogId).Reverse().Where(l => l.LogId < firstId).Take(pageSize).Reverse().ToListAsync();
                hasMorePages = _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId > paginatedList.Last().LogId).Count() > 0;
            }
            return (paginatedList, hasMorePages);
        }
    }
}
