using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.ServerSms;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SendSmsMessage(string text, int chosenGroupId, string chosenGroupName, string additionalPhoneNumbers, string additionalInfo)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUsername = User.GetLoggedInUserName();
            
            var groupPhoneNumbers = _employeeGroupRepository.GetAllActiveEmployeesPhoneNumbersForGroupQueryable(chosenGroupId).ToList();

            if (additionalPhoneNumbers != null)
            {
                var additionalNumbers = additionalPhoneNumbers.Split(',').ToList();

                if (additionalNumbers.Count != 0)
                {
                    groupPhoneNumbers.AddRange(additionalNumbers);
                }
            }

            var data = new Dictionary<string, string>();

            var dataForSmsEntity = new Dictionary<string, string>
            {
                {"details", "true"},
                {"speed", "1"},
                {"test", "true" }
            };


            var phoneNumbersString = string.Join(',', groupPhoneNumbers);

            var response = await _smsService.SendSmsAsync(phoneNumbersString, text, data); //Call API

            dataForSmsEntity.Add("phone", phoneNumbersString);
            dataForSmsEntity.Add("text", text);
            dataForSmsEntity.Add("sender", "Toruń WOL");

            var smsMessage = await _smsRepository.AddEntityToDatabaseAsync(new SMSMessage //Save SMSMessage entity to database in order to link it to the log entity
            {
                IssuerId = adminId,
                ChosenGroupId = chosenGroupId,
                AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                AdditionalInformation = additionalInfo,
                DataDictionarySerialized = JsonConvert.SerializeObject(dataForSmsEntity),
                ServerResponseSerialized = response
            });

            try //try to deserialize response into entities that correspond with response structure and then act depending on the outcome
            {
                string logMessage;
                if (chosenGroupName == null && chosenGroupId == 0)
                {
                    logMessage = "Sms został wysłany do pojedyńczych numerów";
                }
                else
                {
                    logMessage = $"Sms został wysłany do grupy {chosenGroupName}";
                }
                ServerSmsSuccessResponse successResponse = JsonConvert.DeserializeObject<ServerSmsSuccessResponse>(response) ?? throw new Exception("Deserialization failed");
                await _logRepository.AddEntityToDatabaseAsync(new Log
                {
                    LogType = "Info",
                    LogSource = "SMS",
                    LogMessage = logMessage,
                    LogCreator = adminUsername,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>()
                    {
                        { "SmsMessages", smsMessage.SMSId },
                        { "Groups", chosenGroupId }
                    })
                });

                return Json(new { Status = successResponse.Success, Queued = successResponse.Queued, Unsent = successResponse.Unsent });
            }
            catch (JsonException)
            {
                try //deserialization into SuccessResponse failed, try to deserialize into ErrorResponse
                {
                    string logMessage;
                    if (chosenGroupName == null && chosenGroupId == 0)
                    {
                        logMessage = "Nie udało się wysłać sms'a do pojedyńczych numerów";
                    }
                    else
                    {
                        logMessage = $"Nie udało się wysłać sms'a do grupy {chosenGroupName}";
                    }
                    ServerSmsErrorResponse errorResponse = JsonConvert.DeserializeObject<ServerSmsErrorResponse>(response) ?? throw new Exception("Deserialization failed");
                    await _logRepository.AddEntityToDatabaseAsync(new Log
                    {
                        LogType = "Błąd",
                        LogSource = "SMS",
                        LogMessage = logMessage,
                        LogCreator = adminUsername,
                        LogCreatorId = adminId,
                        LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>()
                        {
                            { "SmsMessages", smsMessage.SMSId },
                            { "Groups", chosenGroupId }
                        })
                    });

                    return Json(new { Status = "failed", Code = errorResponse.Error.Code, Message = errorResponse.Error.Message });
                }
                catch (JsonException)
                {
                    throw new Exception("Error deserializing objects: response structure doesn't fit the object structure.");
                }
            }
        }
    }
}
