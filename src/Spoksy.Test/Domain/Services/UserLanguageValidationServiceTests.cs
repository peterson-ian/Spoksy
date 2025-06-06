using Moq;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Test.Domain.Services
{
    public class UserLanguageValidationServiceTests
    {
        private readonly Mock<IUserLanguageRepository> _repositoryMock;
        private readonly UserLanguageValidationService _service;

        public UserLanguageValidationServiceTests()
        {
            _repositoryMock = new Mock<IUserLanguageRepository>();
            _service = new UserLanguageValidationService(_repositoryMock.Object);
        }

        [Fact]
        public async Task EnsureValidLanguagesForUserCreation_WithValidLanguages_ShouldNotThrow()
        {
            var nativeLanguages = new List<Language> { Language.English };
            var nonNativeLanguages = new List<Language> { Language.Portuguese };

            await _service.EnsureValidLanguagesForUserCreation(nativeLanguages, nonNativeLanguages);
        }

        [Fact]
        public async Task EnsureValidLanguagesForUserCreation_WithNullNativeLanguages_ShouldThrowDomainException()
        {
            List<Language> nativeLanguages = null;
            var nonNativeLanguages = new List<Language> { Language.Portuguese };

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureValidLanguagesForUserCreation(nativeLanguages, nonNativeLanguages));

            Assert.Equal("At least one native language is required.", exception.Message);
        }

        [Fact]
        public async Task EnsureValidLanguagesForUserCreation_WithEmptyNativeLanguages_ShouldThrowDomainException()
        {
            var nativeLanguages = new List<Language>();
            var nonNativeLanguages = new List<Language> { Language.Portuguese };

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureValidLanguagesForUserCreation(nativeLanguages, nonNativeLanguages));

            Assert.Equal("At least one native language is required.", exception.Message);
        }

        [Fact]
        public async Task EnsureValidLanguagesForUserCreation_WithNullNonNativeLanguages_ShouldThrowDomainException()
        {
            var nativeLanguages = new List<Language> { Language.English };
            List<Language> nonNativeLanguages = null;

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureValidLanguagesForUserCreation(nativeLanguages, nonNativeLanguages));

            Assert.Equal("At least one non-native language is required.", exception.Message);
        }

        [Fact]
        public async Task EnsureValidLanguagesForUserCreation_WithEmptyNonNativeLanguages_ShouldThrowDomainException()
        {
            var nativeLanguages = new List<Language> { Language.English };
            var nonNativeLanguages = new List<Language>();

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureValidLanguagesForUserCreation(nativeLanguages, nonNativeLanguages));

            Assert.Equal("At least one non-native language is required.", exception.Message);
        }

        [Fact]
        public async Task EnsureLanguageCanBeRemoved_WithValidLanguageAndMoreThanTwoNativeLanguages_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var userLanguage = new UserLanguage(userId, Language.English, ProficiencyLevel.Native);

            _repositoryMock.Setup(r => r.GetByIdAsync(languageId, userId))
                .ReturnsAsync(userLanguage);
            _repositoryMock.Setup(r => r.CountNativeLanguages(userId))
                .ReturnsAsync(3);

            await _service.EnsureLanguageCanBeRemoved(userId, languageId);
        }

        [Fact]
        public async Task EnsureLanguageCanBeRemoved_WithValidLanguageAndMoreThanTwoNonNativeLanguages_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var userLanguage = new UserLanguage(userId, Language.English, ProficiencyLevel.Intermediate);

            _repositoryMock.Setup(r => r.GetByIdAsync(languageId, userId))
                .ReturnsAsync(userLanguage);
            _repositoryMock.Setup(r => r.CountNonNativeLanguages(userId))
                .ReturnsAsync(3);

            await _service.EnsureLanguageCanBeRemoved(userId, languageId);
        }

        [Fact]
        public async Task EnsureLanguageCanBeRemoved_WithNonExistentLanguage_ShouldThrowDomainException()
        {
            var userId = Guid.NewGuid();
            var languageId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(languageId, userId))
                .ReturnsAsync((UserLanguage)null);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureLanguageCanBeRemoved(userId, languageId));

            Assert.Equal("Language not found", exception.Message);
        }

        [Fact]
        public async Task EnsureLanguageCanBeRemoved_WithOnlyOneNativeLanguages_ShouldThrowDomainException()
        {
            var userId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var userLanguage = new UserLanguage(userId, Language.English, ProficiencyLevel.Native);

            _repositoryMock.Setup(r => r.GetByIdAsync(languageId, userId))
                .ReturnsAsync(userLanguage);
            _repositoryMock.Setup(r => r.CountNativeLanguages(userId))
                .ReturnsAsync(1);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureLanguageCanBeRemoved(userId, languageId));

            Assert.Equal("You must have at least two native languages to remove one of them", exception.Message);
        }

        [Fact]
        public async Task EnsureLanguageCanBeRemoved_WithOnlyOneNonNativeLanguages_ShouldThrowDomainException()
        {
            var userId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var userLanguage = new UserLanguage(userId, Language.English, ProficiencyLevel.Intermediate);

            _repositoryMock.Setup(r => r.GetByIdAsync(languageId, userId))
                .ReturnsAsync(userLanguage);
            _repositoryMock.Setup(r => r.CountNonNativeLanguages(userId))
                .ReturnsAsync(1);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureLanguageCanBeRemoved(userId, languageId));

            Assert.Equal("You must have at least two non-native languages to remove one of them", exception.Message);
        }

        [Fact]
        public async Task EnsureUserLanguageRequirements_WithValidRequirements_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.HasNativeLanguage(userId))
                .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.HasNonNativeLanguage(userId))
                .ReturnsAsync(true);

            await _service.EnsureUserLanguageRequirements(userId);
        }

        [Fact]
        public async Task EnsureUserLanguageRequirements_WithoutNativeLanguage_ShouldThrowDomainException()
        {
            var userId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.HasNativeLanguage(userId))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasNonNativeLanguage(userId))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureUserLanguageRequirements(userId));

            Assert.Equal("User must have at least one native language", exception.Message);
        }

        [Fact]
        public async Task EnsureUserLanguageRequirements_WithoutNonNativeLanguage_ShouldThrowDomainException()
        {
            var userId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.HasNativeLanguage(userId))
                .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.HasNonNativeLanguage(userId))
                .ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureUserLanguageRequirements(userId));

            Assert.Equal("User must have at least one language to learn", exception.Message);
        }
    }
} 