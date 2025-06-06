using Spoksy.Application.Commons;

namespace Spoksy.Application.Commands.UserLanguages.DeleteUserLanguage
{
    public interface IDeleteUserLanguageCommandHandler
    {
        Task<Result<string>> Handle(Guid userId, Guid id);
    }
} 