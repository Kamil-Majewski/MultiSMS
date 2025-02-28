using Microsoft.AspNetCore.Http;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IImportExportEmployeesService
    {
        Task<string> ExportContactsExcelAsync();
        Task<ImportResult> ImportContactsAsync(IFormFile file);
        Task<ImportResult> ImportContactsCsvByTypeAsync(IFormFile file, int totalRows, string type);
    }
}