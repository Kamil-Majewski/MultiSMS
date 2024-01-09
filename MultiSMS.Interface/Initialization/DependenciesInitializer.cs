using Microsoft.Extensions.DependencyInjection;
using MultiSMS.Interface.Repositories;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Initialization
{
    public static class DependenciesInitializer
    {
        public static void InitializeInfrastructureDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmployeeRepository, EmployeeRepository>();
            serviceCollection.AddScoped<IAdministratorRepository, AdministratorRepository>();
            serviceCollection.AddScoped<IGroupRepository, GroupRepository>();
            serviceCollection.AddScoped<ISMSMessageTemplateRepository, SMSMessageTemplateRepository>();
            serviceCollection.AddScoped<IEmployeeGroupRepository, EmployeeGroupRepository>();
            serviceCollection.AddScoped<ILogRepository, LogRepository>();
            serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        }
    }
}
