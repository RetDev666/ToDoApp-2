using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Core.Application.Intefaces.Services;
using ToDoApp.Infrastructure.ExternalServices.Services;

namespace ToDoApp.Infrastructure.ExternalServices
{
    public static class ServiceExtensions
    {
        public static void AddExternalServicesInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelegramNotificationService, TelegramNotificationService>();
        }
    }
}