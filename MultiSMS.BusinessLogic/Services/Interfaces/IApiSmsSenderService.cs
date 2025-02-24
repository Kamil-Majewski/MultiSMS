using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IApiSmsSenderService
    {
        Task AssignUserToSender(int userId, int senderId);
        Task<ApiSmsSender> CreateNewSenderAsync(ApiSmsSender sender);
        Task DeleteSenderAsync(int senderId);
        Task<IEnumerable<ApiSmsSender>> GetAllSenders();
        Task<ApiSmsSender> GetSenderById(int senderId);
        Task<ApiSmsSender> GetSenderByUserId(int userId);
        Task UnassignUserFromSender(int userId, int senderId);
        Task<ApiSmsSender> UpdateSenderAsync(ApiSmsSender sender);
    }
}