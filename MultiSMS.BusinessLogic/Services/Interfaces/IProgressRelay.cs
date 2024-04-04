namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IProgressRelay
    {
        Task RelayProgressAsync(string method, string progress);
    }
}
