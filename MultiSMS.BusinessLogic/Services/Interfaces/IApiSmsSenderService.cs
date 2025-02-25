using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IApiSmsSenderService : IGenericService<ApiSmsSender>
    {
        Task AssignUserToSender(int userId, int senderId);
        Task<ApiSmsSender> GetSenderByUserId(int userId);
        Task UnassignUserFromSender(int userId, int senderId);
        Task<List<ApiSmsSender>> GetSendersBySearchPhraseAsync(string searchPhrase);
    }
}