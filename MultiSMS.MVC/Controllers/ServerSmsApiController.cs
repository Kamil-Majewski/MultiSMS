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
        private readonly ILogRepository _logRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;
        private readonly IAdministratorService _administratorService;
        private readonly IGroupRepository _groupRepository;

        public ServerSmsApiController(IServerSmsService smsService, ILogRepository logRepository, IEmployeeGroupRepository employeeGroupRepository, IAdministratorService administratorService, IGroupRepository groupRepository)
        {
            _smsService = smsService;
            _logRepository = logRepository;
            _employeeGroupRepository = employeeGroupRepository;
            _administratorService = administratorService;
            _groupRepository = groupRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SendSmsMessage(string text, int chosenGroupId, string chosenGroupName, string additionalPhoneNumbers, string additionalInfo)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            Group chosenGroup = new Group();

            if (chosenGroupId > 0)
            {
                chosenGroup = await _groupRepository.GetByIdAsync(chosenGroupId);
            }
            
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

            var phoneNumbersString = string.Join(',', groupPhoneNumbers);

            var response = await _smsService.SendSmsAsync(phoneNumbersString, text, data); //Call API

            var dataForSmsEntity = new Dictionary<string, string>
            {
                {"details", "true"},
                {"speed", "1"},
                {"test", "true" },
                {"phone", phoneNumbersString },
                {"text", text },
                {"sender", "Toruń WOL" }
            };

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

                var smsMessage = new SMSMessage
                {
                    ChosenGroupId = chosenGroupId,
                    AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                    AdditionalInformation = additionalInfo,
                    Settings = dataForSmsEntity,
                    ServerResponse = successResponse
                };

                await _logRepository.AddEntityToDatabaseAsync(new Log
                {
                    LogType = "Info",
                    LogSource = "SMS",
                    LogMessage = logMessage,
                    LogCreator = admin.UserName,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>()
                    {
                        { "SmsMessages", JsonConvert.SerializeObject(smsMessage) },
                        { "Groups", JsonConvert.SerializeObject(chosenGroup) },
                        { "Creator", JsonConvert.SerializeObject(admin) }
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

                    var smsMessage = new SMSMessage
                    {
                        ChosenGroupId = chosenGroupId,
                        AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                        AdditionalInformation = additionalInfo,
                        Settings = dataForSmsEntity,
                        ServerResponse = errorResponse
                    };

                    await _logRepository.AddEntityToDatabaseAsync(new Log
                    {
                        LogType = "Błąd",
                        LogSource = "SMS",
                        LogMessage = logMessage,
                        LogCreator = admin.UserName,
                        LogCreatorId = adminId,
                        LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>()
                        {
                            { "SmsMessages", JsonConvert.SerializeObject(smsMessage) },
                            { "Groups", JsonConvert.SerializeObject(chosenGroup) },
                            { "Creator", JsonConvert.SerializeObject(admin) }
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
