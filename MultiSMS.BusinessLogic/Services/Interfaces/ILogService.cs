﻿using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface ILogService : IGenericService<Log>
    {
        Task<(List<Log>, bool)> PaginateLogDataAsync(int firstId, int lastId, int pageSize, bool? moveForward);
        List<Log> GetLogsBySearchPhrase(string searchPhrase);
    }
}
