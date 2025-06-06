using Spoksy.Application.Responses;
using Spoksy.Application.Commons;

namespace Spoksy.Application.Queries.Users.GetUser
{
    public interface IGetUserQueryHandler
    {
        Task<Result<UserDetailsResponse>> Handle(Guid userId);
    }
} 