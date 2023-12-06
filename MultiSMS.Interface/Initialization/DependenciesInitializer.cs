using Microsoft.Extensions.DependencyInjection;
using MultiSMS.Interface.Repositories;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Initialization
{
    public static class DependenciesInitializer
    {
        public static void InitializeElectionResultsInfrastructureDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUsersGroupRepository, UsersGroupRepository>();
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IEmployeeRoleRepository, EmployeeRoleRepository>();
        }
    }
}
