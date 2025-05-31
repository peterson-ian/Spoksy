using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Test.Domain.Entities
{
    public class UserLanguageTests
    {
        private readonly Guid _validUserId = Guid.NewGuid();
        private readonly Language _validLanguage = Language.GetByCode("en");
        private readonly ProficiencyLevel _initialProficiencyLevel = ProficiencyLevel.Beginner;

        [Fact]
        public void CreateUserLanguage_WithValidData_ShouldCreateSuccessfully()
        {
            var userLanguage = new UserLanguage(_validUserId, _validLanguage, _initialProficiencyLevel);

            Assert.NotNull(userLanguage);
            Assert.Equal(_validUserId, userLanguage.UserId);
            Assert.Equal(_validLanguage, userLanguage.Language);
            Assert.Equal(_initialProficiencyLevel, userLanguage.ProficiencyLevel);
            Assert.NotEqual(Guid.Empty, userLanguage.Id);
            Assert.True(userLanguage.StartedLearningOn.Kind == DateTimeKind.Utc);
        }

        [Fact]
        public void CreateUserLanguage_WithNullLanguage_ShouldThrowDomainException()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new UserLanguage(_validUserId, null, _initialProficiencyLevel));
            Assert.Equal("Language cannot be null", exception.Message);
        }

        [Fact]
        public void UpdateProficiency_WithDifferentLevel_ShouldUpdateSuccessfully()
        {
            var userLanguage = new UserLanguage(_validUserId, _validLanguage, ProficiencyLevel.Beginner);
            var newLevel = ProficiencyLevel.Intermediate;

            userLanguage.UpdateProficiency(newLevel);

            Assert.Equal(newLevel, userLanguage.ProficiencyLevel);
        }

        [Fact]
        public void UpdateProficiency_WithSameLevel_ShouldThrowDomainException()
        {
            var userLanguage = new UserLanguage(_validUserId, _validLanguage, ProficiencyLevel.Beginner);

            var exception = Assert.Throws<DomainException>(() =>
                userLanguage.UpdateProficiency(ProficiencyLevel.Beginner));
            Assert.Equal("New proficiency level must be different from the current one", exception.Message);
        }

        [Fact]
        public void UpdateUserId_WithValidId_ShouldUpdateSuccessfully()
        {
            var userLanguage = new UserLanguage(_validUserId, _validLanguage, _initialProficiencyLevel);
            var newUserId = Guid.NewGuid();

            userLanguage.UpdateUserId(newUserId);

            Assert.Equal(newUserId, userLanguage.UserId);
        }

        [Fact]
        public void UpdateUserId_WithEmptyGuid_ShouldThrowDomainException()
        {
            var userLanguage = new UserLanguage(_validUserId, _validLanguage, _initialProficiencyLevel);

            var exception = Assert.Throws<DomainException>(() =>
                userLanguage.UpdateUserId(Guid.Empty));
            Assert.Equal("User ID cannot be empty", exception.Message);
        }
    }
} 