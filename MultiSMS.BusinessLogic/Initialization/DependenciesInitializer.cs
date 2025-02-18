using Microsoft.Extensions.DependencyInjection;
using MultiSMS.BusinessLogic.Services;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Factory;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Factory.Interface;

namespace MultiSMS.BusinessLogic.Initialization
{
    public static class DependenciesInitializer
    {
        public static void InitializeBusinessLogicDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            serviceCollection.AddScoped<IImportExportEmployeesService, ImportExportEmployeesService>();
            serviceCollection.AddScoped<ISendSMSContext, SendSMSContext>();
            serviceCollection.AddScoped<IEmployeeGroupService, EmployeeGroupService>();
            serviceCollection.AddScoped<IEmployeeService, EmployeeService>();
            serviceCollection.AddScoped<IGroupService, GroupService>();
            serviceCollection.AddScoped<ILogService, LogService>();
            serviceCollection.AddScoped<ISMSMessageTemplateService, SMSMessageTemplateService>();
            serviceCollection.AddScoped<IApiSettingsService, ApiSettingsService>();
            serviceCollection.AddScoped<ISmsClient, ServerSmsClient>();
            serviceCollection.AddScoped<ISmsClient, SmsApiClient>();
            serviceCollection.AddScoped<ISmsClientFactory, SmsClientFactory>();
            serviceCollection.AddScoped<ISendSMSContext, SendSMSContext>();
            serviceCollection.AddScoped<IApiSettingsService, ApiSettingsService>();

            serviceCollection.AddHttpClient<ServerSmsClient>(client =>
            {
                client.BaseAddress = new Uri("https://api2.serwersms.pl/");
            });
            serviceCollection.AddHttpClient<SmsApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.smsapi.pl/");
            });
        }
    }
}
