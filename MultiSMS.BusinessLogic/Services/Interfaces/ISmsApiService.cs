namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface ISmsApiService
    {
        Task<string> SendSmsAsync(string to, string message, Dictionary<string, string> data);
    }
}