using Microsoft.AspNetCore.Http;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IImportExportEmployeesService
    {
        string ExportContactsExcel();
        Task<object> ImportContactsAsync(IFormFile file);
        Task<object> ImportContactsCsvByTypeAsync(IFormFile file, string type);
    }
}