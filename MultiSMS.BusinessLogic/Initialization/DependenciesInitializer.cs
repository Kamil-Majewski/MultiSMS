using Microsoft.Extensions.DependencyInjection;
using MultiSMS.BusinessLogic.Services;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Strategy;
using MultiSMS.BusinessLogic.Strategy.Intefaces;

namespace MultiSMS.BusinessLogic.Initialization
{
    public static class DependenciesInitializer
    {
        public static void InitializeBusinessLogicDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<IAdministratorService, AdministratorService>();
            serviceCollection.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            serviceCollection.AddScoped<IImportExportEmployeesService, ImportExportEmployeesService>();
            serviceCollection.AddScoped<ISendSMSContext, SendSMSContext>();
            serviceCollection.AddScoped<IEmployeeGroupService, EmployeeGroupService>();
            serviceCollection.AddScoped<IEmployeeService, EmployeeService>();
            serviceCollection.AddScoped<IGroupService, GroupService>();
            serviceCollection.AddScoped<ILogService, LogService>();
            serviceCollection.AddScoped<ISMSMessageTemplateService, SMSMessageTemplateService>();
            serviceCollection.AddScoped<IApiSettingsService, ApiSettingsService>();
        }
    }
}
