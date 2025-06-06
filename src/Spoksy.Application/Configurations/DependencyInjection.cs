using Microsoft.Extensions.DependencyInjection;
using Spoksy.Application.Commands.UserLanguages.CreateUserLanguage;
using Spoksy.Application.Commands.UserLanguages.DeleteUserLanguage;
using Spoksy.Application.Commands.UserLanguages.UpdateUserLanguage;
using Spoksy.Application.Commands.Users.CreateUser;
using Spoksy.Application.Commands.Users.DeactivateUser;
using Spoksy.Application.Commands.Users.UpdateUser;
using Spoksy.Application.Queries.UserLanguages.GetAllUserLanguage;
using Spoksy.Application.Queries.UserLanguages.GetUserLanguage;
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
            services.AddScoped<ICreateUserLanguageCommandHandler, CreateUserLanguageCommandHandler>();
            services.AddScoped<IUpdateUserLanguageCommandHandler, UpdateUserLanguageCommandHandler>();
            services.AddScoped<IDeleteUserLanguageCommandHandler, DeleteUserLanguageCommandHandler>();


            // Queries  
            services.AddScoped<IGetUserQueryHandler, GetUserQueryHandler>();
            services.AddScoped<IGetAllUserLanguageQueryHandler, GetAllUserLanguageQueryHandler>();
            services.AddScoped<IGetUserLanguageQueryHandler, GetUserLanguageQueryHandler>();


            return services;
        }
    }
}
