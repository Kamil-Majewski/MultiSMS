using MultiSMS.BusinessLogic.DTO;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IImportResultService
    {
        Task<ImportResultDTO> GetImportResultDtoByIdAsync(int id);
        Task<ImportResult> AddEntityToDatabaseAsync(ImportResult entity);
    }
}