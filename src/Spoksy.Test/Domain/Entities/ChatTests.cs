using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Test.Domain.Entities
{
    public class ChatTests
    {
        private readonly Language _primaryLanguage = Language.GetByCode("en");
        private readonly Language _secondaryLanguage = Language.GetByCode("pt");

        [Fact]
        public void CreateChat_WithValidData_ShouldCreateChatSuccessfully()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);

            Assert.NotNull(chat);
            Assert.Equal(_primaryLanguage, chat.PrimaryLanguage);
            Assert.Equal(_secondaryLanguage, chat.SecondaryLanguage);
            Assert.NotEqual(Guid.Empty, chat.Id);
            Assert.Equal(ChatStatus.Active, chat.Status);
            Assert.Equal(2, chat.MaxParticipants);
        }

        [Fact]
        public void CreateChat_WithNullPrimaryLanguage_ShouldThrowDomainException()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new Chat(null, _secondaryLanguage));
            Assert.Equal("Primary language cannot be null", exception.Message);
        }

        [Fact]
        public void CreateChat_WithNullSecondaryLanguage_ShouldThrowDomainException()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new Chat(_primaryLanguage, null));
            Assert.Equal("Secondary language cannot be null", exception.Message);
        }

        [Fact]
        public void CloseChat_ShouldSetStatusToClosed()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);
            var lastActivity = chat.LastActivityAt;

            chat.CloseChat();

            Assert.Equal(ChatStatus.Closed, chat.Status);
            Assert.True(chat.LastActivityAt > lastActivity);
        }

        [Fact]
        public void ArchiveChat_ShouldSetStatusToArchived()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);
            var lastActivity = chat.LastActivityAt;

            chat.ArchiveChat();

            Assert.Equal(ChatStatus.Archived, chat.Status);
            Assert.True(chat.LastActivityAt > lastActivity);
        }

        [Fact]
        public void ReactivateChat_ShouldSetStatusToActive()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);
            chat.CloseChat();
            var lastActivity = chat.LastActivityAt;

            chat.ReactivateChat();

            Assert.Equal(ChatStatus.Active, chat.Status);
            Assert.True(chat.LastActivityAt > lastActivity);
        }

        [Fact]
        public void IsLanguageSupported_WithPrimaryLanguage_ShouldReturnTrue()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);

            var result = chat.IsLanguageSupported(_primaryLanguage);

            Assert.True(result);
        }

        [Fact]
        public void IsLanguageSupported_WithSecondaryLanguage_ShouldReturnTrue()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);

            var result = chat.IsLanguageSupported(_secondaryLanguage);

            Assert.True(result);
        }

        [Fact]
        public void IsLanguageSupported_WithUnsupportedLanguage_ShouldReturnFalse()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);
            var unsupportedLanguage = Language.GetByCode("es");

            var result = chat.IsLanguageSupported(unsupportedLanguage);

            Assert.False(result);
        }

        [Fact]
        public void IsActive_WithActiveStatus_ShouldReturnTrue()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);

            Assert.True(chat.IsActive());
        }

        [Fact]
        public void IsActive_WithClosedStatus_ShouldReturnFalse()
        {
            var chat = new Chat(_primaryLanguage, _secondaryLanguage);
            chat.CloseChat();

            Assert.False(chat.IsActive());
        }
    }
} 