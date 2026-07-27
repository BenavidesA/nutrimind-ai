using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NutriMind.Application.Extensions
{
    public static class MappingExtensions
    {
        public static IServiceCollection AddApplicationMappings(this IServiceCollection services)
        {
            // Mapster will automatically scan the entire NutriMind.Application assembly
            // for IRegister implementations and apply their configurations.
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
