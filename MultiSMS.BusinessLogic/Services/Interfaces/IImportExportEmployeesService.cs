using Microsoft.AspNetCore.Http;
using MultiSMS.BusinessLogic.DTO;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IImportExportEmployeesService
    {
        string ExportContactsExcel();
        Task<ImportResultDTO> ImportContactsAsync(IFormFile file);
        Task<ImportResultDTO> ImportContactsCsvByTypeAsync(IFormFile file, string type);
    }
}