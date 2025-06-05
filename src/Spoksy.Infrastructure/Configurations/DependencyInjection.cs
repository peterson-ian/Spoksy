using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Data;
using Spoksy.Infrastructure.Integrations.IdentityProvider;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Database Context
            services.AddScoped<DbContext, AppDbContext>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // DB Connection Factory
            services.AddScoped<IDbConnectionFactory>(provider => new AppConnectionFactory(connectionString));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserLanguageRepository, UserLanguageRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IChatParticipantRepository, ChatParticipantRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            // Integrations
            services.Configure<KeycloakConfiguration>(options =>
            {
                options.AuthServerUrl = configuration["AUTH_SERVER_URL"];
                options.Realm = configuration["AUTH_REALM"];
                options.Resource = configuration["AUTH_INTEGRATION_CLIENT"];
                options.Secret = configuration["AUTH_INTEGRATION_SECRET"];
            });

            services.AddHttpClient<IKeycloakTokenManager, KeycloakTokenManager>()
                .AddPolicyHandler(KeycloakResiliencePolicies.GetCombinedPolicies());
            services.AddHttpClient<IIdentityProviderIntegration, KeycloakIdentityProviderIntegration>()
                .AddPolicyHandler(KeycloakResiliencePolicies.GetCombinedPolicies());


            return services;
        }
    }
}
