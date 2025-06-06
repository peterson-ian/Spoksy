using Spoksy.Application.Commons;

namespace Spoksy.Application.Commands.Users.DeactivateUser
{
    public interface IDeactivateUserCommandHandler
    {
        Task<Result<string>> Handle(Guid id);
    }
}
