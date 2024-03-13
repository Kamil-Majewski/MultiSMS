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
            serviceCollection.AddScoped<IImportExportEmployeesService, ImportExportEmployeesService>();
            serviceCollection.AddScoped<ISendSMSContext, SendSMSContext>();
        }
    }
}
