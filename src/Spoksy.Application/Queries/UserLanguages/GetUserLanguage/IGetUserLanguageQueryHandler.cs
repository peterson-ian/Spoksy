using Spoksy.Application.Responses;
using Spoksy.Application.Commons;

namespace Spoksy.Application.Queries.UserLanguages.GetUserLanguage
{
    public interface IGetUserLanguageQueryHandler
    {
        Task<Result<UserLanguageResponse>> Handle(Guid userId, Guid userLanguageId);
    }
}
