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
            List<Log> paginatedList;

            if (moveForward)
            {
                query = _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId > lastId);
                paginatedList = await query.Take(pageSize).ToListAsync();
            }
            else
            {
                query = _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId <= lastId);
                if(query.Count() > pageSize)
                {
                    paginatedList = await query.Reverse().Take(pageSize).ToListAsync();
                }
                else
                {
                    paginatedList = await query.Take(pageSize).ToListAsync();
                }
            }
            var hasMorePages = _logRepository.GetAllEntries().OrderBy(l => l.LogId).Where(l => l.LogId > lastId).Count() > pageSize;
            return (paginatedList, hasMorePages);
        }
    }
}
