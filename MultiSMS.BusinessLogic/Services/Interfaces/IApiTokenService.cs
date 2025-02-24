﻿using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IApiTokenService
    {
        Task<List<ApiToken>> GetApiTokensBySearchPhraseAsync(string searchPhrase);
    }
}