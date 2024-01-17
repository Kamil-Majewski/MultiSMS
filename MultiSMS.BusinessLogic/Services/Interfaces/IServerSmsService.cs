namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IServerSmsService
    {
        Task<Object> SendSmsAsync(string phone, string text, string sender, Dictionary<string, string> data);
    }
}