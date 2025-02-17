using Microsoft.Extensions.DependencyInjection;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Factory.Interface;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Factory
{
    public class SmsClientFactory : ISmsClientFactory
    {
        private readonly IApiSettingsService _settingsService;
        private readonly IServiceProvider _serviceProvider;

        public SmsClientFactory(IApiSettingsService settingsService, IServiceProvider serviceProvider)
        {
            _settingsService = settingsService;
            _serviceProvider = serviceProvider;
        }

        public async Task<ISmsClient> GetClientAsync()
        {
            var apiSettings = await _settingsService.GetActiveSettingsAsync();

            switch (apiSettings.ApiSettingsId)
            {
                case 1:
                    return _serviceProvider.GetRequiredService<ServerSmsClient>();
                case 2:
                    return _serviceProvider.GetRequiredService<SmsApiClient>();
                default:
                    throw new Exception("Unknown API");
            }
        }
    }
}
