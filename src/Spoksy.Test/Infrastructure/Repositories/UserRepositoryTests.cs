using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class UserRepositoryTests : IntegrationTestBase
    {
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests() : base()
        {
            _userRepository = new UserRepository(_context);
        }

        private User CreateValidUser(string name = "Test User", string email = "test@example.com")
        {
            return new User(
                name,
                email,
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("BR")
            );
        }

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
        {
            var user = CreateValidUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.GetByEmailAsync(user.Email);

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
        {
            var result = await _userRepository.GetByEmailAsync("teste@email.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_WithUniqueEmail_ShouldReturnTrue()
        {
            var uniqueEmail = "unique@example.com";

            var result = await _userRepository.IsEmailUniqueAsync(uniqueEmail);

            Assert.True(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_WithExistingEmail_ShouldReturnFalse()
        {
            var user = CreateValidUser();
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _userRepository.IsEmailUniqueAsync(user.Email);

            Assert.False(result);
        }
    }
}
