using Microsoft.Extensions.DependencyInjection;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Services;

namespace Spoksy.Domain.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Domain Services 
            services.AddScoped<IUserLanguageValidationService, UserLanguageValidationService>();
            services.AddScoped<IKnowledgeExchangeValidationService, KnowledgeExchangeValidationService>();
            services.AddScoped<IChatValidationService, ChatValidationService>();

            return services;
        }
    }
}
