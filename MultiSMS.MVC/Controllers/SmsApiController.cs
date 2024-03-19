using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.BusinessLogic.Strategy;
using MultiSMS.BusinessLogic.Strategy.Intefaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.ServerSms;
using MultiSMS.Interface.Entities.SmsApi;
using MultiSMS.Interface.Extensions;
using Newtonsoft.Json;

namespace MultiSMS.MVC.Controllers
{
    public class SmsApiController : Controller
    {
        private readonly IOptions<ServerSmsSettings> _serverSmsSettings;
        private readonly IOptions<SmsApiSettings> _smsApiSettings;
        private readonly ILogService _logService;
        private readonly IEmployeeGroupService _employeeGroupService;
        private readonly IAdministratorService _administratorService;
        private readonly IGroupService _groupService;
        private readonly ISendSMSContext _smsContext;
        private readonly IApiSettingsService _apiSettingsService;

        public SmsApiController(IOptions<ServerSmsSettings> serverSmsSettings,
                                IOptions<SmsApiSettings> smsApiSettings,
                                ILogService logService,
                                IEmployeeGroupService employeeGroupService,
                                IAdministratorService administratorService,
                                IGroupService groupService,
                                ISendSMSContext smsContext,
                                IApiSettingsService apiSettingsService)
        {
            _serverSmsSettings = serverSmsSettings;
            _smsApiSettings = smsApiSettings;
            _logService = logService;
            _employeeGroupService = employeeGroupService;
            _administratorService = administratorService;
            _groupService = groupService;
            _smsContext = smsContext;
            _apiSettingsService = apiSettingsService;
        }

        private async Task<IActionResult> SendSmsMessageThroughServerSMS(string chosenGroupName, Group chosenGroup, int chosenGroupId, string additionalInfo, string? additionalPhoneNumbers, int adminId, AdministratorDTO admin, string phoneNumbersString, string text, Dictionary<string, string> data, ApiSettings activeApiSettings)
        {
            _smsContext.SetSmsStrategy(new SendSmsTroughServerSms(_serverSmsSettings, _apiSettingsService));
            var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, data);

            var dataForSmsEntity = new Dictionary<string, string>
                {
                    {"details", "true"},
                    {"phone", phoneNumbersString },
                    {"text", text },
                    { "speed", activeApiSettings.FastChannel ? "1" : "0" },
                    { "test", activeApiSettings.TestMode ? "true" : "false" },
                    { "sender", $"{activeApiSettings.SenderName}" }
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
                ServerSmsSuccessResponse successResponse = JsonConvert.DeserializeObject<ServerSmsSuccessResponse>(response) ?? throw new Exception("Deserialization to ServerSmsSuccessResponse failed");

                var smsMessage = new SMSMessage
                {
                    ChosenGroupId = chosenGroupId,
                    AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                    AdditionalInformation = additionalInfo,
                    Settings = dataForSmsEntity,
                    ServerResponse = successResponse
                };

                await _logService.AddEntityToDatabaseAsync(new Log
                {
                    LogType = "Info",
                    LogSource = "ServerSms",
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
                    ServerSmsErrorResponse errorResponse = JsonConvert.DeserializeObject<ServerSmsErrorResponse>(response) ?? throw new Exception("Deserialization to ServerSmsErrorResponse failed");

                    var smsMessage = new SMSMessage
                    {
                        ChosenGroupId = chosenGroupId,
                        AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                        AdditionalInformation = additionalInfo,
                        Settings = dataForSmsEntity,
                        ServerResponse = errorResponse
                    };

                    await _logService.AddEntityToDatabaseAsync(new Log
                    {
                        LogType = "Błąd",
                        LogSource = "ServerSms",
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

        private async Task<IActionResult> SendSmsMessageThroughSmsApi(string chosenGroupName, Group chosenGroup, int chosenGroupId, string additionalInfo, string? additionalPhoneNumbers, int adminId, AdministratorDTO admin, string phoneNumbersString, string text, Dictionary<string, string> data, ApiSettings activeApiSettings)
        {
            _smsContext.SetSmsStrategy(new SendSmsTroughSmsApi(_smsApiSettings, _apiSettingsService));
            var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, data);

            var dataForSmsEntity = new Dictionary<string, string>
                {
                    { "to", phoneNumbersString },
                    { "message", text },
                    { "format", "json" },
                    { "from", $"{activeApiSettings.SenderName}" },
                    { "fast", activeApiSettings.FastChannel ? "1" : "0" },
                    { "test",  activeApiSettings.TestMode ? "true" : "false" },
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

                SmsApiSuccessResponse successResponse = JsonConvert.DeserializeObject<SmsApiSuccessResponse>(response) ?? throw new Exception("Deserialization to SmsApiSuccessResponse failed");

                var smsMessage = new SMSMessage
                {
                    ChosenGroupId = chosenGroupId,
                    AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                    AdditionalInformation = additionalInfo,
                    Settings = dataForSmsEntity,
                    ServerResponse = successResponse
                };

                await _logService.AddEntityToDatabaseAsync(new Log
                {
                    LogType = "Info",
                    LogSource = "SmsApi",
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

                return Json(new { Status = "success", Queued = successResponse.Details.Count(d => d.Status == "QUEUE"), Unsent = successResponse.Details.Count(d => d.Status != "QUEUE") });
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
                    SmsApiErrorResponse errorResponse = JsonConvert.DeserializeObject<SmsApiErrorResponse>(response) ?? throw new Exception("Deserialization to SmsApiErrorResponse failed");

                    var smsMessage = new SMSMessage
                    {
                        ChosenGroupId = chosenGroupId,
                        AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                        AdditionalInformation = additionalInfo,
                        Settings = dataForSmsEntity,
                        ServerResponse = errorResponse
                    };

                    await _logService.AddEntityToDatabaseAsync(new Log
                    {
                        LogType = "Błąd",
                        LogSource = "SmsApi",
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

                    return Json(new { Status = "failed", Code = errorResponse.ErrorCode, Message = errorResponse.ErrorMessage });
                }
                catch (JsonException)
                {
                    throw new Exception("Error deserializing objects: response structure doesn't fit the object structure.");
                }
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SendSmsMessage(string text, int chosenGroupId, string chosenGroupName, string additionalPhoneNumbers, string additionalInfo)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var activeApiSettings = await _apiSettingsService.GetActiveSettingsAsync();

            Group chosenGroup = new Group();

            if (chosenGroupId > 0)
            {
                chosenGroup = await _groupService.GetByIdAsync(chosenGroupId);
                chosenGroup.Members = await _employeeGroupService.GetAllEmployeesForGroupListAsync(chosenGroupId);
            }

            var groupPhoneNumbers = await _employeeGroupService.GetAllActiveEmployeesPhoneNumbersForGroupListAsync(chosenGroupId);

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

            if (activeApiSettings.ApiName == "ServerSms")
            {
                return await SendSmsMessageThroughServerSMS(chosenGroupName, chosenGroup, chosenGroupId, additionalInfo, additionalPhoneNumbers, adminId, admin, phoneNumbersString, text, data, activeApiSettings);
            }
            else if (activeApiSettings.ApiName == "SmsApi")
            {
                return await SendSmsMessageThroughSmsApi(chosenGroupName, chosenGroup, chosenGroupId, additionalInfo, additionalPhoneNumbers, adminId, admin, phoneNumbersString, text, data, activeApiSettings);
            }
            else
            {
                throw new Exception("Wrong API name was received when fetching active API settings");
            }
        }
    }
}
