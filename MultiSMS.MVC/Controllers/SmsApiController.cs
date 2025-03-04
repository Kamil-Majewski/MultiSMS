using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Extensions;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.MProfi;
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
        private readonly IApiSmsSenderService _smsSenderService;

        public SmsApiController(ILogService logService,
                                IEmployeeGroupService employeeGroupService,
                                IUserService administratorService,
                                IGroupService groupService,
                                ISendSMSContext smsContext,
                                IApiSettingsService apiSettingsService,
                                IApiSmsSenderService smsSenderService)
        {
            _logService = logService;
            _employeeGroupService = employeeGroupService;
            _userService = administratorService;
            _groupService = groupService;
            _smsContext = smsContext;
            _apiSettingsService = apiSettingsService;
            _smsSenderService = smsSenderService;
        }

        private async Task<object> HandleApiResponse(string response, Dictionary<string, object> parameters, Group chosenGroup, string additionalPhoneNumbers, string additionalInfo,
                                               int adminId, ApiSettings activeApiSettings)
        {
            var user = await _userService.GetManageUserDtoByIdAsync(adminId);

            switch (activeApiSettings.ApiSettingsId)
            {
                case 1:
                    return await HandleSmsResponse<ServerSmsSuccessResponse, ServerSmsErrorResponse>(response, parameters, chosenGroup, additionalPhoneNumbers,
                                                                                                     additionalInfo, user, logSource: "ServerSms",
                                                                                                     getSuccessData: sr => (sr.Success, sr.Queued, sr.Unsent),
                                                                                                     getErrorData: er => (er.Error.Code, er.Error.Message));
                case 2:
                    return await HandleSmsResponse<SmsApiSuccessResponse, SmsApiErrorResponse>(response, parameters, chosenGroup, additionalPhoneNumbers,
                                                                                                     additionalInfo, user, logSource: "SmsApi",
                                                                                                     getSuccessData: sr => (true, sr.Details.Count(d => d.Status == "QUEUE"), sr.Details.Count(d => d.Status != "QUEUE")),
                                                                                                     getErrorData: er => (er.ErrorCode, er.ErrorMessage));
                case 3:
                    return await HandleSmsResponse<MProfiSuccessResponse, MProfiErrorResponse>(response, parameters, chosenGroup, additionalPhoneNumbers,
                                                                                                     additionalInfo, user, logSource: "MProfi",
                                                                                                     getSuccessData: sr => (true, sr.Result.Where(r => r.Id != null).Count(),
                                                                                                     sr.Result.Where(r => r.ErrorCode != null && r.ErrorMessage != null).Count()),
                                                                                                     getErrorData: er => ("", er.Detail));
                default:
                    throw new Exception("Unknown API Id");
            }
        }

        private async Task<object> HandleSmsResponse<TSuccess, TError>(string response, Dictionary<string, object> parameters ,Group chosenGroup,
                                                                       string additionalPhoneNumbers, string additionalInfo, ManageUserDTO user,
                                                                       string logSource,
                                                                       Func<TSuccess, (bool success, int queued, int unsent)> getSuccessData,
                                                                       Func<TError, (string errorCode, string errorMessage)> getErrorData)
        {
            TSuccess? successResponse = default;
            TError? errorResponse = default;

            try
            {
                successResponse = JsonConvert.DeserializeObject<TSuccess>(response);
            }
            catch (JsonException)
            {
                // We'll try the error response below.
            }

            if (successResponse == null)
            {
                try
                {
                    errorResponse = JsonConvert.DeserializeObject<TError>(response);
                }
                catch (JsonException)
                {
                    throw new Exception("Error deserializing objects: response structure doesn't fit the expected object structure.");
                }
            }

            string logType;
            string logMessage;
            object result;

            if (successResponse != null)
            {
                logType = "Info";
                logMessage = string.IsNullOrEmpty(chosenGroup.GroupName) && chosenGroup.GroupId == 0
                    ? $"{logSource}: Sms został wysłany do pojedyńczych numerów"
                    : $"{logSource}: Sms został wysłany do grupy {chosenGroup.GroupName}";

                var (success, queued, unsent) = getSuccessData(successResponse);
                result = (ValueTuple<bool, int, int>)(success, queued, unsent);
            }
            else
            {
                logType = "Błąd";
                logMessage = string.IsNullOrEmpty(chosenGroup.GroupName) && chosenGroup.GroupId == 0
                    ? $"{logSource}: Wystąpił błąd podczas wysyłania Sms do pojedyńczych numerów"
                    : $"{logSource}: Wystąpił błąd podczas wysyłania Sms do grupy {chosenGroup.GroupName}";

                var (errorCode, errorMessage) = getErrorData(errorResponse!);
                result = (ValueTuple<string, string, string>)("failed", errorCode, errorMessage);
            }

            // Build the SMS message.
            var smsMessage = new SMSMessage
            {
                ChosenGroupId = chosenGroup.GroupId,
                AdditionalPhoneNumbers = additionalPhoneNumbers,
                AdditionalInformation = additionalInfo,
                Settings = parameters,
                ServerResponse = successResponse != null ? (object)successResponse : errorResponse!
            };

            // Remove vulnerable data from logs
            smsMessage.Settings.Remove("apikey");

            // Log the event.
            var log = new Log
            {
                LogType = logType,
                LogSource = logSource,
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

            return result;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SendSmsMessage(string text, int chosenGroupId, string additionalPhoneNumbers, string additionalInfo)
        {
            var userId = User.GetLoggedInUserId<int>();
            var activeApiSettings = await _apiSettingsService.GetActiveSettingsAsync();
            var sender = await _smsSenderService.GetSenderByUserId(userId);

            Group chosenGroup = new();
            List<string> groupPhoneNumbers = new();

            if (chosenGroupId > 0)
            {
                chosenGroup = await _groupService.GetByIdAsync(chosenGroupId);
                chosenGroup.Members = await _employeeGroupService.GetAllEmployeesForGroupListAsync(chosenGroupId);
                groupPhoneNumbers = await _employeeGroupService.GetAllActiveEmployeesPhoneNumbersForGroupListAsync(chosenGroupId);
            }            

            if (!string.IsNullOrEmpty(additionalPhoneNumbers))
            {
                var additionalNumbers = additionalPhoneNumbers
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(num => num.Trim())
                    .Where(num => !string.IsNullOrWhiteSpace(num))
                    .ToList();

                groupPhoneNumbers.AddRange(additionalNumbers);
            }

            if (groupPhoneNumbers.Count <= 200)
            {
                var phoneNumbersString = string.Join(',', groupPhoneNumbers);
                var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, sender);

                var result = await HandleApiResponse(response.ResponseContent, response.Parameters, chosenGroup,
                                               additionalPhoneNumbers, additionalInfo, userId, activeApiSettings);

                if (result is ValueTuple<bool, int, int> successTuple)
                {
                    return Json(new { Status = successTuple.Item1, Queued = successTuple.Item2, Unsent = successTuple.Item3 });
                }
                else if (result is ValueTuple<string, string, string> failedTuple)
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
                    var batch = groupPhoneNumbers.Skip(i).Take(200);
                    var phoneNumbersString = string.Join(',', batch);

                    var response = await _smsContext.SendSMSAsync(phoneNumbersString, text, sender);

                    var result = await HandleApiResponse(response.ResponseContent, response.Parameters, chosenGroup,
                                               additionalPhoneNumbers, additionalInfo, userId, activeApiSettings);

                    if (result is ValueTuple<bool, int, int> successTuple)
                    {
                        queued += successTuple.Item2;
                        unsent += successTuple.Item3;
                    }
                    else if (result is ValueTuple<string, string, string> failedTuple)
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
    }
}
