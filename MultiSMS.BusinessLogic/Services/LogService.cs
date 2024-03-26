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

        public async Task<(List<Log>, bool)> PaginateLogDataAsync(int lastId, int pageSize, bool moveForward)
        {
            IQueryable<Log> query;
            bool hasMorePages;

            if (moveForward)
            {
                query = _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId > lastId);
                hasMorePages = query.Count() > pageSize;
            }
            else
            {
                query = _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId <= lastId);
                hasMorePages = true;
            }
            
            var paginatedList = await query.Take(pageSize).ToListAsync();

            return (paginatedList, hasMorePages);
        }
    }
}
