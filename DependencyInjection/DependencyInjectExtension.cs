using DependencyInjection.Services;

namespace DependencyInjection
{
    public static class DependencyInjectExtension
    {
        public static void RegisterDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDoSomethingService, DoSomethingService>(
                    d => new DoSomethingService(configuration.GetSection("ConfigA").Value)
                );
        }
    }
}
