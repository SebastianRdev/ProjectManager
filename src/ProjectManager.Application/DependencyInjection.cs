using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManager.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskItemService, TaskItemService>();
            return services;
        }
    }
}
