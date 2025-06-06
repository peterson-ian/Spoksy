
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;
using Xunit;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class UserRepositoryTests : IntegrationTestBase
    {
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests() : base()
        {
            _userRepository = new UserRepository(_context);
        }

        private User CreateUser(
            string name = "Test User",
            string email = "test@example.com",
            string identityProviderId = null
        )
        {
            return new User(
                name,
                email,
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("BR"),
                identityProviderId ?? Guid.NewGuid().ToString()
            );
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnUser()
        {
            var user = CreateUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.GetByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            var result = await _userRepository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
        {
            var email = "unique@example.com";
            var user = CreateUser(email: email);
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.GetByEmailAsync(email);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
        {
            var result = await _userRepository.GetByEmailAsync("nonexistent@example.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WithInactiveUser_ShouldReturnNull()
        {
            var email = "inactive@example.com";
            var user = CreateUser(email: email);
            user.DeactivateUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.GetByEmailAsync(email);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdentityProviderIdAsync_WithExistingId_ShouldReturnUser()
        {
            var identityProviderId = "auth0|123456";
            var user = CreateUser(identityProviderId: identityProviderId);
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.GetByIdentityProviderIdAsync(identityProviderId);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(identityProviderId, result.IdentityProviderId);
        }

        [Fact]
        public async Task GetByIdentityProviderIdAsync_WithInactiveUser_ShouldReturnNull()
        {
            var identityProviderId = "auth0|inactive";
            var user = CreateUser(identityProviderId: identityProviderId);
            user.DeactivateUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.GetByIdentityProviderIdAsync(identityProviderId);

            Assert.Null(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_WithUniqueEmail_ShouldReturnTrue()
        {
            var email = "unique@example.com";

            var result = await _userRepository.IsEmailUniqueAsync(email);

            Assert.True(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_WithExistingActiveEmail_ShouldReturnFalse()
        {
            var email = "existing@example.com";
            var user = CreateUser(email: email);
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.IsEmailUniqueAsync(email);


            Assert.False(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_WithExistingInactiveEmail_ShouldReturnTrue()
        {
            var email = "inactive@example.com";
            var user = CreateUser(email: email);
            user.DeactivateUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.IsEmailUniqueAsync(email);

            Assert.True(result);
        }

        [Fact]
        public async Task AddAsync_ShouldCreateAndReturnUser()
        {
            var user = CreateUser();

            var result = await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var savedUser = await _userRepository.GetByIdAsync(result.Id);
            Assert.NotNull(savedUser);
            Assert.Equal(user.Email, savedUser.Email);
            Assert.Equal(user.Name, savedUser.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            var user = CreateUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var newName = "Updated Name";
            user.UpdateName(newName);

            var result = await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var updatedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(newName, updatedUser.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser()
        {
            var user = CreateUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            await _userRepository.DeleteAsync(user.Id);
            await _unitOfWork.CommitAsync();

            var deletedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingUser_ShouldReturnTrue()
        {
            var user = CreateUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.ExistsAsync(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingUser_ShouldReturnFalse()
        {
            var result = await _userRepository.ExistsAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }
}
