using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class LogService : GenericService<Log>, ILogService
    {
        public LogService(IGenericRepository<Log> logRepository) : base(logRepository) { }

        public async Task<(List<Log>, bool)> PaginateLogDataAsync(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            IQueryable<Log> query = GetAllEntriesQueryable().OrderByDescending(l => l.LogId);

            List<Log> paginatedList;
            bool hasMorePages;

            if(moveForward == null)
            {
                query = query.Where(g => g.LogId <= firstId);
            }
            else if (moveForward == true)
            {
                if(lastId == 0)
                {
                    query = query.Take(pageSize + 1);
                }
                else
                {
                    query = query.Where(l => l.LogId < lastId);
                }
            }
            else
            {
                paginatedList = await query.Reverse().Where(l => l.LogId > firstId).Take(pageSize).Select(l => new Log
                {
                    LogId = l.LogId,
                    LogType = l.LogType,
                    LogSource = l.LogSource,
                    LogMessage = l.LogMessage,
                    LogCreationDate = l.LogCreationDate

                }).Reverse().ToListAsync();
                hasMorePages = await query.AnyAsync(l => l.LogId < paginatedList.Last().LogId);

                return (paginatedList, hasMorePages);
            }

            paginatedList = await query.Select(l => new Log
            {
                LogId = l.LogId,
                LogType = l.LogType,
                LogSource = l.LogSource,
                LogMessage = l.LogMessage,
                LogCreationDate = l.LogCreationDate

            }).Take(pageSize).ToListAsync();
            hasMorePages = await query.Skip(pageSize).AnyAsync();

            return (paginatedList, hasMorePages);
        }

        public List<Log> GetLogsBySearchPhrase(string searchPhrase)
        {

            return GetAllEntriesQueryable().Select(l => new Log{LogId = l.LogId, LogCreationDate = l.LogCreationDate, LogType = l.LogType, LogMessage = l.LogMessage, LogSource = l.LogSource })
                .AsEnumerable()
                .Where(l =>
            l.LogType.ToLower().Contains(searchPhrase) ||
            l.LogMessage.ToLower().Contains(searchPhrase) ||
            l.LogSource.ToLower().Contains(searchPhrase) ||
            l.LogCreationDate.ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchPhrase)).ToList();
        }
    }
}
