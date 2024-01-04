﻿using Microsoft.AspNetCore.Mvc;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;
using MultiSMS.MVC.Models;
using System.Diagnostics;

namespace MultiSMS.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISMSMessageTemplateRepository _smsTemplateRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IGroupRepository _groupRepository;
        public HomeController(ISMSMessageTemplateRepository smsTemplateRepository, IEmployeeRepository employeeRepository, IGroupRepository groupRepository)
        {
            _smsTemplateRepository = smsTemplateRepository;
            _employeeRepository = employeeRepository;
            _groupRepository = groupRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewTemplate(string templateName, string templateDescription, string templateContent)
        {
            var template = new SMSMessageTemplate() { TemplateName = templateName, TemplateDescription = templateDescription, TemplateContent = templateContent };
            await _smsTemplateRepository.AddEntityToDatabaseAsync(template);
            return Json(template);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewContact(string contactName, string contactSurname, string email, string phone, string address, string zip, string city, string department, string isActive)
        {
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
            await _employeeRepository.AddEntityToDatabaseAsync(contact);

            return Json(contact.Name);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewGroup(string groupName, string groupDescription)
        {
            var group = new Group() { GroupName = groupName, GroupDescription = groupDescription };
            await _groupRepository.AddEntityToDatabaseAsync(group);
            return Json(group.GroupName);
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
            var contacts = await Task.FromResult(_employeeRepository.GetAllEntries());
            return Json(contacts);
        }

        [HttpGet]
        public async Task<IActionResult> FetchAllGroups()
        {
            var groups = await Task.FromResult(_groupRepository.GetAllEntries());
            return Json(groups);
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
            return Json(group);
        }

        [HttpGet]
        public async Task<IActionResult> EditTemplate(int id, string name, string description, string content)
        {
            var template = new SMSMessageTemplate { TemplateId = id, TemplateName = name, TemplateDescription = description, TemplateContent = content};
            
            await _smsTemplateRepository.UpdateEntityAsync(template);
            return Json(template.TemplateName);
        }

        [HttpGet]
        public async Task<IActionResult> EditContact(int contactId, string contactName, string contactSurname, string email, string phone, string address, string zip, string city, string department, string isActive)
        {
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

            await _employeeRepository.UpdateEntityAsync(contact);
            return Json(contact.Name);
        }

        [HttpGet]
        public async Task<IActionResult> EditGroup(int id, string name, string description)
        {
            var group = new Group { GroupId = id, GroupName = name, GroupDescription = description};

            await _groupRepository.UpdateEntityAsync(group);
            return Json(group.GroupName);
        }

        [HttpGet]
        public async Task DeleteTemplate(int id)
        {
            await _smsTemplateRepository.DeleteEntityAsync(id);
        }

        [HttpGet]
        public async Task DeleteContact(int id)
        {
            await _employeeRepository.DeleteEntityAsync(id);
        }

        [HttpGet]
        public async Task DeleteGroup(int id)
        {
            await _groupRepository.DeleteEntityAsync(id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}