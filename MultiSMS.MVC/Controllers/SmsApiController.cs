using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Extensions;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.ServerSms;
using MultiSMS.Interface.Entities.SmsApi;
using Newtonsoft.Json;

namespace MultiSMS.MVC.Controllers
{
    public class SmsApiController : Controller
    {
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
            _logService = logService;
            _employeeGroupService = employeeGroupService;
            _userService = administratorService;
            _groupService = groupService;
            _smsContext = smsContext;
            _apiSettingsService = apiSettingsService;
        }

        private async Task<object> HandleServerSmsResponse(string response, Dictionary<string, string> parameters, Group chosenGroup,
                                                           string additionalPhoneNumbers, string additionalInfo, ManageUserDTO user)
        {
            ServerSmsSuccessResponse? successResponse = null;
            ServerSmsErrorResponse? errorResponse = null;

            try
            {
                successResponse = JsonConvert.DeserializeObject<ServerSmsSuccessResponse>(response);
            }
            catch (JsonException)
            {
                // Ignore; we'll try errorResponse below.
            }

            // If successResponse is null, try deserializing as error response.
            if (successResponse == null)
            {
                try
                {
                    errorResponse = JsonConvert.DeserializeObject<ServerSmsErrorResponse>(response);
                }
                catch (JsonException)
                {
                    throw new Exception("Error deserializing objects: response structure doesn't fit the expected object structure.");
                }
            }

            string logType;
            string logMessage;
            bool? success = null;
            int? queued = null;
            int? unsent = null;
            int? errorCode = null;
            string? errorMessage = null;

            if (successResponse != null)
            {
                logType = "Info";
                logMessage = string.IsNullOrEmpty(chosenGroup.GroupName) && chosenGroup.GroupId == 0
                    ? "Sms został wysłany do pojedyńczych numerów"
                    : $"Sms został wysłany do grupy {chosenGroup.GroupName}";

                success = successResponse.Success;
                queued = successResponse.Queued;
                unsent = successResponse.Unsent;
            }
            else // errorResponse is not null
            {
                logType = "Błąd";
                logMessage = string.IsNullOrEmpty(chosenGroup.GroupName) && chosenGroup.GroupId == 0
                    ? "Wystąpił błąd podczas wysyłania Sms do pojedyńczych numerów"
                    : $"Wystąpił błąd podczas wysyłania Sms do grupy {chosenGroup.GroupName}";

                errorCode = errorResponse!.Error.Code;
                errorMessage = errorResponse.Error.Message;
            }

            // Build the SMS message
            var smsMessage = new SMSMessage
            {
                ChosenGroupId = chosenGroup.GroupId,
                AdditionalPhoneNumbers = additionalPhoneNumbers,
                AdditionalInformation = additionalInfo,
                Settings = parameters,
                ServerResponse = successResponse == null ? errorResponse! : successResponse
            };

            // Log the event.
            var log = new Log
            {
                LogType = logType,
                LogSource = "ServerSms",
                LogMessage = logMessage,
                LogCreator = user.UserName,
                LogCreatorId = user.Id,
                LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "SmsMessages", JsonConvert.SerializeObject(smsMessage) },
                    { "Groups", JsonConvert.SerializeObject(chosenGroup) },
                    { "Creator", JsonConvert.SerializeObject(user) }
                })
            };

            await _logService.AddEntityToDatabaseAsync(log);

            // Return a tuple – might want to standardize this later.
            return success != null && queued != null && unsent != null
                ? (success.Value, queued.Value, unsent.Value)
                : ("failed", errorCode, errorMessage);
        }

        private async Task<object> HandleSmsApiResponse(string response, Dictionary<string, string> parameters, Group chosenGroup, string additionalPhoneNumbers,
                                                        string additionalInfo, ManageUserDTO user)
        {
            SmsApiSuccessResponse? successResponse = null;
            SmsApiErrorResponse? errorResponse = null;

            try
            {
                successResponse = JsonConvert.DeserializeObject<SmsApiSuccessResponse>(response);
            }
            catch (JsonException)
            {
                // Ignore
            }

            if(successResponse == null)
            {
                try
                {
                    errorResponse = JsonConvert.DeserializeObject<SmsApiErrorResponse>(response);
                }
                catch
                {
                    throw new Exception("Error deserializing objects: response structure doesn't fit the expected object structure.");
                }
            }

            string logType;
            string logMessage;
            int? queued = null;
            int? unsent = null;
            int? errorCode = null;
            string? errorMessage = null;

            if (successResponse != null)
            {
                logType = "Info";
                logMessage = string.IsNullOrEmpty(chosenGroup.GroupName) && chosenGroup.GroupId == 0
                    ? "Sms został wysłany do pojedyńczych numerów"
                    : $"Sms został wysłany do grupy {chosenGroup.GroupName}";

                queued = successResponse.Details.Count(d => d.Status == "QUEUE");
                unsent = successResponse.Details.Count(d => d.Status != "QUEUE");
            }
            else
            {
                logType = "Błąd";
                logMessage = string.IsNullOrEmpty(chosenGroup.GroupName) && chosenGroup.GroupId == 0
                    ? "Wystąpił błąd podczas wysyłania Sms do pojedyńczych numerów"
                    : $"Wystąpił błąd podczas wysyłania Sms do grupy {chosenGroup.GroupName}";

                errorCode = errorResponse!.ErrorCode;
                errorMessage = errorResponse.ErrorMessage;
            }

            var smsMessage = new SMSMessage
            {
                ChosenGroupId = chosenGroup.GroupId,
                AdditionalPhoneNumbers = additionalPhoneNumbers,
                AdditionalInformation = additionalInfo,
                Settings = parameters,
                ServerResponse = successResponse == null ? errorResponse! : successResponse
            };

            var log = new Log
            {
                LogType = logType,
                LogSource = "SmsApi",
                LogMessage = logMessage,
                LogCreator = user.UserName,
                LogCreatorId = user.Id,
                LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "SmsMessages", JsonConvert.SerializeObject(smsMessage) },
                    { "Groups", JsonConvert.SerializeObject(chosenGroup) },
                    { "Creator", JsonConvert.SerializeObject(user) }
                })
            };

            await _logService.AddEntityToDatabaseAsync(log);

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
                case 3:
                    //return HandleMProfiApiResponse(response, parameters, chosenGroup, additionalPhoneNumbers, additionalInfo, user);
                    throw new NotImplementedException();
                default:
                    throw new Exception("Unknown API Id");
            }
        }
    }
}
