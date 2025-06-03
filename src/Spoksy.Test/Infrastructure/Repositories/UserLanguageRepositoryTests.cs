using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class UserLanguageRepositoryTests : IntegrationTestBase
    {
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IUserRepository _userRepository;

        public UserLanguageRepositoryTests() : base()
        {
            _userLanguageRepository = new UserLanguageRepository(_context);
            _userRepository = new UserRepository(_context);
        }

        private async Task<User> CreateTestUser()
        {
            var user = new User(
                "Test User",
                "test@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("BR"),
                Guid.NewGuid().ToString()
            );
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        private UserLanguage CreateUserLanguage(Guid userId, string languageCode = "en", ProficiencyLevel proficiency = ProficiencyLevel.Native)
        {
            return new UserLanguage(
                userId,
                Language.GetByCode(languageCode),
                proficiency
            );
        }

        [Fact]
        public async Task GetUserLanguagesAsync_ShouldReturnAllUserLanguages()
        {
            var user = await CreateTestUser();
            var language1 = CreateUserLanguage(user.Id, "en", ProficiencyLevel.Native);
            var language2 = CreateUserLanguage(user.Id, "es", ProficiencyLevel.Intermediate);
            await _userLanguageRepository.AddAsync(language1);
            await _userLanguageRepository.AddAsync(language2);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.GetUserLanguagesAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, l => l.Language.Code == "en");
            Assert.Contains(result, l => l.Language.Code == "es");
        }

        [Fact]
        public async Task HasLanguageAsync_WithExistingLanguage_ShouldReturnTrue()
        {
            var user = await CreateTestUser();
            var language = CreateUserLanguage(user.Id, "en");
            await _userLanguageRepository.AddAsync(language);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasLanguageAsync(user.Id, Language.English);

            Assert.True(result);
        }

        [Fact]
        public async Task HasLanguageAsync_WithNonExistingLanguage_ShouldReturnFalse()
        {
            var user = await CreateTestUser();

            var result = await _userLanguageRepository.HasLanguageAsync(user.Id, Language.English);

            Assert.False(result);
        }

        [Fact]
        public async Task CountNativeLanguages_ShouldReturnCorrectCount()
        {
            var user = await CreateTestUser();
            var language1 = CreateUserLanguage(user.Id, "en", ProficiencyLevel.Native);
            var language2 = CreateUserLanguage(user.Id, "es", ProficiencyLevel.Native);
            var language3 = CreateUserLanguage(user.Id, "pt", ProficiencyLevel.Intermediate);
            await _userLanguageRepository.AddAsync(language1);
            await _userLanguageRepository.AddAsync(language2);
            await _userLanguageRepository.AddAsync(language3);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.CountNativeLanguages(user.Id);

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CountNonNativeLanguages_ShouldReturnCorrectCount()
        {
            var user = await CreateTestUser();
            var language1 = CreateUserLanguage(user.Id, "en", ProficiencyLevel.Native);
            var language2 = CreateUserLanguage(user.Id, "es", ProficiencyLevel.Intermediate);
            var language3 = CreateUserLanguage(user.Id, "pt", ProficiencyLevel.Beginner);
            await _userLanguageRepository.AddAsync(language1);
            await _userLanguageRepository.AddAsync(language2);
            await _userLanguageRepository.AddAsync(language3);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.CountNonNativeLanguages(user.Id);

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task HasNativeLanguage_WithNativeLanguage_ShouldReturnTrue()
        {
            var user = await CreateTestUser();
            var language = CreateUserLanguage(user.Id, "en", ProficiencyLevel.Native);
            await _userLanguageRepository.AddAsync(language);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNativeLanguage(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task HasNativeLanguage_WithoutNativeLanguage_ShouldReturnFalse()
        {
            var user = await CreateTestUser();
            var language = CreateUserLanguage(user.Id, "en", ProficiencyLevel.Intermediate);
            await _userLanguageRepository.AddAsync(language);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNativeLanguage(user.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task HasNonNativeLanguage_WithNonNativeLanguage_ShouldReturnTrue()
        {
            var user = await CreateTestUser();
            var language = CreateUserLanguage(user.Id, "en", ProficiencyLevel.Intermediate);
            await _userLanguageRepository.AddAsync(language);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNonNativeLanguage(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task HasNonNativeLanguage_WithoutNonNativeLanguage_ShouldReturnFalse()
        {
            var user = await CreateTestUser();
            var language = CreateUserLanguage(user.Id, "en", ProficiencyLevel.Native);
            await _userLanguageRepository.AddAsync(language);
            await _unitOfWork.CommitAsync();

            var result = await _userLanguageRepository.HasNonNativeLanguage(user.Id);

            Assert.False(result);
        }
    }
} 