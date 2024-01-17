using Microsoft.AspNetCore.Mvc;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Extensions;
using MultiSMS.Interface.Repositories.Interfaces;
using Newtonsoft.Json;

namespace MultiSMS.MVC.Controllers
{
    public class ServerSmsApiController : Controller
    {
        private readonly IServerSmsService _smsService;
        private readonly ISMSMessageRepository _smsRepository;
        private readonly ILogRepository _logRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;

        public ServerSmsApiController(IServerSmsService smsService, ISMSMessageRepository smsRepository, ILogRepository logRepository, IEmployeeGroupRepository employeeGroupRepository)
        {
            _smsService = smsService;
            _smsRepository = smsRepository;
            _logRepository = logRepository;
            _employeeGroupRepository = employeeGroupRepository;
        }

        public async Task<IActionResult> SendSmsMessage(string text, int chosenGroupId, string chosenGroupName, List<string> additionalPhoneNumbers, string additionalInfo)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUsername = User.GetLoggedInUserName();
            var groupPhoneNumbers = _employeeGroupRepository.GetAllPhoneNumbersForGroupQueryable(chosenGroupId).ToList();

            var data = new Dictionary<string, string>
            {
                {"details", "true"},
                {"speed", "1"},
                {"test", "true" }
            };

            if (additionalPhoneNumbers.Count != 0)
            {
                groupPhoneNumbers.AddRange(additionalPhoneNumbers);
            }

            var phoneNumbersString = string.Join(',', groupPhoneNumbers);

            var response = await _smsService.SendSmsAsync(phoneNumbersString, text, data);

            dynamic responseObject = JsonConvert.DeserializeObject(response) ?? throw new Exception("Deserialization failed");

            data.Add("phone", phoneNumbersString);
            data.Add("text", text);
            data.Add("sender", "Toruń WOL");

            var smsMessage = await _smsRepository.AddEntityToDatabaseAsync(new SMSMessage
            {
                IssuerId = adminId,
                ChosenGroupId = chosenGroupId,
                AdditionalPhoneNumbers = additionalPhoneNumbers,
                AdditionalInformation = additionalInfo,
                DataDictionarySerialized = JsonConvert.SerializeObject(data),
                ServerResponseSerialized = response
            });

            if (responseObject.success == "true")
            {
                await _logRepository.AddEntityToDatabaseAsync(new Log
                {
                    LogType = "Info",
                    LogSource = "SMS",
                    LogMessage = $"Sms został wysłany do grupy {chosenGroupName}",
                    LogCreator = adminUsername,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>()
                    {
                        { "SmsMessages", smsMessage.SMSId },
                        { "Groups", chosenGroupId }
                    })
                });

                return Json(new { Status = "success", Queued = responseObject.queued, Unsent = responseObject.unsent });
            }
            else
            {
                await _logRepository.AddEntityToDatabaseAsync(new Log
                {
                    LogType = "Błąd",
                    LogSource = "SMS",
                    LogMessage = $"Próba wysłania smsa do grupy {chosenGroupName} zakończona niepowodzeniem",
                    LogCreator = adminUsername,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>()
                    {
                        { "SmsMessages", smsMessage.SMSId },
                        { "Groups", chosenGroupId }
                    })
                });

                return Json(new { Status = "success", Queued = responseObject.queued, Unsent = responseObject.unsent });
            }



        }
    }
}
