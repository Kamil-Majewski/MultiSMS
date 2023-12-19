using Microsoft.AspNetCore.Mvc;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories;
using MultiSMS.Interface.Repositories.Interfaces;
using MultiSMS.MVC.Models;
using System.Diagnostics;

namespace MultiSMS.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISMSMessageTemplateRepository _smsTemplateRepository;
        public HomeController(ISMSMessageTemplateRepository smsTemplateRepository)
        {
            _smsTemplateRepository = smsTemplateRepository;
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
        public async Task<IActionResult> FetchAllTemplates()
        {
            var templates = await Task.FromResult(_smsTemplateRepository.GetAllEntries());
            return Json(templates);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}