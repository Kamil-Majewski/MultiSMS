using Microsoft.Extensions.DependencyInjection;
using MultiSMS.BusinessLogic.Services;
using MultiSMS.BusinessLogic.Services.Interfaces;

namespace MultiSMS.BusinessLogic.Initialization
{
    public static class DependenciesInitializer
    {
        public static void InitializeBusinessLogicDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<IAdministratorService, AdministratorService>();
            serviceCollection.AddScoped<IServerSmsService, ServerSmsService>();
            serviceCollection.AddScoped<ISmsApiService, SmsApiService>();
            serviceCollection.AddScoped<IImportExportEmployeesService, ImportExportEmployeesService>();
            serviceCollection.AddScoped<ISMSMessageService, SMSMessageService>();
            serviceCollection.AddScoped<IImportResultService, ImportResultService>();
        }
    }
}
