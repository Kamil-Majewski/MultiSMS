using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Seeding
{
    public static class SeedApiSettings
    {
        public static List<ApiSettings> GetApiSettingsSeed()
        {
            var apiSettingsList = new List<ApiSettings>();

            apiSettingsList.Add(new ApiSettings
            {
                ApiSettingsId = 1,
                ApiName = "ServerSms",
                ApiActive = true,
                FastChannel = true,
                TestMode = true,
                SenderName = "Torun WOL"
            });

            apiSettingsList.Add(new ApiSettings
            {
                ApiSettingsId = 2,
                ApiName = "SmsApi",
                ApiActive = false,
                FastChannel = true,
                TestMode = true,
                SenderName = "Torun WOL"
            });

            return apiSettingsList;
        }
    }
}
