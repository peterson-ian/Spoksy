using Microsoft.Extensions.DependencyInjection;
using Spoksy.Application.Commands.Users.CreateUser;
using Spoksy.Application.Commands.Users.DeactivateUser;
using Spoksy.Application.Commands.Users.UpdateUser;
using Spoksy.Application.Queries.Users.GetUser;
using Spoksy.Domain.Services;

namespace Spoksy.Application.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommandsAndQueries(this IServiceCollection services)
        {
            // Commands  
            services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();
            services.AddScoped<IUpdateUserCommandHandler, UpdateUserCommandHandler>();
            services.AddScoped<IDeactivateUserCommandHandler, DeactivateUserCommandHandler>();
 

            // Queries  
            services.AddScoped<IGetUserQueryHandler, GetUserQueryHandler>();

            return services;
        }
    }
}
