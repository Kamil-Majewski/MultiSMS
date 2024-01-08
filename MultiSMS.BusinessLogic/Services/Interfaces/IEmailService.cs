using MimeKit;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IEmailService
    {
        MimeMessage CreateEmailMessage(Message message);
        void Send(MimeMessage mailMessage);
        void SendEmail(Message message);
    }
}