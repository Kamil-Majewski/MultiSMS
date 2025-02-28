using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IApiSmsSenderService : IGenericService<ApiSmsSender>
    {
        Task<bool> AssignUserToSender(int userId, int senderId);
        Task<ApiSmsSender> GetSenderByUserId(int userId);
        Task<List<ApiSmsSender>> GetSendersBySearchPhraseAsync(string searchPhrase);
        Task<bool> UnassignUserFromSender(int userId);
    }
}