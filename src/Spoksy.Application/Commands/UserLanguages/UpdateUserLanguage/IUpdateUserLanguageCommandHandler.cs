using Spoksy.Application.Responses;
using Spoksy.Application.Commons;

namespace Spoksy.Application.Commands.UserLanguages.UpdateUserLanguage
{
    public interface IUpdateUserLanguageCommandHandler
    {
        Task<Result<UserLanguageResponse>> Handle(Guid userId, UpdateUserLanguageCommand command);
    }
} 