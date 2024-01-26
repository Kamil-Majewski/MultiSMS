using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Extensions;
using MultiSMS.Interface.Repositories.Interfaces;
using MultiSMS.MVC.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;

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

        [HttpGet]
        public async Task AddUserToGroup(int groupId, int employeeId)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();
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
                    LogCreator = adminUserName,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>
                    {
                        {"Employees", employeeId },
                        {"Groups", groupId }

                    })
                }
            );
        }

        [HttpGet]
        public async Task RemoveUserFromGroup(int groupId, int employeeId)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();
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
                    LogCreator = adminUserName,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>
                    {
                        {"Employees", employeeId },
                        {"Groups", groupId }
                    })
                }
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewTemplate(string templateName, string templateDescription, string templateContent)
        {

            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();

            var template = new SMSMessageTemplate() { TemplateName = templateName, TemplateDescription = templateDescription, TemplateContent = templateContent };
            var addedTemplate = await _smsTemplateRepository.AddEntityToDatabaseAsync(template);

            await _logRepository.AddEntityToDatabaseAsync(
                new Log
                {
                    LogType = "Info",
                    LogSource = "Szablony",
                    LogMessage = $"Szablon {addedTemplate.TemplateName} został utworzony",
                    LogCreator = adminUserName,
                    LogCreatorId = adminId,
                    LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>
                    {
                        { "Templates", template.TemplateId }
                    })

                }
            ); 

            return Json(addedTemplate);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewContact(string contactName, string contactSurname, string email, string phone, string address, string zip, string city, string department, string isActive)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();

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
                      LogCreator = adminUserName,
                      LogCreatorId = adminId,
                      LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>
                      {
                        { "Employees", addedContact.EmployeeId }
                      })

                  }
              );

            return Json(contact.Name);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewGroup(string groupName, string groupDescription)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();

            var group = new Group() { GroupName = groupName, GroupDescription = groupDescription };
            var addedGroup =  await _groupRepository.AddEntityToDatabaseAsync(group);

            await _logRepository.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Grupy",
                     LogMessage = $"Grupa {addedGroup.GroupName} została utworzona",
                     LogCreator = adminUserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>
                     {
                        { "Groups", addedGroup.GroupId}
                     })

                 }
             );

            return Json(group.GroupName);
        }

        [HttpPost]
        public async Task<IActionResult> ImportContacts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            return Json(await _ieService.ImportContactsCsvAsync(file));
        }

        [HttpGet]
        public async Task<IActionResult> FetchAllTemplates()
        {
            var templates = await Task.FromResult(_smsTemplateRepository.GetAllEntries());
            return Json(templates);
        }

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

        [HttpGet]
        public async Task<IActionResult> FetchAllLogs()
        {
            var logs = await Task.FromResult(_logRepository.GetAllEntries());
            return Json(logs);
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            var template = await _smsTemplateRepository.GetByIdAsync(id);
            return Json(template);
        }

        [HttpGet]
        public async Task<IActionResult> GetContactById(int id)
        {
            var template = await _employeeRepository.GetByIdAsync(id);
            return Json(template);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            group.MembersIds = _employeeGroupRepository.GetAllEmployeesIdsForGroupQueryable(group.GroupId).ToList();
            return Json(group);
        }

        [HttpGet]
        public async Task<IActionResult> EditTemplate(int id, string name, string description, string content)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();

            var template = new SMSMessageTemplate { TemplateId = id, TemplateName = name, TemplateDescription = description, TemplateContent = content};
            
            var editedTemplate = await _smsTemplateRepository.UpdateEntityAsync(template);

            await _logRepository.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Szablony",
                     LogMessage = $"Szablon {editedTemplate.TemplateName} został zedytowany",
                     LogCreator = adminUserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>
                     {
                        { "Templates", editedTemplate.TemplateId}
                     })

                 }
             );

            return Json(template.TemplateName);
        }

        [HttpGet]
        public async Task<IActionResult> EditContact(int contactId, string contactName, string contactSurname, string email, string phone, string address, string zip, string city, string department, string isActive)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();

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
                     LogCreator = adminUserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject(new Dictionary<string, int>
                     {
                        { "Employees", editedContact.EmployeeId}
                     })

                 }
             );

            return Json(contact.Name);
        }

        [HttpGet]
        public async Task<IActionResult> EditGroup(int id, string name, string description)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();

            var group = new Group { GroupId = id, GroupName = name, GroupDescription = description};

            var editedGroup = await _groupRepository.UpdateEntityAsync(group);
            await _logRepository.AddEntityToDatabaseAsync(
                 new Log
                 {
                     LogType = "Info",
                     LogSource = "Grupy",
                     LogMessage = $"Grupa {editedGroup.GroupName} została zedytowana",
                     LogCreator = adminUserName,
                     LogCreatorId = adminId,
                     LogRelatedObjectsDictionarySerialized = JsonConvert.SerializeObject( new Dictionary<string, int>
                     {
                         {"Groups", editedGroup.GroupId}
                     })
                 }
             );

            return Json(group.GroupName);
        }

        [HttpGet]
        public async Task DeleteTemplate(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();
            var template = await _smsTemplateRepository.GetByIdAsync(id);
            await _logRepository.AddEntityToDatabaseAsync(
                new Log
                {
                    LogType = "Info",
                    LogSource = "Szablony",
                    LogMessage = $"Szablon {template.TemplateName} został usunięty",
                    LogCreator = adminUserName,
                    LogCreatorId = adminId,
                }
            );

            await _smsTemplateRepository.DeleteEntityAsync(id);
        }

        [HttpGet]
        public async Task DeleteContact(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();
            var contact = await _employeeRepository.GetByIdAsync(id);
            await _logRepository.AddEntityToDatabaseAsync(
               new Log
               {
                   LogType = "Info",
                   LogSource = "Kontakty",
                   LogMessage = $"Kontakt {contact.Name} {contact.Surname} ({contact.PhoneNumber}) został usunięty",
                   LogCreator = adminUserName,
                   LogCreatorId = adminId,
               }
           );
            await _employeeRepository.DeleteEntityAsync(id);
        }

        [HttpGet]
        public async Task DeleteGroup(int id)
        {
            var adminId = User.GetLoggedInUserId<int>();
            var adminUserName = User.GetLoggedInUserName();
            var group = await _groupRepository.GetByIdAsync(id);
            await _logRepository.AddEntityToDatabaseAsync(
               new Log
               {
                   LogType = "Info",
                   LogSource = "Grupy",
                   LogMessage = $"Grupa {group.GroupName} została usunięta",
                   LogCreator = adminUserName,
                   LogCreatorId = adminId,
               }
           );
            await _groupRepository.DeleteEntityAsync(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetLog(int id)
        {
            Dictionary<string, int> logRelatedObjects;

            var log = await _logRepository.GetByIdAsync(id);
            var logSanitized = new { LogType = log.LogType, LogSource = log.LogSource, LogMessage = log.LogMessage, LogCreationDate = log.LogCreationDate };
            var logCreator = await _administratorService.GetAdministratorDtoByIdAsync(log.LogCreatorId);

            if (log.LogRelatedObjectsDictionarySerialized == null)
            {
                
                if (log.LogSource == "Szablony" || log.LogSource == "Kontakty" || log.LogSource == "Grupy")
                {
                    return Json(new { Type = "Entity-Delete", Log = logSanitized, LogCreator = logCreator });
                }
                else
                {
                    throw new Exception("Error: LogRelatedObjects is null where it shouldn't be");
                }
            }
            else
            {
                logRelatedObjects = JsonConvert.DeserializeObject<Dictionary<string, int>>(log.LogRelatedObjectsDictionarySerialized)!;
            }
            
            switch (log.LogSource)
            {
                case "Szablony":
                    var template = await _smsTemplateRepository.GetByIdAsync(logRelatedObjects["Templates"]);
                    return Json(new { Type = "Template", Template = template, Log = logSanitized, logCreator = logCreator });
                case "Kontakty":
                    var employee = await _employeeRepository.GetByIdAsync(logRelatedObjects["Employees"]);
                    return Json(new { Type = "Contact", Contact = employee, Log = logSanitized, logCreator = logCreator });
                case "Grupy":
                    var group = await _groupRepository.GetByIdAsync(logRelatedObjects["Groups"]);
                    try
                    {
                        var addedEmployee = await _employeeRepository.GetByIdAsync(logRelatedObjects["Employees"]);
                        return Json(new { Type = "Groups-Assign", Group = group, Contact = addedEmployee, Log = logSanitized, logCreator = logCreator });
                    }
                    catch (Exception)
                    {
                        return Json(new { Type = "Groups", Group = group, Log = logSanitized, logCreator = logCreator });
                    }
                case "SMS":
                    var sms = await _smsTemplateRepository.GetByIdAsync(logRelatedObjects["SmsMessages"]);
                    try
                    {
                        var chosenGroup = await _groupRepository.GetByIdAsync(logRelatedObjects["Groups"]);
                        return Json(new { Type = "SMS-Group", Sms = sms, Group = chosenGroup, Log = logSanitized, logCreator = logCreator });
                    }
                    catch (Exception)
                    {
                        return Json(new { Type = "SMS-NoGroup", Sms = sms, Log = logSanitized, logCreator = logCreator });
                    }
                default:
                    throw new Exception("Unknown case of log source");
            }
        }

        public IActionResult DownloadExcelWithContacts()
        {
            var fileDirectory = _ieService.ExportContactsExcel();

            if (System.IO.File.Exists(fileDirectory))
            {
                try
                {
                    var fileBytes = System.IO.File.ReadAllBytes(fileDirectory);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MultiSMS-Kontakty.xlsx");
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