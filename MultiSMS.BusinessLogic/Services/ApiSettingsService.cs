using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class ApiSettingsService : GenericService<ApiSettings>, IApiSettingsService
    {
        private readonly IApiSettingsRepository _settingsRepository;

        public ApiSettingsService(IApiSettingsRepository settingsRepository, IGenericRepository<ApiSettings> genericRepository) : base(genericRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<ApiSettings> GetActiveSettingsAsync()
        {
            return await _settingsRepository.GetAllEntries().FirstOrDefaultAsync(s => s.ApiActive == true) ?? throw new Exception("No active api found");
        }

        public async Task<ApiSettings> GetSettingsByNameAsync(string settingsName)
        {
            return await _settingsRepository.GetAllEntries().FirstOrDefaultAsync(s => s.ApiName == settingsName) ?? throw new Exception($"Could not find api settings by name {settingsName}");
        }

        public async Task<ApiSettings> ChangeSettingsAsync(ApiSettings newSettings)
        {
            if (newSettings.ApiActive == false)
            {
                var previousApi = await GetActiveSettingsAsync();
                previousApi.ApiActive = false;
                await UpdateEntityAsync(newSettings);

                newSettings.ApiActive = true;
                return await UpdateEntityAsync(newSettings);
            }
            else
            {
                return await UpdateEntityAsync(newSettings);
            }
        }
    }
}
