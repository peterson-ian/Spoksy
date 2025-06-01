using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Data;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
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

            return services;
        }
    }
}
