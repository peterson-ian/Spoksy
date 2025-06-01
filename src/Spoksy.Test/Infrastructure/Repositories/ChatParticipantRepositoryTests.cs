using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class ChatParticipantRepositoryTests : IntegrationTestBase
    {
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public ChatParticipantRepositoryTests() : base()
        {
            _chatParticipantRepository = new ChatParticipantRepository(_context);
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

        private ChatParticipant CreateChatParticipant(Guid chatId, Guid userId)
        {
            return new ChatParticipant(userId, chatId);
        }

        [Fact]
        public async Task GetParticipantsByChatAsync_ShouldReturnAllParticipants()
        {
            var chat = await CreateTestChat();
            var user1 = await CreateTestUser("User1", "user1@example.com");
            var user2 = await CreateTestUser("User2", "user2@example.com");
            var participant1 = CreateChatParticipant(chat.Id, user1.Id);
            var participant2 = CreateChatParticipant(chat.Id, user2.Id);
            await _chatParticipantRepository.AddAsync(participant1);
            await _chatParticipantRepository.AddAsync(participant2);
            await _unitOfWork.CommitAsync();

            var result = await _chatParticipantRepository.GetParticipantsByChatAsync(chat.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.UserId == user1.Id);
            Assert.Contains(result, p => p.UserId == user2.Id);
        }

        [Fact]
        public async Task HasParticipantInChatAsync_WithExistingParticipant_ShouldReturnTrue()
        {
            var chat = await CreateTestChat();
            var user = await CreateTestUser();
            var participant = CreateChatParticipant(chat.Id, user.Id);
            await _chatParticipantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            var result = await _chatParticipantRepository.HasParticipantInChatAsync(chat.Id, user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task HasParticipantInChatAsync_WithNonExistingParticipant_ShouldReturnFalse()
        {
            var chat = await CreateTestChat();
            var user = await CreateTestUser();

            var result = await _chatParticipantRepository.HasParticipantInChatAsync(chat.Id, user.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task GetParticipantAsync_WithExistingParticipant_ShouldReturnParticipant()
        {
            var chat = await CreateTestChat();
            var user = await CreateTestUser();
            var participant = CreateChatParticipant(chat.Id, user.Id);

            await _chatParticipantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            var result = await _chatParticipantRepository.GetParticipantAsync(chat.Id, user.Id);

            Assert.NotNull(result);
            Assert.Equal(chat.Id, result.ChatId);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public async Task GetParticipantAsync_WithNonExistingParticipant_ShouldReturnNull()
        {
            var chat = await CreateTestChat();
            var user = await CreateTestUser();

            var result = await _chatParticipantRepository.GetParticipantAsync(chat.Id, user.Id);

            Assert.Null(result);
        }
    }
} 