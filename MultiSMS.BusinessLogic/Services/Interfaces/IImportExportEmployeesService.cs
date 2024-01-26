using Microsoft.AspNetCore.Http;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IImportExportEmployeesService
    {
        Task<object> ImportContactsCsvAsync(IFormFile file);
        string ExportContactsExcel();
    }
}