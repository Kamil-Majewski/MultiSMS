using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Seeding
{
    internal static class SeedApiSettings
    {
        internal static List<ApiSettings> GetApiSettingsSeed()
        {
            var apiSettingsList = new List<ApiSettings>
            {
                new ApiSettings
                {
                    ApiSettingsId = 1,
                    ApiName = "ServerSms",
                    ApiActive = true,
                    FastChannel = true,
                    TestMode = true,
                },
                new ApiSettings
                {
                    ApiSettingsId = 2,
                    ApiName = "SmsApi",
                    ApiActive = false,
                    FastChannel = true,
                    TestMode = true,
                },
                new ApiSettings
                {
                    ApiSettingsId = 3,
                    ApiName = "mProfi",
                    ApiActive = false,
                    FastChannel = true,
                    TestMode = false,
                }
            };

            return apiSettingsList;
        }
    }
}
