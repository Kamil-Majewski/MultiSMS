namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IServerSmsService
    {
        Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data);
    }
}