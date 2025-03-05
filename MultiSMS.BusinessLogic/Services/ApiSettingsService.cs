using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class ApiSettingsService : GenericService<ApiSettings>, IApiSettingsService
    {
        private readonly ApiSettingsSettings _apiSettingsPassword;

        public ApiSettingsService(IGenericRepository<ApiSettings> genericRepository, IOptions<ApiSettingsSettings> apiSettingsPassword) : base(genericRepository)
        {
            _apiSettingsPassword = apiSettingsPassword.Value;
        }

        public async Task<ApiSettings> GetActiveSettingsAsync()
        {
            return await GetAllEntriesQueryable().FirstOrDefaultAsync(s => s.ApiActive == true) ?? throw new InvalidOperationException("No active api found");
        }

        public async Task<ApiSettings> GetSettingsByNameAsync(string settingsName)
        {
            ValidationHelper.ValidateString(settingsName, nameof(settingsName));

            return await GetAllEntriesQueryable().FirstOrDefaultAsync(s => s.ApiName == settingsName)
                                                                      ?? throw new InvalidOperationException($"Could not find api settings by name {settingsName}");
        }

        public async Task<ApiSettings> ChangeSettingsAsync(ApiSettings newSettings)
        {
            ValidationHelper.ValidateObject(newSettings, nameof(newSettings));

            var settingsFromDatabase = await GetSettingsByNameAsync(newSettings.ApiName);
            newSettings.ApiSettingsId = settingsFromDatabase.ApiSettingsId;
            DetachEntity(settingsFromDatabase);

            if (!settingsFromDatabase.ApiActive)
            {
                var previousApi = await GetActiveSettingsAsync();
                previousApi.ApiActive = false;
                await UpdateEntityAsync(previousApi);

                return await UpdateEntityAsync(newSettings);
            }
            else
            {
                return await UpdateEntityAsync(newSettings);
            }
        }

        public bool CheckIfAuthorizationSuccessful(string password)
        {
            ValidationHelper.ValidateString(password, nameof(password));

            return password == _apiSettingsPassword.Password ? true : false;
        }
    }
}
