using Spoksy.Application.Responses;
using Spoksy.Application.Commons;

namespace Spoksy.Application.Queries.UserLanguages.GetAllUserLanguage
{
    public interface IGetAllUserLanguageQueryHandler
    {
        Task<Result<List<UserLanguageResponse>>> Handle(Guid userId);
    }
}
