using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Extensions;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.BusinessLogic.Strategy;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.ServerSms;
using MultiSMS.Interface.Entities.SmsApi;
using Newtonsoft.Json;

namespace MultiSMS.MVC.Controllers
{
    public class SmsApiController : Controller
    {
        private readonly IOptions<ServerSmsSettings> _serverSmsSettings;
        private readonly IOptions<SmsApiSettings> _smsApiSettings;
        private readonly ILogService _logService;
        private readonly IEmployeeGroupService _employeeGroupService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly ISendSMSContext _smsContext;
        private readonly IApiSettingsService _apiSettingsService;

        public SmsApiController(IOptions<ServerSmsSettings> serverSmsSettings,
                                IOptions<SmsApiSettings> smsApiSettings,
                                ILogService logService,
                                IEmployeeGroupService employeeGroupService,
                                IUserService administratorService,
                                IGroupService groupService,
                                ISendSMSContext smsContext,
                                IApiSettingsService apiSettingsService)
        {
            _serverSmsSettings = serverSmsSettings;
            _smsApiSettings = smsApiSettings;
            _logService = logService;
            _employeeGroupService = employeeGroupService;
            _userService = administratorService;
            _groupService = groupService;
            _smsContext = smsContext;
            _apiSettingsService = apiSettingsService;
        }

        private async Task<object> SendSmsMessageThroughServerSMS(string chosenGroupName, Group chosenGroup, int chosenGroupId, string additionalInfo, string? additionalPhoneNumbers, int adminId, UserDTO admin, string phoneNumbersString, string text, Dictionary<string, string> data, ApiSettings activeApiSettings)
        {
            _smsContext.SetSmsStrategy(new SendSmsTroughServerSms(_serverSmsSettings, _apiSettingsService));
            var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, data);

            string logMessage = (chosenGroupName == null && chosenGroupId == 0)
                ? "Sms został wysłany do pojedyńczych numerów"
                : $"Sms został wysłany do grupy {chosenGroupName}";

