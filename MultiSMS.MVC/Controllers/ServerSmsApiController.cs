using Microsoft.AspNetCore.Mvc;
using MultiSMS.BusinessLogic.Services.Interfaces;

namespace MultiSMS.MVC.Controllers
{
    public class ServerSmsApiController : Controller
    {
        private readonly IServerSmsService _smsService;

        public ServerSmsApiController(IServerSmsService smsService)
        {
            _smsService = smsService;
        }

        public IActionResult SendSmsMessage()
        {


            //var smsMessage = await _smsRepsoitory.AddEntityToDatabaseAsync(new SMSMessage
            //{
            //    IssuerId = issuerId,
            //    ChosenGroupId = chosenGroupId,
            //    AdditionalEmployeesIds = additionalEmployeesIds,
            //    AdditionalInformation = additionalInfo,
            //    DataDictionarySerialized = JsonConvert.SerializeObject(data),
            //    ServerResponseSerialized = JsonConvert.SerializeObject(response)
            //});

            //await _logRepository.AddEntityToDatabaseAsync(new Log
            //{
            //    LogType = "Info",
            //    LogSource = "SMS",
            //    LogMessage = $"Sms od {adminUsername} został wysłany do grupy {chosenGroupName}",
            //    LogCreator = adminUsername,
            //    LogCreatorId = issuerId,
            //    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>()
            //    {
            //        {"SmsMessages", smsMessage.SMSId },
            //        {"Groups", chosenGroupId }
            //    })
            //});
            throw new NotImplementedException();
        }

    }
}
