using Microsoft.Extensions.DependencyInjection;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Initialization
{
    public static class DependenciesInitializer
    {
        public static void InitializeInfrastructureDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(MultiSmsGenericRepository<>));
            serviceCollection.AddScoped(typeof(IGenericRepository<ApiToken>), typeof(LocalGenericRepository<ApiToken>));
            serviceCollection.AddScoped(typeof(IGenericRepository<ApiSmsSender>), typeof(LocalGenericRepository<ApiSmsSender>));
            serviceCollection.AddScoped(typeof(IGenericRepository<ApiSmsSenderUser>), typeof(LocalGenericRepository<ApiSmsSenderUser>));
        }
    }
}