            try //try to deserialize response into entities that correspond with response structure and then act depending on the outcome
            {
                ServerSmsSuccessResponse successResponse = JsonConvert.DeserializeObject<ServerSmsSuccessResponse>(response) ?? throw new Exception("Deserialization to ServerSmsSuccessResponse failed");

                var smsMessage = new SMSMessage
                {
                    ChosenGroupId = chosenGroupId,
                    AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                    AdditionalInformation = additionalInfo,
                    Settings = data,
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

                return (successResponse.Success, successResponse.Queued, successResponse.Unsent);
            }
            catch (JsonException)
            {
                try //deserialization into SuccessResponse failed, try to deserialize into ErrorResponse
                {
                    ServerSmsErrorResponse errorResponse = JsonConvert.DeserializeObject<ServerSmsErrorResponse>(response) ?? throw new Exception("Deserialization to ServerSmsErrorResponse failed");

                    var smsMessage = new SMSMessage
                    {
                        ChosenGroupId = chosenGroupId,
                        AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                        AdditionalInformation = additionalInfo,
                        Settings = data,
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

                    return ("failed", errorResponse.Error.Code, errorResponse.Error.Message);
                }
                catch (JsonException)
                {
                    throw new Exception("Error deserializing objects: response structure doesn't fit the expected object structure.");
                }
            }
        }

        private async Task<object> SendSmsMessageThroughSmsApi(string chosenGroupName, Group chosenGroup, int chosenGroupId, string additionalInfo, string? additionalPhoneNumbers, int adminId, UserDTO admin, string phoneNumbersString, string text, Dictionary<string, string> data, ApiSettings activeApiSettings)
        {
            _smsContext.SetSmsStrategy(new SendSmsTroughSmsApi(_smsApiSettings, _apiSettingsService));
            var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, data);

            string logMessage = (chosenGroupName == null && chosenGroupId == 0)
                ? "Sms został wysłany do pojedyńczych numerów"
                : $"Sms został wysłany do grupy {chosenGroupName}";

            try //try to deserialize response into entities that correspond with response structure and then act depending on the outcome
            {
                SmsApiSuccessResponse successResponse = JsonConvert.DeserializeObject<SmsApiSuccessResponse>(response) ?? throw new Exception("Deserialization to SmsApiSuccessResponse failed");

                var smsMessage = new SMSMessage
                {
                    ChosenGroupId = chosenGroupId,
                    AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                    AdditionalInformation = additionalInfo,
                    Settings = data,
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

                var queued = successResponse.Details.Count(d => d.Status == "QUEUE");
                var unsent = successResponse.Details.Count(d => d.Status != "QUEUE");

                return (true, queued, unsent);
            }
            catch (JsonException)
            {
                try //deserialization into SuccessResponse failed, try to deserialize into ErrorResponse
                {
                    SmsApiErrorResponse errorResponse = JsonConvert.DeserializeObject<SmsApiErrorResponse>(response) ?? throw new Exception("Deserialization to SmsApiErrorResponse failed");

                    var smsMessage = new SMSMessage
                    {
                        ChosenGroupId = chosenGroupId,
                        AdditionalPhoneNumbers = string.Join(',', additionalPhoneNumbers),
                        AdditionalInformation = additionalInfo,
                        Settings = data,
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
                    { "Creator", JsonConvert.SerializeObject(user) }
                            })
                    });

                    return ("failed", errorResponse.ErrorCode, errorResponse.ErrorMessage);

            return queued != null && unsent != null
                ? (true, queued.Value, unsent.Value)
                : ("failed", errorCode, errorMessage);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SendSmsMessage(string text, int chosenGroupId, string senderName, string additionalPhoneNumbers, string additionalInfo)
        {
            var userId = User.GetLoggedInUserId<int>();

            var activeApiSettings = await _apiSettingsService.GetActiveSettingsAsync();

            Group chosenGroup = new();

            if (chosenGroupId > 0)
            {
                chosenGroup = await _groupService.GetByIdAsync(chosenGroupId);
                chosenGroup.Members = await _employeeGroupService.GetAllEmployeesForGroupListAsync(chosenGroupId);
            }

            var groupPhoneNumbers = await _employeeGroupService.GetAllActiveEmployeesPhoneNumbersForGroupListAsync(chosenGroupId);

            if (!string.IsNullOrEmpty(additionalPhoneNumbers))
            {
                var additionalNumbers = additionalPhoneNumbers.Split(',').ToList();
                groupPhoneNumbers.AddRange(additionalNumbers);
            }

            if (groupPhoneNumbers.Count < 200)
            {
                var phoneNumbersString = string.Join(',', groupPhoneNumbers);
                var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, senderName);

                var result = await HandleApiResponse(response.ResponseContent, response.Parameters, chosenGroup,
                                               additionalPhoneNumbers, additionalInfo, userId, activeApiSettings);

                if (result is ValueTuple<bool, int, int> successTuple)
                {
                    return Json(new { Status = successTuple.Item1, Queued = successTuple.Item2, Unsent = successTuple.Item3 });
                }
                else if (result is ValueTuple<string, int, string> failedTuple)
                {
                    return Json(new { Status = failedTuple.Item1, Code = failedTuple.Item2, Message = failedTuple.Item3 });
                }
                else
                {
                    throw new Exception("Unknown object type");
                }
            }
            else
            {
                var queued = 0;
                var unsent = 0;
                var listOfErrors = new List<string>();

                for (int i = 0; i < groupPhoneNumbers.Count; i += 200)
                {
                    var data = new Dictionary<string, string>();

                    var chunk = groupPhoneNumbers.GetRange(i, Math.Min(200, groupPhoneNumbers.Count - i));
                    var phoneNumbersString = string.Join(',', chunk);

                    var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, senderName);

                    var result = await HandleApiResponse(response.ResponseContent, response.Parameters, chosenGroup,
                                               additionalPhoneNumbers, additionalInfo, userId, activeApiSettings);

                    if (result is ValueTuple<bool, int, int> successTuple)
                    {
                        queued += successTuple.Item2;
                        unsent += successTuple.Item3;
                    }
                    else if (result is ValueTuple<string, int, string> failedTuple)
                    {
                        listOfErrors.Add(failedTuple.Item3);
                    }
                    else
                    {
                        throw new Exception("Unknown object type");
                    }
                }

                if (queued == 0 && unsent == 0)
                {
                    return Json(new { Status = "Multiple-Failure", Errors = listOfErrors.Distinct() });
                }
                else if (listOfErrors.Count == 0)
                {
                    return Json(new { Status = "Multiple-Success", Queued = queued, Unsent = unsent });
                }
                else
                {
                    return Json(new { Status = "Multiple-Partial", Queued = queued, Unsent = unsent, Errors = listOfErrors.Distinct() });
                }
            }
        }

        private async Task<object> HandleApiResponse(string response, Dictionary<string, string> parameters, Group chosenGroup, string additionalPhoneNumbers, string additionalInfo,
                                               int adminId, ApiSettings activeApiSettings)
        {
            var user = await _userService.GetIdentityUserById(adminId);

            switch (activeApiSettings.ApiSettingsId)
            {
                case 1:
                    return HandleServerSmsResponse(response, parameters, chosenGroup, additionalPhoneNumbers, additionalInfo, user);
                case 2:
                    return HandleSmsApiResponse(response, parameters, chosenGroup, additionalPhoneNumbers, additionalInfo, user);
                default:
                    throw new Exception("Unknown API Id");
            }
        }
    }
}
