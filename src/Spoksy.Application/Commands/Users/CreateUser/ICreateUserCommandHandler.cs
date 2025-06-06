using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Commands.Users.CreateUser
{
    public interface ICreateUserCommandHandler
    {
        Task<Result<UserDetailsResponse>> Handle(CreateUserCommand command);
    }
} 