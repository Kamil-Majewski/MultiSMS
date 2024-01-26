using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IEntitiesValidationService
    {
        bool CheckEmployeeValidity(Employee employee);
    }
}