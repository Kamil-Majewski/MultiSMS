using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Extensions;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.ServerSms;
using MultiSMS.Interface.Entities.SmsApi;
using MultiSMS.MVC.Models;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Diagnostics;

namespace MultiSMS.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdministratorService _administratorService;
        private readonly IImportExportEmployeesService _ieService;
        private readonly ISMSMessageTemplateService _smsTemplateService;
        private readonly IEmployeeService _employeeService;
        private readonly IGroupService _groupService;
        private readonly IEmployeeGroupService _employeeGroupService;
        private readonly ILogService _logService;
        private readonly IApiSettingsService _apiSettingsService;
        public HomeController(ISMSMessageTemplateService smsTemplateRepository, IEmployeeService employeeRepository, IGroupService groupRepository, IEmployeeGroupService employeeGroupRepository, ILogService logRepository, IAdministratorService administratorService, IImportExportEmployeesService ieService, IApiSettingsService apiSettingsService)
        {
            _smsTemplateService = smsTemplateRepository;
            _employeeService = employeeRepository;
            _groupService = groupRepository;
            _employeeGroupService = employeeGroupRepository;
            _logService = logRepository;
            _administratorService = administratorService;
            _ieService = ieService;
            _apiSettingsService = apiSettingsService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Templates

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateNewTemplate(string templateName, string templateDescription, string templateContent)
        {

            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var template = new SMSMessageTemplate() { TemplateName = templateName, TemplateDescription = templateDescription, TemplateContent = templateContent };
            await _smsTemplateService.AddEntityToDatabaseAsync(template);

            await _logService.AddEntityToDatabaseAsync(
                new Log
                {
                    LogType = "Info",
                    LogSource = "Szablony",
                    LogMessage = $"Szablon {template.TemplateName} został utworzony",
                    LogCreator = admin.UserName,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"Templates", JsonConvert.SerializeObject(template) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                    })
                }
            );

            return Json(template);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllTemplates()
        {
            var templates = await Task.FromResult(_smsTemplateService.GetAllEntries());
            return Json(templates);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaginateTemplates(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            var (paginatedTemplates, hasMorePages) = await _smsTemplateService.PaginateTemplateDataAsync(firstId, lastId, pageSize, moveForward);
            return Json(new {paginatedTemplates, hasMorePages});
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            var template = await _smsTemplateService.GetByIdAsync(id);
            return Json(template);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditTemplate(int id, string name, string description, string content)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var template = new SMSMessageTemplate { TemplateId = id, TemplateName = name, TemplateDescription = description, TemplateContent = content };

            var editedTemplate = await _smsTemplateService.UpdateEntityAsync(template);

            await _logService.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Szablony",
                     LogMessage = $"Szablon {editedTemplate.TemplateName} został zedytowany",
                     LogCreator = admin.UserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                     {
                        {"Templates", JsonConvert.SerializeObject(editedTemplate) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                     })

                 }
             );

            return Ok("Successfully edited template");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var template = await _smsTemplateService.GetByIdAsync(id);
            await _logService.AddEntityToDatabaseAsync(
                new Log
                {
                    LogType = "Info",
                    LogSource = "Szablony",
                    LogMessage = $"Szablon {template.TemplateName} został usunięty",
                    LogCreator = admin.UserName,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                         {"Templates", JsonConvert.SerializeObject(template) },
                         {"Creator", JsonConvert.SerializeObject(admin) }
                    })

                }
            );

            await _smsTemplateService.DeleteEntityAsync(id);
            return Ok("Successfully deleted template");
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTemplatesBySeachPhrase(string searchPhrase)
        {
            return Json(await _smsTemplateService.GetTemplatesBySearchPhraseAsync(searchPhrase));
        }

            #endregion

        #region Employees
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateNewContact(string contactName, string contactSurname, string email, string phone, string address, string zip, string city, string department, string isActive)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            bool activeValue = isActive == "yes" ? true : false;

            var contact = new Employee
            {
                Name = contactName,
                Surname = contactSurname,
                Email = email,
                PhoneNumber = phone,
                HQAddress = address,
                PostalNumber = zip,
                City = city,
                Department = department,
                IsActive = activeValue,
            };
            await _employeeService.AddEntityToDatabaseAsync(contact);

            await _logService.AddEntityToDatabaseAsync(
                  new Log
                  {
                      LogType = "Info",
                      LogSource = "Kontakty",
                      LogMessage = $"Kontakt {contact.Name} {contact.Surname} został utworzony",
                      LogCreator = admin.UserName,
                      LogCreatorId = adminId,
                      LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                      {
                        {"Employees", JsonConvert.SerializeObject(contact) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                      })

                  }
              );

            return Json(contact);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllContacts()
        {
            var contacts = await Task.FromResult(_employeeService.GetAllEntries());
            foreach (var contact in contacts)
            {
                contact.EmployeeGroupNames = await _employeeGroupService.GetAllGroupNamesForEmployeeListAsync(contact.EmployeeId);
            }
            return Json(contacts);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaginateContacts(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            var (paginatedContacts, hasMorePages) = await _employeeService.PaginateEmployeeDataAsync(firstId,lastId, pageSize, moveForward);
            foreach (var contact in paginatedContacts)
            {
                contact.EmployeeGroupNames = await _employeeGroupService.GetAllGroupNamesForEmployeeListAsync(contact.EmployeeId);
            }
            return Json(new { paginatedContacts, hasMorePages });
        }

        

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _employeeService.GetByIdAsync(id);
            contact.EmployeeGroupNames = await _employeeGroupService.GetAllGroupNamesForEmployeeListAsync(id);
            return Json(contact);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditContact(int contactId, string contactName, string contactSurname, string email, string phone, string address, string zip, string city, string department, string isActive)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            bool activeValue = isActive == "yes" ? true : false;

            var contact = new Employee
            {
                EmployeeId = contactId,
                Name = contactName,
                Surname = contactSurname,
                Email = email,
                PhoneNumber = phone,
                HQAddress = address,
                PostalNumber = zip,
                City = city,
                Department = department,
                IsActive = activeValue,
            };

            var editedContact = await _employeeService.UpdateEntityAsync(contact);
            await _logService.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Kontakty",
                     LogMessage = $"Kontakt {editedContact.Name} {editedContact.Surname} został zedytowany",
                     LogCreator = admin.UserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                     {
                        {"Employees", JsonConvert.SerializeObject(editedContact) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                     })

                 }
             );

            return Ok("Successfully edited contact");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var contact = await _employeeService.GetByIdAsync(id);
            await _logService.AddEntityToDatabaseAsync(
               new Log
               {
                   LogType = "Info",
                   LogSource = "Kontakty",
                   LogMessage = $"Kontakt {contact.Name} {contact.Surname} został usunięty",
                   LogCreator = admin.UserName,
                   LogCreatorId = adminId,
                   LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                   {
                         {"Employees", JsonConvert.SerializeObject(contact) },
                         {"Creator", JsonConvert.SerializeObject(admin) }
                   })
               }
           );
            await _employeeService.DeleteEntityAsync(id);

            return Ok("Successfully deleted contact");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetContactsBySearchPhrase(string searchPhrase)
        {
            var contacts = await Task.FromResult(_employeeService.GetAllEntries());

            var dictionaryEmployeeIdGroupNames = _employeeGroupService.GetDictionaryOfEmployeeIdAndGroupNames();

            var filtereContacts = contacts.Where(e =>
            e.Name.ToLower().Contains(searchPhrase) ||
            e.Surname.ToLower().Contains(searchPhrase) ||
            (e.Email == null || e.Email.Equals(string.Empty) ? "Brak danych" : e.Email!).ToLower().Contains(searchPhrase) ||
            e.PhoneNumber.ToLower().Contains(searchPhrase) ||
            (e.IsActive ? "Aktywny" : "Nieaktywny").ToLower().Contains(searchPhrase) ||
            dictionaryEmployeeIdGroupNames[e.EmployeeId].Any(g => g.ToLower().Contains(searchPhrase))).ToList();

            foreach(var contact in filtereContacts)
            {
                contact.EmployeeGroupNames = dictionaryEmployeeIdGroupNames[contact.EmployeeId].ToList();
            }

            return Json(filtereContacts);

        }
        #endregion

        #region Groups

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateNewGroup(string groupName, string groupDescription)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var group = new Group() { GroupName = groupName, GroupDescription = groupDescription };
            await _groupService.AddEntityToDatabaseAsync(group);

            await _logService.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Grupy",
                     LogMessage = $"Grupa {group.GroupName} została utworzona",
                     LogCreator = admin.UserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                     {
                        {"Groups", JsonConvert.SerializeObject(group) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                     })

                 }
             );

            return Json(group);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllGroups()
        {
            var groups = await Task.FromResult(_groupService.GetAllEntries().ToList());
            foreach (var group in groups)
            {
                group.MembersIds = await _employeeGroupService.GetAllEmployeesIdsForGroupListAsync(group.GroupId);
            }
            return Json(groups);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaginateGroups(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            var (paginatedGroups, hasMorePages) = await _groupService.PaginateGroupDataAsync(firstId, lastId, pageSize, moveForward);
            foreach(var group in paginatedGroups)
            {
                group.MembersIds = await _employeeGroupService.GetAllEmployeesIdsForGroupListAsync(group.GroupId);
            };
            return Json(new {paginatedGroups, hasMorePages});
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllValidGroups()
        {
            var groups = await Task.FromResult(_groupService.GetAllGroupsWithGroupMembersList());
            int amountOfInactives = 0;
            List<Group> validGroups = new List<Group>();
            foreach (var group in groups)
            {
                group.MembersIds = await _employeeGroupService.GetAllEmployeesIdsForGroupListAsync(group.GroupId);

                if (group.GroupMembers != null)
                {
                    foreach (var member in group.GroupMembers)
                    {
                        if (member.Employee.IsActive == false)
                        {
                            amountOfInactives += 1;
                        }
                    }

                    if (amountOfInactives != group.GroupMembers.Count())
                    {
                        group.GroupMembers = null;
                        validGroups.Add(group);
                        amountOfInactives = 0;
                    }
                }

            }

            return Json(validGroups);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var group = await _groupService.GetByIdAsync(id);
            group.MembersIds = await _employeeGroupService.GetAllEmployeesIdsForGroupListAsync(group.GroupId);
            return Json(group);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditGroup(int id, string name, string description)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var group = new Group { GroupId = id, GroupName = name, GroupDescription = description };
            group.Members = await _employeeGroupService.GetAllEmployeesForGroupListAsync(id);

            var editedGroup = await _groupService.UpdateEntityAsync(group);
            await _logService.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Grupy",
                     LogMessage = $"Grupa {editedGroup.GroupName} została zedytowana",
                     LogCreator = admin.UserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                     {
                         {"Groups", JsonConvert.SerializeObject(editedGroup) },
                         {"Creator", JsonConvert.SerializeObject(admin) }
                     })
                 }
             );

            return Ok("Successfully edited group");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var group = await _groupService.GetByIdAsync(id);
            group.Members = await _employeeGroupService.GetAllEmployeesForGroupListAsync(id);
            await _logService.AddEntityToDatabaseAsync(
               new Log
               {
                   LogType = "Info",
                   LogSource = "Grupy",
                   LogMessage = $"Grupa {group.GroupName} została usunięta",
                   LogCreator = admin.UserName,
                   LogCreatorId = adminId,
                   LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                   {
                         {"Groups", JsonConvert.SerializeObject(group) },
                         {"Creator", JsonConvert.SerializeObject(admin) }
                   })
               }
           );
            await _groupService.DeleteEntityAsync(id);
            return Ok("Successfully deleted group");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGroupsBySearchPhrase(string searchPhrase)
        {
            var filteredGroups = await _groupService.GetGroupsBySearchPhraseAsync(searchPhrase);

            foreach (var group in filteredGroups)
            {
                group.MembersIds = await _employeeGroupService.GetAllEmployeesIdsForGroupListAsync(group.GroupId);
            }

            return Json(filteredGroups);
        }

        #endregion

        #region EmployeeGroups

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddUserToGroup(int groupId, int employeeId)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var employee = await _employeeService.GetByIdAsync(employeeId);
            var group = await _groupService.GetByIdAsync(groupId);

            await _employeeGroupService.AddGroupMemberAsync(groupId, employeeId);

            await _logService.AddEntityToDatabaseAsync(
                new Log
                {
                    LogType = "Info",
                    LogSource = "Grupy",
                    LogMessage = $"{employee.Name} {employee.Surname} został dodany do grupy {group.GroupName}",
                    LogCreatorId = adminId,
                    LogCreator = admin.UserName,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"Employees", JsonConvert.SerializeObject(employee) },
                        {"Groups", JsonConvert.SerializeObject(new {GroupId = group.GroupId, GroupName = group.GroupName, GroupDescription = group.GroupDescription}) },
                        {"Creator", JsonConvert.SerializeObject(admin) }

                    })
                }
            );
            return Ok("Successfully added contact to group");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemoveUserFromGroup(int groupId, int employeeId)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var employee = await _employeeService.GetByIdAsync(employeeId);
            var group = await _groupService.GetByIdAsync(groupId);

            await _employeeGroupService.RemoveGroupMember(groupId, employeeId);

            await _logService.AddEntityToDatabaseAsync(
                new Log
                {
                    LogType = "Info",
                    LogSource = "Grupy",
                    LogMessage = $"{employee.Name} {employee.Surname} został usunięty z grupy {group.GroupName}",
                    LogCreatorId = adminId,
                    LogCreator = admin.UserName,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"Employees", JsonConvert.SerializeObject(employee) },
                        {"Groups", JsonConvert.SerializeObject(group) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                    })
                }
            );
            return Ok("Successfully removed user from group");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeesForGroup(int groupId)
        {
            return Json(await _employeeGroupService.GetAllEmployeesForGroupListAsync(groupId));
        }

        #endregion

        #region Import-Export

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ImportContacts(IFormFile file)
        {

            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            ImportResult importResultDto = await _ieService.ImportContactsAsync(file);

            var status = importResultDto.ImportStatus;
            string logMessage;
            string logType;
            string logRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                     {
                        {"Imports", JsonConvert.SerializeObject(importResultDto) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                     });

            if (status == "Success")
            {

                logMessage = $"Zaimportowano nowe kontakty ({importResultDto.AddedEmployees!.Count()}) i przypisano do grup.";
                logType = "Info";


            }
            else if (status == "Partial Success")
            {
                logMessage = $"Zaimportowano nowe kontakty ({importResultDto.AddedEmployees!.Count()}), nie wszystkie grupy przypisano.";
                logType = "Info";
            }
            else
            {
                logMessage = $"Nie zaimportowano kontaktów ({importResultDto.ImportMessage})";
                logType = "Błąd";
            }

            await _logService.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = logType,
                     LogSource = "Import",
                     LogMessage = logMessage,
                     LogCreator = admin.UserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = logRelatedObjectsDictionarySerialized
                 }
             );

            return Json(new { Status = importResultDto.ImportStatus, Message = importResultDto.ImportMessage, AddedEmployees = importResultDto.AddedEmployees, RepeatedEmployees = importResultDto.RepeatedEmployees, InvalidEmployees = importResultDto.InvalidEmployees, NonExistantGroupIds = importResultDto.NonExistantGroupIds });
        }


        [Authorize]
        public IActionResult DownloadExcelWithContacts()
        {
            var fileDirectory = _ieService.ExportContactsExcel();

            if (System.IO.File.Exists(fileDirectory))
            {
                try
                {
                    var fileBytes = System.IO.File.ReadAllBytes(fileDirectory);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                return NotFound("Excel file not found");
            }
        }

        #endregion

        #region Logs

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetLog(int logId)
        {
            Dictionary<string, string> logRelatedObjects;

            var log = await _logService.GetByIdAsync(logId);
            var logSanitized = new { LogType = log.LogType, LogSource = log.LogSource, LogMessage = log.LogMessage, LogCreationDate = log.LogCreationDate };

            if (log.LogRelatedObjectsDictionarySerialized == null)
            {
                return StatusCode(500, "NullReference: Log related objects is null");
            }
            else
            {
                logRelatedObjects = JsonConvert.DeserializeObject<Dictionary<string, string>>(log.LogRelatedObjectsDictionarySerialized)!;
            }

            var logCreator = JsonConvert.DeserializeObject<AdministratorDTO>(logRelatedObjects["Creator"]);

            switch (log.LogSource)
            {
                case "Szablony":
                    var template = JsonConvert.DeserializeObject<SMSMessageTemplate>(logRelatedObjects["Templates"]);
                    return Json(new { Type = "Template", Template = template, Log = logSanitized, LogCreator = logCreator });
                case "Kontakty":
                    var employee = JsonConvert.DeserializeObject<Employee>(logRelatedObjects["Employees"]);
                    return Json(new { Type = "Contact", Contact = employee, Log = logSanitized, LogCreator = logCreator });
                case "Grupy":
                    var group = JsonConvert.DeserializeObject<Group>(logRelatedObjects["Groups"]);
                    try
                    {
                        var addedEmployee = JsonConvert.DeserializeObject<Employee>(logRelatedObjects["Employees"]);
                        return Json(new { Type = "Groups-Assign", Group = group, Contact = addedEmployee, Log = logSanitized, LogCreator = logCreator });
                    }
                    catch (Exception)
                    {
                        return Json(new { Type = "Groups", Group = group, Log = logSanitized, LogCreator = logCreator });
                    }
                case "ServerSms":
                    var serverSmsDto = JsonConvert.DeserializeObject<SMSMessage>(logRelatedObjects["SmsMessages"]);

                    if (serverSmsDto!.ServerResponse.ToJToken().SelectToken("error") != null)
                    {
                        serverSmsDto.ServerResponse = JsonConvert.DeserializeObject<ServerSmsErrorResponse>(serverSmsDto.ServerResponse.ToString()!)!;
                    }
                    else
                    {
                        serverSmsDto.ServerResponse = JsonConvert.DeserializeObject<ServerSmsSuccessResponse>(serverSmsDto.ServerResponse.ToString()!)!;
                    }

                    if (serverSmsDto!.ChosenGroupId == 0)
                    {
                        return Json(new { Type = "SMS-ServerSms-NoGroup", Sms = serverSmsDto, Log = logSanitized, LogCreator = logCreator });
                    }
                    else
                    {
                        var chosenGroup = JsonConvert.DeserializeObject<Group>(logRelatedObjects["Groups"]);
                        return Json(new { Type = "SMS-ServerSms-Group", Sms = serverSmsDto, Group = chosenGroup, Log = logSanitized, LogCreator = logCreator });
                    }
                case "SmsApi":
                    var smsApiDto = JsonConvert.DeserializeObject<SMSMessage>(logRelatedObjects["SmsMessages"]);

                    if(smsApiDto!.ServerResponse.ToJToken().SelectToken("error") != null)
                    {
                        smsApiDto.ServerResponse = JsonConvert.DeserializeObject<SmsApiErrorResponse>(smsApiDto.ServerResponse.ToString()!)!;
                    }
                    else
                    {
                        smsApiDto.ServerResponse = JsonConvert.DeserializeObject<SmsApiSuccessResponse>(smsApiDto.ServerResponse.ToString()!)!;
                    }

                    if(smsApiDto.ChosenGroupId == 0)
                    {
                        return Json(new { Type = "SMS-SmsApi-NoGroup", Sms = smsApiDto, Log = logSanitized, LogCreator = logCreator });
                    }
                    else
                    {
                        var chosenGroup = JsonConvert.DeserializeObject<Group>(logRelatedObjects["Groups"]);
                        return Json(new { Type = "SMS-SmsApi-Group", Sms = smsApiDto, Group = chosenGroup, Log = logSanitized, LogCreator = logCreator });
                    }
                case "Import":
                    var importDto = JsonConvert.DeserializeObject<ImportResult>(logRelatedObjects["Imports"]);
                    return Json(new { Type = "Import", Import = importDto, Log = logSanitized, LogCreator = logCreator });
                default:
                    return StatusCode(500, "Unknown case of source log");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllLogs()
        {
            var logs = await Task.FromResult(_logService.GetAllEntries());
            return Json(logs);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaginateLogs(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            var (paginatedLogs, hasMorePages) = await _logService.PaginateLogDataAsync(firstId, lastId, pageSize, moveForward);
            return Json(new {paginatedLogs, hasMorePages});
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetLogsBySearchPhrase(string searchPhrase)
        {
            return Json(_logService.GetLogsBySearchPhrase(searchPhrase));
        }

        #endregion

        #region ApiSettings

        [Authorize]
        [HttpPost]
        public IActionResult CheckIfAuthorizationSuccessful([FromBody]string password)
        {
            var authSuccessful = _apiSettingsService.CheckIfAuthorizationSuccessful(password);

            if (authSuccessful)
            {
                var apiSettings = _apiSettingsService.GetAllEntries();
                ApiSettings activeApiSettings = null!;

                try
                {
                    activeApiSettings = apiSettings.FirstOrDefault(a => a.ApiActive == true) ?? throw new Exception("Could not find active API");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }

                apiSettings.Remove(activeApiSettings);

                var htmlContent = $@"<div class=""active-api"">
                    <span class=""form-subtitle"">API</span>
                    <div class=""form-group"" style=""margin-top:20px;"">
                        <div class=""row mb-10"">
                            <div class=""col"" style=""display: flex; justify-content: space-between; align-items:center;"">
                                <label style=""min-width:180px"">Aktywne API</label>
                                <select class=""form-input"" id=""select-active-api"" style=""margin-bottom: 0"">
                                    <option selected value=""{activeApiSettings.ApiName}"">{activeApiSettings.ApiName}</option>";

                foreach(var apiSetting in apiSettings)
                {
                    htmlContent += $@"<option value=""{apiSetting.ApiName}"">{apiSetting.ApiName}</option>";
                }
                htmlContent +=$@"</select>
                            </div>
                        </div>
                    </div>
                </div>
                <hr style=""margin: 20px;"" />
                <div class=""active-api-settings"" style=""margin-bottom:20px;"">
                    <span class=""form-subtitle"">Konfiguracja API</span>
                    <div class=""form-group"" style=""margin-top:20px;"">
                        <div class=""row"" style=""margin-bottom:20px;"">
                            <div class=""col"" style=""display: flex; justify-content: space-between; align-items:center;"">
                                <label for=""api-sender-name"" style=""min-width:180px"" style=""display:inline;"">Nazwa nadawcy</label>
                                <input type=""text"" value=""{activeApiSettings.SenderName}"" class=""form-input"" id=""api-sender-name"" style=""margin-bottom:0;"" required>
                            </div>
                        </div>
                        <div class=""row"" style=""margin-bottom:20px;"">
                            <div class=""col"">
                                <label style=""min-width:180px;"">Kanał priorytetowy</label>
                                <label class=""switch"">
                                    <input type=""checkbox"" id=""fast-channel-checkbox"" {(activeApiSettings.FastChannel == true ? "checked" : "")}>
                                    <span class=""slider round""></span>
                                </label>
                            </div>
                        </div>
                        <div class=""row"" style=""margin-bottom:36px;"">
                            <div class=""col"">
                                <label style=""min-width:180px;"">Tryb testowy</label>
                                <label class=""switch"">
                                    <input type=""checkbox"" id=""test-mode-checkbox"" {(activeApiSettings.TestMode == true ? "checked" : "")}>
                                    <span class=""slider round""></span>
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class=""d-flex justify-content-center"">
                    <button type=""submit"" class=""violet-button w-50"" id=""submit-settings-form-button"">Zapisz zmiany</button>
                </div>";
                return Content(htmlContent, "text/html");
            }
            else
            {
                string failureMessage = "Nieudana próba autoryzacji";
                return Content(failureMessage, "text/html");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateApiSettings([FromBody]UpdateApiSettingsModel model)
        {
            await _apiSettingsService.ChangeSettingsAsync(new ApiSettings
            {
                ApiName = model.ActiveApiName,
                ApiActive = true,
                SenderName = model.SenderName,
                FastChannel = model.FastChannel,
                TestMode = model.TestMode
            });

            return Ok("Successfully updated API settings");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchApiSettingsByName(string apiName)
        {
            return Json(await _apiSettingsService.GetSettingsByNameAsync(apiName));
        }
        #endregion


    }
}