using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IApiTokenService : IGenericService<ApiToken>
    {
        Task<List<ApiToken>> GetApiTokensBySearchPhraseAsync(string searchPhrase);
    }
}