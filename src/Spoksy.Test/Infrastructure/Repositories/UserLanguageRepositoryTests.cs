using System;
using System.Linq;
using System.Threading.Tasks;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class UserLanguageRepositoryTests : IntegrationTestBase
    {
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly DbSet<User> _users;

        public UserLanguageRepositoryTests() : base()
        {
            _userLanguageRepository = new UserLanguageRepository(_context);
            _users = _context.Set<User>();
        }

        private async Task<User> CreateUserAsync(string name = "Test User", string email = "test@example.com")
        {
            var user = new User(
                name,
                email,
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("BR"),
                Guid.NewGuid().ToString()
            );
            await _users.AddAsync(user);
            return user;
        }

        private UserLanguage CreateUserLanguage(
            User user,
            Language language = null,
            ProficiencyLevel proficiency = ProficiencyLevel.Beginner
        )
        {
            return new UserLanguage(
                user.Id,
                language ?? Language.GetByCode("en"),
                proficiency
            );
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnUserLanguage()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user);
            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.GetByIdAsync(userLanguage.Id, user.Id);

            Assert.NotNull(result);
            Assert.Equal(userLanguage.Id, result.Id);
            Assert.Equal(userLanguage.UserId, result.UserId);
            Assert.Equal(userLanguage.Language, result.Language);
            Assert.Equal(userLanguage.ProficiencyLevel, result.ProficiencyLevel);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            var result = await _userLanguageRepository.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserLanguagesAsync_ShouldReturnAllUserLanguages()
        {
            var user = await CreateUserAsync();
            var otherUser = await CreateUserAsync();
            var userLanguage1 = CreateUserLanguage(user: user, language: Language.GetByCode("en"));
            var userLanguage2 = CreateUserLanguage(user: user, language: Language.GetByCode("es"));
            var otherUserLanguage = CreateUserLanguage(otherUser);

            await _userLanguageRepository.AddAsync(userLanguage1);
            await _userLanguageRepository.AddAsync(userLanguage2);
            await _userLanguageRepository.AddAsync(otherUserLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.GetUserLanguagesAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, ul => ul.Id == userLanguage1.Id);
            Assert.Contains(result, ul => ul.Id == userLanguage2.Id);
            Assert.All(result, ul => Assert.Equal(user.Id, ul.UserId));
        }

        [Fact]
        public async Task GetUserLanguagesAsync_ShouldReturnEmptyList()
        {
            var result = await _userLanguageRepository.GetUserLanguagesAsync(Guid.NewGuid());

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_ShouldCreateAndReturnUserLanguage()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user);

            var result = await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var savedUserLanguage = await _userLanguageRepository.GetByIdAsync(result.Id, user.Id);
            Assert.NotNull(savedUserLanguage);
            Assert.Equal(userLanguage.UserId, savedUserLanguage.UserId);
            Assert.Equal(userLanguage.Language, savedUserLanguage.Language);
            Assert.Equal(userLanguage.ProficiencyLevel, savedUserLanguage.ProficiencyLevel);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserLanguage()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user);
            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            var newProficiency = ProficiencyLevel.Advanced;
            userLanguage.UpdateProficiency(newProficiency);

            var result = await _userLanguageRepository.UpdateAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var updatedUserLanguage = await _userLanguageRepository.GetByIdAsync(userLanguage.Id, user.Id);
            Assert.NotNull(updatedUserLanguage);
            Assert.Equal(userLanguage.ProficiencyLevel, updatedUserLanguage.ProficiencyLevel);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUserLanguage()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user);
            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            await _userLanguageRepository.DeleteAsync(userLanguage.Id, user.Id);
            await _unitOfWork.CommitAsync();

            var deletedUserLanguage = await _userLanguageRepository.GetByIdAsync(userLanguage.Id, user.Id);
            Assert.Null(deletedUserLanguage);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingUserLanguage_ShouldReturnTrue()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user);
            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.ExistsAsync(userLanguage.Id, user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingUserLanguage_ShouldReturnFalse()
        {
            var result = await _userLanguageRepository.ExistsAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task CountNativeLanguages_ShouldReturnCorrectCount()
        {
            var user = await CreateUserAsync();
            var userLanguage1 = CreateUserLanguage(user: user, language: Language.GetByCode("en"), ProficiencyLevel.Native);
            var userLanguage2 = CreateUserLanguage(user: user, language: Language.GetByCode("es"));
            var otherUserLanguage = CreateUserLanguage(user);

            await _userLanguageRepository.AddAsync(userLanguage1);
            await _userLanguageRepository.AddAsync(userLanguage2);
            await _userLanguageRepository.AddAsync(otherUserLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.CountNativeLanguages(user.Id);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CountNativeLanguages_ShouldReturnIncorrectCount()
        {
            var user = await CreateUserAsync();
            var userLanguage1 = CreateUserLanguage(user: user, language: Language.GetByCode("en"), ProficiencyLevel.Native);
            var userLanguage2 = CreateUserLanguage(user: user, language: Language.GetByCode("es"), ProficiencyLevel.Native);
            var otherUserLanguage = CreateUserLanguage(user);

            await _userLanguageRepository.AddAsync(userLanguage1);
            await _userLanguageRepository.AddAsync(userLanguage2);
            await _userLanguageRepository.AddAsync(otherUserLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.CountNativeLanguages(user.Id);

            Assert.NotEqual(1, result);
        }

        [Fact]
        public async Task CountNonNativeLanguages_ShouldReturnCorrectCount()
        {
            var user = await CreateUserAsync();
            var userLanguage1 = CreateUserLanguage(user: user, language: Language.GetByCode("en"), ProficiencyLevel.Native);
            var userLanguage2 = CreateUserLanguage(user: user, language: Language.GetByCode("es"), ProficiencyLevel.Native);
            var otherUserLanguage = CreateUserLanguage(user);

            await _userLanguageRepository.AddAsync(userLanguage1);
            await _userLanguageRepository.AddAsync(userLanguage2);
            await _userLanguageRepository.AddAsync(otherUserLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.CountNonNativeLanguages(user.Id);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CountNonNativeLanguages_ShouldReturnIncorrectCount()
        {
            var user = await CreateUserAsync();
            var userLanguage1 = CreateUserLanguage(user: user, language: Language.GetByCode("en"), ProficiencyLevel.Native);
            var userLanguage2 = CreateUserLanguage(user: user, language: Language.GetByCode("es"));
            var otherUserLanguage = CreateUserLanguage(user);

            await _userLanguageRepository.AddAsync(userLanguage1);
            await _userLanguageRepository.AddAsync(userLanguage2);
            await _userLanguageRepository.AddAsync(otherUserLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.CountNonNativeLanguages(user.Id);

            Assert.NotEqual(1, result);
        }

        [Fact]
        public async Task HasNativeLanguage_ShouldReturnTrue()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user, language: Language.GetByCode("en"), ProficiencyLevel.Native);

            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNativeLanguage(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task HasNativeLanguage_ShouldReturnFalse()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user, language: Language.GetByCode("en"));

            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNativeLanguage(user.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task HasNonNativeLanguage_ShouldReturnTrue()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user, language: Language.GetByCode("en"));

            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNonNativeLanguage(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task HasNonNativeLanguage_ShouldReturnFalse()
        {
            var user = await CreateUserAsync();
            var userLanguage = CreateUserLanguage(user: user, language: Language.GetByCode("en"), ProficiencyLevel.Native);

            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNonNativeLanguage(user.Id);

            Assert.False(result);
        }
    }
} 