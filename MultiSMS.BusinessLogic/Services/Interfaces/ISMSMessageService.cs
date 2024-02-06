using MultiSMS.BusinessLogic.DTO;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface ISMSMessageService
    {
        Task<SmsMessageDTO> GetSmsMessageDtoById(int id);
    }
}