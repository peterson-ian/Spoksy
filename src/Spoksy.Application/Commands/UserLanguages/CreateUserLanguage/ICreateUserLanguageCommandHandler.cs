using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Commands.UserLanguages.CreateUserLanguage
{
    public interface ICreateUserLanguageCommandHandler
    {
        Task<Result<UserLanguageResponse>> Handle(Guid userId, CreateUserLanguageCommand command);
    }
} 