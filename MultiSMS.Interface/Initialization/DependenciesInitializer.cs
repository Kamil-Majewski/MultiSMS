using Microsoft.Extensions.DependencyInjection;
using MultiSMS.Interface.Repositories;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Initialization
{
    public static class DependenciesInitializer
    {
        public static void InitializeMultiSMSInfrastructureDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEmployeeRepository, EmployeeRepository>();
            serviceCollection.AddScoped<IEmployeesGroupRepository, EmployeesGroupRepository>();
            serviceCollection.AddScoped<ISMSMessageTemplateRepository, SMSMessageTemplateRepository>();
            serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
    }
}
