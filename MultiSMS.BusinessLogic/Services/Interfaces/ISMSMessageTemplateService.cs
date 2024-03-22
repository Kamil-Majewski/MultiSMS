﻿using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface ISMSMessageTemplateService : IGenericService<SMSMessageTemplate>
    {
        Task<SMSMessageTemplate> GetTemplateByNameAsync(string name);
    }
}