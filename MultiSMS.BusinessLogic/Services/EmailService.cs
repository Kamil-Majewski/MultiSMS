using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _mailSettings;
        public EmailService(IOptions<EmailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public void SendEmail(Message message)
        {
            ValidationHelper.ValidateObject(message, nameof(message));

            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage);
        }

        public MimeMessage CreateEmailMessage(Message message)
        {
            ValidationHelper.ValidateObject(message, nameof(message));

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_mailSettings.MailAddress));
            var mailboxAddresses = new List<MailboxAddress>();
            foreach (var receipent in message.To)
            {
                mailboxAddresses.Add(MailboxAddress.Parse(receipent));
            }

            emailMessage.To.AddRange(mailboxAddresses);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.MessageContent };

            return emailMessage;
        }
        public void Send(MimeMessage mailMessage)
        {
            ValidationHelper.ValidateObject(_mailSettings, nameof(mailMessage));

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_mailSettings.MailAddress, _mailSettings.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}

