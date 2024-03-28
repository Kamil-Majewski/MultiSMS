using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Extensions;
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
            IQueryable<Log> query = _logRepository.GetAllEntries().OrderBy(l => l.LogId);

            List<Log> paginatedList;
            bool hasMorePages;

            if (moveForward)
            {
                query = query.Where(l => l.LogId > lastId);
            }
            else
            {
                paginatedList = await query.Reverse().Where(l => l.LogId < firstId).Take(pageSize).Reverse().ToListAsync();
                hasMorePages = await query.AnyAsync(l => l.LogId > paginatedList.Last().LogId);

                return (paginatedList, hasMorePages);
            }

            paginatedList = await query.Take(pageSize).ToListAsync();
            hasMorePages = await query.Skip(pageSize).AnyAsync();

            return (paginatedList, hasMorePages);
        }

        public async Task<List<Log>> GetLogsBySearchPhrase(string searchPhrase)
        {
            return await _logRepository.GetAllEntries().Where(l =>
            l.LogType.ContainsCaseInsensitive(searchPhrase) ||
            l.LogMessage.ContainsCaseInsensitive(searchPhrase) ||
            l.LogSource.ContainsCaseInsensitive(searchPhrase) ||
            l.LogCreationDate.ToString().ContainsCaseInsensitive(searchPhrase)).ToListAsync();
        }
    }
}
