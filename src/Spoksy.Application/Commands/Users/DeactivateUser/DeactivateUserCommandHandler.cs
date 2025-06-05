using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Domain.Contracts;

namespace Spoksy.Application.Commands.Users.DeactivateUser
{
    public class DeactivateUserCommandHandler : IDeactivateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityProviderIntegration _identityProviderIntegration;

        public DeactivateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IIdentityProviderIntegration identityProviderIntegration)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _identityProviderIntegration = identityProviderIntegration;
        }

        public async Task<Result<string>> Handle(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFoundResult<string>.Create("User not found");

            user.DeactivateUser();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            await _identityProviderIntegration.DeleteUserAsync(user.IdentityProviderId);

            return Result<string>.Success("User deactivated successfully");
        }
    }
}
