using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class ApiTokenService : GenericService<ApiToken>, IApiTokenService
    {
        public ApiTokenService(IGenericRepository<ApiToken> apiTokenRepository) : base(apiTokenRepository) { }

        public async Task<List<ApiToken>> GetApiTokensBySearchPhraseAsync(string searchPhrase)
        {
            ValidationHelper.ValidateString(searchPhrase, nameof(searchPhrase));

            return await GetAllEntriesQueryable().Where(s => s.Provider.ToLower().Contains(searchPhrase)
            || (string.IsNullOrEmpty(s.Description) ? "Brak opisu" : s.Description).ToLower().Contains(searchPhrase)).ToListAsync();
        }
    }
}
