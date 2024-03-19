using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IApiSettingsService : IGenericService<ApiSettings>
    {
        Task<ApiSettings> ChangeSettingsAsync(ApiSettings newSettings);
        Task<ApiSettings> GetSettingsByNameAsync(string settingsName);
        Task<ApiSettings> GetActiveSettingsAsync();
    }
}