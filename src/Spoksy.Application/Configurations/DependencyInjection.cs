using Microsoft.Extensions.DependencyInjection;
using Spoksy.Application.Commands.Chats.CloseChat;
using Spoksy.Application.Commands.Chats.CreateChat;
using Spoksy.Application.Commands.Messages.DeleteMessage;
using Spoksy.Application.Commands.Messages.EditMessage;
using Spoksy.Application.Commands.Messages.SendMessage;
using Spoksy.Application.Commands.UserLanguages.CreateUserLanguage;
using Spoksy.Application.Commands.UserLanguages.DeleteUserLanguage;
using Spoksy.Application.Commands.UserLanguages.UpdateUserLanguage;
using Spoksy.Application.Commands.Users.CreateUser;
using Spoksy.Application.Commands.Users.DeactivateUser;
using Spoksy.Application.Commands.Users.UpdateUser;
using Spoksy.Application.Queries.ChatParticipants.CheckParticipantCanAccessChat;
using Spoksy.Application.Queries.Chats.GetAllChat;
using Spoksy.Application.Queries.Chats.GetChat;
using Spoksy.Application.Queries.Messages.GetAllMessage;
using Spoksy.Application.Queries.Messages.GetMessage;
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
            
            // User Commands
            services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();
            services.AddScoped<IUpdateUserCommandHandler, UpdateUserCommandHandler>();
            services.AddScoped<IDeactivateUserCommandHandler, DeactivateUserCommandHandler>();
            // User Language Commands
            services.AddScoped<ICreateUserLanguageCommandHandler, CreateUserLanguageCommandHandler>();
            services.AddScoped<IUpdateUserLanguageCommandHandler, UpdateUserLanguageCommandHandler>();
            services.AddScoped<IDeleteUserLanguageCommandHandler, DeleteUserLanguageCommandHandler>();
            // Chat Commands
            services.AddScoped<ICreateChatCommandHandler, CreateChatCommandHandler>();
            services.AddScoped<ICloseChatCommandHandler, CloseChatCommandHandler>();
            // Message Commands
            services.AddScoped<ISendMessageCommandHandler, SendMessageCommandHandler>();
            services.AddScoped<IEditMessageCommmandHandler, EditMessageCommmandHandler>();
            services.AddScoped<IDeleteMessageCommandHandler, DeleteMessageCommandHandler>();


            // Queries  

            // User Queries
            services.AddScoped<IGetUserQueryHandler, GetUserQueryHandler>();
            // User Language Queries
            services.AddScoped<IGetAllUserLanguageQueryHandler, GetAllUserLanguageQueryHandler>();
            services.AddScoped<IGetUserLanguageQueryHandler, GetUserLanguageQueryHandler>();
            // Chat Participants Queries
            services.AddScoped<ICheckParticipantCanAccessChatQueryHandler, CheckParticipantCanAccessChatQueryHandler>();
            // Chat Queries
            services.AddScoped<IGetChatQueryHandler, GetChatQueryHandler>();
            services.AddScoped<IGetAllChatQueryHandler, GetAllChatQueryHandler>();
            // Message Queries
            services.AddScoped<IGetAllMessageQueryHandler, GetAllMessageQueryHandler>();
            services.AddScoped<IGetMessageQueryHandler, GetMessageQueryHandler>();


            return services;
        }
    }
}
