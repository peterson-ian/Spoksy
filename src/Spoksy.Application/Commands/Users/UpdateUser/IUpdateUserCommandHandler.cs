using Spoksy.Application.Responses;
using Spoksy.Application.Commons;

namespace Spoksy.Application.Commands.Users.UpdateUser
{
    public interface IUpdateUserCommandHandler
    {
        Task<Result<UserDetailsResponse>> Handle(Guid id, UpdateUserCommand command);
    }
} 