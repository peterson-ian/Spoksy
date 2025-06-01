using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class MessageRepositoryTests : IntegrationTestBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public MessageRepositoryTests() : base()
        {
            _messageRepository = new MessageRepository(_context);
            _chatRepository = new ChatRepository(_context);
            _userRepository = new UserRepository(_context);
        }

        private async Task<User> CreateTestUser(string name = "Test User", string email = "test@example.com")
        {
            var user = new User(
                name,
                email,
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("BR")
            );
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        private async Task<Chat> CreateTestChat(string name = "Test Chat")
        {
            var chat = new Chat(
                Language.GetByCode("en"),
                Language.GetByCode("es")
            );
            await _chatRepository.AddAsync(chat);
            await _unitOfWork.CommitAsync();
            return chat;
        }

        private Message CreateMessage(
            Guid chatId,
            Guid userId,
            string content = "Test message")
        {
            return new Message(chatId, userId, content);
        }

        [Fact]
        public async Task GetMessagesByChatAsync_ShouldReturnAllChatMessages()
        {
            var chat = await CreateTestChat();
            var user = await CreateTestUser();
            var message1 = CreateMessage(chat.Id, user.Id, "First message");
            var message2 = CreateMessage(chat.Id, user.Id, "Second message");
            await _messageRepository.AddAsync(message1);
            await _messageRepository.AddAsync(message2);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetMessagesByChatAsync(chat.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Content == "First message");
            Assert.Contains(result, m => m.Content == "Second message");
        }

        [Fact]
        public async Task GetMessagesByUserAsync_ShouldReturnAllUserMessages()
        {
            var chat1 = await CreateTestChat("Chat 1");
            var chat2 = await CreateTestChat("Chat 2");
            var user = await CreateTestUser();
            var otherUser = await CreateTestUser("Other User", "other@example.com");
            var message1 = CreateMessage(chat1.Id, user.Id, "User message 1");
            var message2 = CreateMessage(chat2.Id, user.Id, "User message 2");
            var otherMessage = CreateMessage(chat1.Id, otherUser.Id, "Other user message");
            await _messageRepository.AddAsync(message1);
            await _messageRepository.AddAsync(message2);
            await _messageRepository.AddAsync(otherMessage);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetMessagesByUserAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, m => Assert.Equal(user.Id, m.SenderId));
            Assert.Contains(result, m => m.Content == "User message 1");
            Assert.Contains(result, m => m.Content == "User message 2");
        }

        [Fact]
        public async Task GetUnreadMessagesCountAsync_ShouldReturnCorrectCount()
        {
            var chat = await CreateTestChat();
            var user = await CreateTestUser();
            var message1 = CreateMessage(chat.Id, user.Id);
            var message2 = CreateMessage(chat.Id, user.Id);
            var message3 = CreateMessage(chat.Id, user.Id);
            message3.SetRead();
            await _messageRepository.AddAsync(message1);
            await _messageRepository.AddAsync(message2);
            await _messageRepository.AddAsync(message3);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetUnreadMessagesCountAsync(chat.Id, user.Id);

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetLastMessageFromChatAsync_ShouldReturnMostRecentMessage()
        {
            var chat = await CreateTestChat();
            var user = await CreateTestUser();
            var message1 = CreateMessage(chat.Id, user.Id, "First message");
            var message2 = CreateMessage(chat.Id, user.Id, "Last message");
            await _messageRepository.AddAsync(message1);
            await _messageRepository.AddAsync(message2);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetLastMessageFromChatAsync(chat.Id);

            Assert.NotNull(result);
            Assert.Equal("Last message", result.Content);
        }

        [Fact]
        public async Task GetLastMessageFromChatAsync_WithNoMessages_ShouldReturnNull()
        {
            var chat = await CreateTestChat();

            var result = await _messageRepository.GetLastMessageFromChatAsync(chat.Id);

            Assert.Null(result);
        }
    }
} 