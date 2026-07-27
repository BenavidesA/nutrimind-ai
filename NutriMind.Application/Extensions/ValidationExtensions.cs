using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NutriMind.Application.Extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddApplicationValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
