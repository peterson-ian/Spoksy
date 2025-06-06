using Moq;
using Spoksy.Application.Commands.Users.DeactivateUser;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Application.Commons.Results;
using Spoksy.Test.Infrastructure;

namespace Spoksy.Test.Application.Commands.Users
{
    public class DeactivateUserCommandTests : IntegrationTestBase
    {
        private readonly IDeactivateUserCommandHandler _handler;
        private readonly IUserRepository _userRepository;
        private readonly Mock<IIdentityProviderIntegration> _identityProviderMock;

        public DeactivateUserCommandTests() : base()
        {
            _userRepository = new UserRepository(_context);
            _identityProviderMock = new Mock<IIdentityProviderIntegration>();

            _identityProviderMock.Setup(x => x.DeleteUserAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _handler = new DeactivateUserCommandHandler(
                _userRepository,
                _unitOfWork,
                _identityProviderMock.Object
            );
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldDeactivateUser()
        {
            var user = new User(
                "Test User",
                "test@test.com",
                DateTime.UtcNow.AddYears(-20),
                Country.GetByCode("BR"),
                "test_identity_id"
            );
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _handler.Handle(user.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal("User deactivated successfully", result.Value);

            var deactivatedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.NotNull(deactivatedUser);
            Assert.False(deactivatedUser.IsActive());

            _identityProviderMock.Verify(x => x.DeleteUserAsync(user.IdentityProviderId), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldReturnNotFound()
        {
            var invalidId = Guid.NewGuid();

            var result = await _handler.Handle(invalidId);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<string>);
            Assert.Contains("User not found", result.Errors);

            _identityProviderMock.Verify(x => x.DeleteUserAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenIdentityProviderFails_ShouldStillDeactivateUser()
        {
            _identityProviderMock.Setup(x => x.DeleteUserAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Identity provider error"));

            var user = new User(
                "Test User",
                "test@test.com",
                DateTime.UtcNow.AddYears(-20),
                Country.GetByCode("BR"),
                "test_identity_id"
            );
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            await Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(user.Id));

        }
    }
} 