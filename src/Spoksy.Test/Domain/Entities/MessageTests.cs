using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Test.Domain.Entities
{
    public class MessageTests
    {
        private readonly Guid _validChatId = Guid.NewGuid();
        private readonly Guid _validSenderId = Guid.NewGuid();
        private readonly string _validContent = "Hello, this is a test message";
        private readonly Language _validLanguage = Language.GetByCode("en");

        [Fact]
        public void CreateMessage_WithValidData_ShouldCreateMessageSuccessfully()
        {
            var message = new Message(_validChatId, _validSenderId, _validContent, _validLanguage);

            Assert.NotNull(message);
            Assert.Equal(_validChatId, message.ChatId);
            Assert.Equal(_validSenderId, message.SenderId);
            Assert.Equal(_validContent, message.Content);
            Assert.Equal(_validLanguage, message.Language);
            Assert.NotEqual(Guid.Empty, message.Id);
            Assert.False(message.IsRead);
            Assert.False(message.IsDelete);
            Assert.False(message.IsEdit);
        }

        [Fact]
        public void CreateMessage_WithEmptyChatId_ShouldThrowDomainException()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new Message(Guid.Empty, _validSenderId, _validContent, _validLanguage));
            Assert.Equal("Chat ID cannot be empty", exception.Message);
        }

        [Fact]
        public void CreateMessage_WithEmptySenderId_ShouldThrowDomainException()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new Message(_validChatId, Guid.Empty, _validContent, _validLanguage));
            Assert.Equal("Sender ID cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateMessage_WithInvalidContent_ShouldThrowDomainException(string invalidContent)
        {
            var exception = Assert.Throws<DomainException>(() =>
                new Message(_validChatId, _validSenderId, invalidContent, _validLanguage));
            Assert.Equal("Message content cannot be empty", exception.Message);
        }

        [Fact]
        public void SetRead_ShouldMarkMessageAsRead()
        {
            var message = new Message(_validChatId, _validSenderId, _validContent, _validLanguage);

            message.SetRead();

            Assert.True(message.IsRead);
        }

        [Fact]
        public void EditMessage_WithValidData_ShouldEditMessageSuccessfully()
        {
            var message = new Message(_validChatId, _validSenderId, _validContent, _validLanguage);
            var newContent = "Updated message content";

            message.EditMessage(_validChatId, _validSenderId, newContent);

            Assert.Equal(newContent, message.Content);
            Assert.True(message.IsEdit);
            Assert.NotNull(message.EditAt);
        }

        [Fact]
        public void EditMessage_WithDifferentUser_ShouldThrowDomainException()
        {
            var message = new Message(_validChatId, _validSenderId, _validContent, _validLanguage);
            var differentUserId = Guid.NewGuid();

            var exception = Assert.Throws<DomainException>(() =>
                message.EditMessage(_validChatId, differentUserId, "New content"));
            Assert.Equal("The user dont have access to edit the message", exception.Message);
        }

        [Fact]
        public void DeleteMessage_WithValidData_ShouldDeleteMessageSuccessfully()
        {
            var message = new Message(_validChatId, _validSenderId, _validContent, _validLanguage);

            message.DeleteMessage(_validChatId, _validSenderId);

            Assert.True(message.IsDelete);
            Assert.NotNull(message.DeleteAt);
        }

        [Fact]
        public void DeleteMessage_WithDifferentUser_ShouldThrowDomainException()
        {
            var message = new Message(_validChatId, _validSenderId, _validContent, _validLanguage);
            var differentUserId = Guid.NewGuid();

            var exception = Assert.Throws<DomainException>(() =>
                message.DeleteMessage(_validChatId, differentUserId));
            Assert.Equal("The user dont have access to delete the message", exception.Message);
        }
    }
} 