using Moq;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Test.Domain.Services
{
    public class KnowledgeExchangeValidationServiceTests
    {
        private readonly Mock<IUserLanguageValidationService> _userLanguageValidationServiceMock;
        private readonly Mock<IUserLanguageRepository> _userLanguageRepositoryMock;
        private readonly IKnowledgeExchangeValidationService _service;

        public KnowledgeExchangeValidationServiceTests()
        {
            _userLanguageRepositoryMock = new Mock<IUserLanguageRepository>();
            _userLanguageValidationServiceMock = new Mock<IUserLanguageValidationService>();
            _service = new KnowledgeExchangeValidationService(_userLanguageValidationServiceMock.Object, _userLanguageRepositoryMock.Object);
        }

        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithValidExchange_ShouldNotThrow()
        {
            var firstUserId = Guid.NewGuid();
            var languageA = Language.English;
            var languageB = Language.Portuguese;
            var secondUserId = Guid.NewGuid();

            var firstUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(firstUserId, languageA, ProficiencyLevel.Native), 
                new UserLanguage(firstUserId, languageB, ProficiencyLevel.Beginner) 
            };

            var secondUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(secondUserId, languageA, ProficiencyLevel.Beginner), 
                new UserLanguage(secondUserId, languageB, ProficiencyLevel.Native) 
            };

            _userLanguageValidationServiceMock
              .Setup(r => r.EnsureUserLanguageRequirements(firstUserId))
              .Returns(Task.CompletedTask);

            _userLanguageValidationServiceMock
              .Setup(r => r.EnsureUserLanguageRequirements(secondUserId))
              .Returns(Task.CompletedTask);

            _userLanguageRepositoryMock.Setup(r => r.GetUserLanguagesAsync(firstUserId))
                .ReturnsAsync(firstUserLanguages);

            _userLanguageRepositoryMock.Setup(r => r.GetUserLanguagesAsync(secondUserId))
                .ReturnsAsync(secondUserLanguages);

            await _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, languageA, languageB);
        }

        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithEmptyFirstUserId_ShouldThrowDomainException()
        {
            var firstUserId = Guid.Empty;
            var languageA = Language.English;
            var languageB = Language.Portuguese;
            var secondUserId = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, languageA, languageB));

            Assert.Equal("First user ID cannot be empty", exception.Message);
        }

        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithDuplicateUserId_ShouldThrowDomainException()
        {
            var firstUserId = Guid.NewGuid();
            var languageA = Language.English;
            var languageB = Language.Portuguese;
            var secondUserId = firstUserId;

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, languageA, languageB));

            Assert.Equal("Users must be different for knowledge exchange", exception.Message);
        }

        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithNullLanguageA_ShouldThrowDomainException()
        {
            var firstUserId = Guid.NewGuid();
            var languageB = Language.Portuguese;
            var secondUserId = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, null, languageB));

            Assert.Equal("Language A cannot be null", exception.Message);
        }

        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithNullLanguageB_ShouldThrowDomainException()
        {
            var firstUserId = Guid.NewGuid();
            var languageA = Language.English;
            var secondUserId = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, languageA, null));

            Assert.Equal("Language B cannot be null", exception.Message);
        }


        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithDuplicateLanguage_ShouldThrowDomainException()
        {
            var firstUserId = Guid.NewGuid();
            var languageA = Language.English;
            var secondUserId = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, languageA, languageA));

            Assert.Equal("Languages must be different for knowledge exchange", exception.Message);
        }

        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithFirstUserMissingLanguage_ShouldThrowDomainException()
        {
            var firstUserId = Guid.NewGuid();
            var languageA = Language.English;
            var languageB = Language.Portuguese;
            var languageC = Language.Spanish;
            var secondUserId = Guid.NewGuid();

            var firstUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(firstUserId, languageA, ProficiencyLevel.Native),
                new UserLanguage(firstUserId, languageB, ProficiencyLevel.Beginner)
            };

            var secondUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(secondUserId, languageA, ProficiencyLevel.Beginner),
                new UserLanguage(secondUserId, languageB, ProficiencyLevel.Native)
            };

            _userLanguageValidationServiceMock
              .Setup(r => r.EnsureUserLanguageRequirements(firstUserId))
              .Returns(Task.CompletedTask);

            _userLanguageValidationServiceMock
              .Setup(r => r.EnsureUserLanguageRequirements(secondUserId))
              .Returns(Task.CompletedTask);

            _userLanguageRepositoryMock.Setup(r => r.GetUserLanguagesAsync(firstUserId))
                .ReturnsAsync(firstUserLanguages);

            _userLanguageRepositoryMock.Setup(r => r.GetUserLanguagesAsync(secondUserId))
                .ReturnsAsync(secondUserLanguages);

           var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, languageA, languageC));

            Assert.Equal($"User {firstUserId} must have both selected languages", exception.Message);
        }

        [Fact]
        public async Task EnsureKnowledgeExchangeAsync_WithInvalidExchange_ShouldThrowDomainException()
        {
            var firstUserId = Guid.NewGuid();
            var languageA = Language.English;
            var languageB = Language.Portuguese;
            var languageC = Language.Spanish;
            var secondUserId = Guid.NewGuid();

            var firstUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(firstUserId, languageA, ProficiencyLevel.Native),
                new UserLanguage(firstUserId, languageB, ProficiencyLevel.Beginner),
                new UserLanguage(firstUserId, languageC, ProficiencyLevel.Beginner)
            };

            var secondUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(secondUserId, languageC, ProficiencyLevel.Intermediate),
                new UserLanguage(secondUserId, languageB, ProficiencyLevel.Native)
            };

            _userLanguageValidationServiceMock
              .Setup(r => r.EnsureUserLanguageRequirements(firstUserId))
              .Returns(Task.CompletedTask);

            _userLanguageValidationServiceMock
              .Setup(r => r.EnsureUserLanguageRequirements(secondUserId))
              .Returns(Task.CompletedTask);

            _userLanguageRepositoryMock.Setup(r => r.GetUserLanguagesAsync(firstUserId))
                .ReturnsAsync(firstUserLanguages);

            _userLanguageRepositoryMock.Setup(r => r.GetUserLanguagesAsync(secondUserId))
                .ReturnsAsync(secondUserLanguages);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                 _service.EnsureKnowledgeExchangeAsync(firstUserId, secondUserId, languageB, languageC));

            Assert.Equal("Users do not have complementary proficiency levels for knowledge exchange", exception.Message);
        }
    }
}
