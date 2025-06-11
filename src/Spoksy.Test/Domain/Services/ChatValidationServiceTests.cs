using Moq;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;
using System;

namespace Spoksy.Test.Domain.Services
{
    public class ChatValidationServiceTests
    {
        private readonly Mock<IChatRepository> _chatRepositoryMock;
        private readonly Mock<IChatParticipantRepository> _chatParticipantRepositoryMock;
        private readonly ChatValidationService _service;

        public ChatValidationServiceTests()
        {
            _chatRepositoryMock = new Mock<IChatRepository>();
            _chatParticipantRepositoryMock = new Mock<IChatParticipantRepository>();
            _service = new ChatValidationService(_chatRepositoryMock.Object, _chatParticipantRepositoryMock.Object);
        }


        [Fact]
        public async Task EnsureChatAvailableAsync_WithValidChat_ShouldNotThrow()
        {
            var chat = new Chat(Language.English, Language.Spanish);

            _chatRepositoryMock.Setup(r => r.GetByIdAsync(chat.Id))
                .ReturnsAsync(chat);

            await _service.EnsureChatAvailableAsync(chat.Id);
        }

        [Fact]
        public async Task EnsureChatAvailableAsync_WithEmptyChatId_ShouldThrowDomainException()
        {
            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureChatAvailableAsync(Guid.Empty));

            Assert.Equal("Chat ID cannot be empty.", exception.Message);
        }

        [Fact]
        public async Task EnsureChatAvailableAsync_WithInvalidChatId_ShouldThrowDomainException()
        {
            var chatId = Guid.NewGuid();    

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                 _service.EnsureChatAvailableAsync(chatId));

            Assert.Equal($"Chat with ID {chatId} not found.", exception.Message);
        }

        [Fact]
        public async Task EnsureChatAvailableAsync_WithCloseChat_ShouldThrowDomainException()
        {
            var chat = new Chat(Language.English, Language.Spanish);
            chat.CloseChat();

            _chatRepositoryMock.Setup(r => r.GetByIdAsync(chat.Id))
                .ReturnsAsync(chat);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureChatAvailableAsync(chat.Id));

            Assert.Equal($"Chat with ID {chat.Id} is not active.", exception.Message);
        }

        [Fact]
        public async Task EnsureUserAccessForChatAsync_WithValidChatParticipant_ShouldNotThrow()
        {
            var userId = Guid.NewGuid();
            var chat = new Chat(Language.English, Language.Spanish);
            var participant = new ChatParticipant(userId, chat.Id);

            _chatParticipantRepositoryMock.Setup(r => r.GetByChatIdAndUserIdAsync(chat.Id, userId))
                .ReturnsAsync(participant);

            await _service.EnsureUserAccessForChatAsync(userId, chat.Id);
        }

        [Fact]
        public async Task EnsureUserAccessForChatAsync_WithEmptyChatId_ShouldThrowDomainException()
        {
            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureUserAccessForChatAsync( Guid.NewGuid(), Guid.Empty));

            Assert.Equal("Chat ID cannot be empty.", exception.Message);
        }

        [Fact]
        public async Task EnsureUserAccessForChatAsync_WithEmptyUserId_ShouldThrowDomainException()
        {
            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                _service.EnsureUserAccessForChatAsync(Guid.Empty, Guid.NewGuid()));

            Assert.Equal("User ID cannot be empty.", exception.Message);
        }

        [Fact]
        public async Task EnsureUserAccessForChatAsync_WithInvalidChatParticipant_ShouldThrowDomainException()
        {
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                 _service.EnsureUserAccessForChatAsync(userId, chatId));

            Assert.Equal($"User with ID {userId} is not a participant of chat with ID {chatId}.", exception.Message);
        }

        [Fact]
        public async Task EnsureUserAccessForChatAsync_WithLeavedChatParticipant_ShouldThrowDomainException()
        {
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            var participant = new ChatParticipant(userId, chatId);
            participant.Leave();

            _chatParticipantRepositoryMock.Setup(r => r.GetByChatIdAndUserIdAsync(chatId, userId))
               .ReturnsAsync(participant);

            var exception = await Assert.ThrowsAsync<DomainException>(() =>
                 _service.EnsureUserAccessForChatAsync(userId, chatId));

            Assert.Equal($"User with ID {userId} has left the chat with ID {chatId}.", exception.Message);
        }
    }
}
