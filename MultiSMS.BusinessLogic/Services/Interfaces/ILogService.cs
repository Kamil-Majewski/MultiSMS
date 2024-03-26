using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface ILogService : IGenericService<Log>
    {
        Task<(List<Log>, bool)> PaginateLogDataAsync(int lastId, int pageSize, bool moveForward);
    }
}
