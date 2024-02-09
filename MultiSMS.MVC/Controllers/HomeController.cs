using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.ServerSms;
using MultiSMS.Interface.Extensions;
using MultiSMS.Interface.Repositories.Interfaces;
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
        private readonly ISMSMessageTemplateRepository _smsTemplateRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;
        private readonly ILogRepository _logRepository;
        public HomeController(ISMSMessageTemplateRepository smsTemplateRepository, IEmployeeRepository employeeRepository, IGroupRepository groupRepository, IEmployeeGroupRepository employeeGroupRepository, ILogRepository logRepository, IAdministratorService administratorService, IImportExportEmployeesService ieService)
        {
            _smsTemplateRepository = smsTemplateRepository;
            _employeeRepository = employeeRepository;
            _groupRepository = groupRepository;
            _employeeGroupRepository = employeeGroupRepository;
            _logRepository = logRepository;
            _administratorService = administratorService;
            _ieService = ieService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task AddUserToGroup(int groupId, int employeeId)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);            

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            var group = await _groupRepository.GetByIdAsync(groupId);

            await _employeeGroupRepository.AddGroupMemberAsync(groupId, employeeId);

            await _logRepository.AddEntityToDatabaseAsync(
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
        }

        [Authorize]
        [HttpGet]
        public async Task RemoveUserFromGroup(int groupId, int employeeId)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            var group = await _groupRepository.GetByIdAsync(groupId);

            await _employeeGroupRepository.RemoveGroupMember(groupId, employeeId);

            await _logRepository.AddEntityToDatabaseAsync(
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
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateNewTemplate(string templateName, string templateDescription, string templateContent)
        {

            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var template = new SMSMessageTemplate() { TemplateName = templateName, TemplateDescription = templateDescription, TemplateContent = templateContent };
            var addedTemplate = await _smsTemplateRepository.AddEntityToDatabaseAsync(template);

            await _logRepository.AddEntityToDatabaseAsync(
                new Log
                {
                    LogType = "Info",
                    LogSource = "Szablony",
                    LogMessage = $"Szablon {addedTemplate.TemplateName} został utworzony",
                    LogCreator = admin.UserName,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"Templates", JsonConvert.SerializeObject(addedTemplate) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                    })
                }
            ); 

            return Json(addedTemplate);
        }

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
            var addedContact = await _employeeRepository.AddEntityToDatabaseAsync(contact);

            await _logRepository.AddEntityToDatabaseAsync(
                  new Log
                  {
                      LogType = "Info",
                      LogSource = "Kontakty",
                      LogMessage = $"Kontakt {addedContact.Name} {addedContact.Surname} został utworzony",
                      LogCreator = admin.UserName,
                      LogCreatorId = adminId,
                      LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                      {
                        {"Employees", JsonConvert.SerializeObject(addedContact) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                      })

                  }
              );

            return Json(contact.Name);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateNewGroup(string groupName, string groupDescription)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var group = new Group() { GroupName = groupName, GroupDescription = groupDescription };
            var addedGroup =  await _groupRepository.AddEntityToDatabaseAsync(group);

            await _logRepository.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Grupy",
                     LogMessage = $"Grupa {addedGroup.GroupName} została utworzona",
                     LogCreator = admin.UserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, string>
                     {
                        {"Groups", JsonConvert.SerializeObject(addedGroup) },
                        {"Creator", JsonConvert.SerializeObject(admin) }
                     })

                 }
             );

            return Json(group.GroupName);
        }

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
            else if(status == "Partial Success")
            {
                logMessage = $"Zaimportowano nowe kontakty ({importResultDto.AddedEmployees!.Count()}), nie wszystkie grupy przypisano.";
                logType = "Info";
            }
            else
            {
                logMessage = $"Nie zaimportowano kontaktów ({importResultDto.ImportMessage})";
                logType = "Błąd";
            }

            await _logRepository.AddEntityToDatabaseAsync(
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

            return Json(new {Status = importResultDto.ImportStatus, Message = importResultDto.ImportMessage, AddedEmployees = importResultDto.AddedEmployees, RepeatedEmployees = importResultDto.RepeatedEmployees, InvalidEmployees = importResultDto.InvalidEmployees, NonExistantGroupIds = importResultDto.NonExistantGroupIds });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllTemplates()
        {
            var templates = await Task.FromResult(_smsTemplateRepository.GetAllEntries());
            return Json(templates);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllContacts()
        {
            var contacts = await Task.FromResult(_employeeRepository.GetAllEntries().ToList());
            foreach(var contact in contacts)
            {
                contact.EmployeeGroupNames = _employeeGroupRepository.GetAllGroupNamesForEmployeeQueryable(contact.EmployeeId).ToList();
            }
            return Json(contacts);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllGroups()
        {
            var groups = await Task.FromResult(_groupRepository.GetAllEntries().ToList());
            foreach(var group in groups)
            {
                group.MembersIds = _employeeGroupRepository.GetAllEmployeesIdsForGroupQueryable(group.GroupId).ToList();
            }
            return Json(groups);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FetchAllValidGroups()
        {
            var groups = await Task.FromResult(_groupRepository.GetAllGroupsWithGroupMembersQueryable().ToList());
            int amountOfInactives = 0;
            List<Group> validGroups = new List<Group>();
            foreach (var group in groups)
            {
                group.MembersIds = _employeeGroupRepository.GetAllEmployeesIdsForGroupQueryable(group.GroupId).ToList();

                if (group.GroupMembers != null)
                {
                    foreach (var member in group.GroupMembers)
                    {
                        if (member.Employee.IsActive == false)
                        {
                            amountOfInactives += 1;
                        }
                    }

                    if(amountOfInactives != group.GroupMembers.Count()) {
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
        public async Task<IActionResult> FetchAllLogs()
        {
            var logs = await Task.FromResult(_logRepository.GetAllEntries());
            return Json(logs);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            var template = await _smsTemplateRepository.GetByIdAsync(id);
            return Json(template);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _employeeRepository.GetByIdAsync(id);
            contact.EmployeeGroupNames = _employeeGroupRepository.GetAllGroupNamesForEmployeeQueryable(id).ToList();
            return Json(contact);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            group.MembersIds = _employeeGroupRepository.GetAllEmployeesIdsForGroupQueryable(group.GroupId).ToList();
            return Json(group);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditTemplate(int id, string name, string description, string content)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var template = new SMSMessageTemplate { TemplateId = id, TemplateName = name, TemplateDescription = description, TemplateContent = content};
            
            var editedTemplate = await _smsTemplateRepository.UpdateEntityAsync(template);

            await _logRepository.AddEntityToDatabaseAsync(
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

            return Json(template.TemplateName);
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

            var editedContact = await _employeeRepository.UpdateEntityAsync(contact);
            await _logRepository.AddEntityToDatabaseAsync(
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

            return Json(contact.Name);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditGroup(int id, string name, string description)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var group = new Group { GroupId = id, GroupName = name, GroupDescription = description};

            var editedGroup = await _groupRepository.UpdateEntityAsync(group);
            await _logRepository.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Grupy",
                     LogMessage = $"Grupa {editedGroup.GroupName} została zedytowana",
                     LogCreator = admin.UserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject( new Dictionary<string, string>
                     {
                         {"Groups", JsonConvert.SerializeObject(editedGroup) },
                         {"Creator", JsonConvert.SerializeObject(admin) }
                     })
                 }
             );

            return Json(group.GroupName);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeesForGroup(int groupId)
        {
            return Json(await _employeeGroupRepository.GetAllEmployeesForGroupQueryable(groupId).ToListAsync());
        }

        [Authorize]
        [HttpGet]
        public async Task DeleteTemplate(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var template = await _smsTemplateRepository.GetByIdAsync(id);
            await _logRepository.AddEntityToDatabaseAsync(
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

            await _smsTemplateRepository.DeleteEntityAsync(id);
        }

        [Authorize]
        [HttpGet]
        public async Task DeleteContact(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var contact = await _employeeRepository.GetByIdAsync(id);
            await _logRepository.AddEntityToDatabaseAsync(
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
            await _employeeRepository.DeleteEntityAsync(id);
        }

        [Authorize]
        [HttpGet]
        public async Task DeleteGroup(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var admin = await _administratorService.GetAdministratorDtoByIdAsync(adminId);

            var group = await _groupRepository.GetByIdAsync(id);
            await _logRepository.AddEntityToDatabaseAsync(
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
            await _groupRepository.DeleteEntityAsync(id);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetLog(int logId)
        {
            Dictionary<string, string> logRelatedObjects;

            var log = await _logRepository.GetByIdAsync(logId);
            var logSanitized = new { LogType = log.LogType, LogSource = log.LogSource, LogMessage = log.LogMessage, LogCreationDate = log.LogCreationDate };            

            if (log.LogRelatedObjectsDictionarySerialized == null)
            {
                throw new Exception("Error: LogRelatedObjects is null where it shouldn't be");
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
                case "SMS":
                    var smsDto = JsonConvert.DeserializeObject<SMSMessage>(logRelatedObjects["SmsMessages"]);

                    if (smsDto!.ServerResponse.ToJToken().SelectToken("error") != null)
                    {
                        smsDto.ServerResponse = JsonConvert.DeserializeObject<ServerSmsErrorResponse>(smsDto.ServerResponse.ToString()!)!;
                    }
                    else
                    {
                        smsDto.ServerResponse = JsonConvert.DeserializeObject<ServerSmsSuccessResponse>(smsDto.ServerResponse.ToString()!)!;
                    }

                    if (smsDto!.ChosenGroupId == 0)
                    {
                        return Json(new { Type = "SMS-NoGroup", Sms = smsDto, Log = logSanitized, LogCreator = logCreator });
                    }
                    else
                    {
                        var chosenGroup = JsonConvert.DeserializeObject<Group>(logRelatedObjects["Groups"]);
                        return Json(new { Type = "SMS-Group", Sms = smsDto, Group = chosenGroup, Log = logSanitized, LogCreator = logCreator });
                    }
                case "Import":
                    var importDto = JsonConvert.DeserializeObject<ImportResult>(logRelatedObjects["Imports"]);
                    return Json(new { Type = "Import", Import = importDto, Log = logSanitized, LogCreator = logCreator });
                default:
                    throw new Exception("Unknown case of log source");
            }
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
                catch( Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                return NotFound("Excel file not found");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}