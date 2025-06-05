using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class ChatParticipantRepositoryTests : IntegrationTestBase
    {
        private readonly IChatParticipantRepository _participantRepository;
        private readonly DbSet<Chat> _chats;
        private readonly DbSet<User> _users;

        public ChatParticipantRepositoryTests() : base()
        {
            _participantRepository = new ChatParticipantRepository(_context);
            _chats = _context.Set<Chat>();
            _users = _context.Set<User>();
        }

        private async Task<Chat> CreateChatAsync()
        {
            var chat = new Chat(
                Language.GetByCode("en"),
                Language.GetByCode("es")
            );
            await _chats.AddAsync(chat);
            return chat;
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

        private async Task<ChatParticipant> CreateParticipantAsync(
            Chat chat = null,
            User user = null
        )
        {
            chat ??= await CreateChatAsync();
            user ??= await CreateUserAsync();
            
            var participant = new ChatParticipant(user.Id, chat.Id);
            return participant;
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnParticipant()
        {
            var participant = await CreateParticipantAsync();
            await _participantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            var result = await _participantRepository.GetByIdAsync(participant.Id);

            Assert.NotNull(result);
            Assert.Equal(participant.Id, result.Id);
            Assert.Equal(participant.ChatId, result.ChatId);
            Assert.Equal(participant.UserId, result.UserId);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            var result = await _participantRepository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByChatIdAsync_ShouldReturnAllParticipantsForChat()
        {
            var chat = await CreateChatAsync();
            var participant1 = await CreateParticipantAsync(chat: chat);
            var participant2 = await CreateParticipantAsync(chat: chat);
            var otherParticipant = await CreateParticipantAsync();

            await _participantRepository.AddAsync(participant1);
            await _participantRepository.AddAsync(participant2);
            await _participantRepository.AddAsync(otherParticipant);
            await _unitOfWork.CommitAsync();

            var result = await _participantRepository.GetByChatIdAsync(chat.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Id == participant1.Id);
            Assert.Contains(result, p => p.Id == participant2.Id);
            Assert.All(result, p => Assert.Equal(chat.Id, p.ChatId));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnAllParticipationsForUser()
        {
            var user = await CreateUserAsync();
            var participant1 = await CreateParticipantAsync(user: user);
            var participant2 = await CreateParticipantAsync(user: user);
            var otherParticipant = await CreateParticipantAsync();

            await _participantRepository.AddAsync(participant1);
            await _participantRepository.AddAsync(participant2);
            await _participantRepository.AddAsync(otherParticipant);
            await _unitOfWork.CommitAsync();

            var result = await _participantRepository.GetByUserIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Id == participant1.Id);
            Assert.Contains(result, p => p.Id == participant2.Id);
            Assert.All(result, p => Assert.Equal(user.Id, p.UserId));
        }

        [Fact]
        public async Task AddAsync_ShouldCreateAndReturnParticipant()
        {
            var participant = await CreateParticipantAsync();

            var result = await _participantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var savedParticipant = await _participantRepository.GetByIdAsync(result.Id);
            Assert.NotNull(savedParticipant);
            Assert.Equal(participant.ChatId, savedParticipant.ChatId);
            Assert.Equal(participant.UserId, savedParticipant.UserId);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateParticipant()
        {
            var participant = await CreateParticipantAsync();
            await _participantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            participant.Leave();

            var result = await _participantRepository.UpdateAsync(participant);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var updatedParticipant = await _participantRepository.GetByIdAsync(participant.Id);
            Assert.NotNull(updatedParticipant);
            Assert.NotNull(updatedParticipant.LeaveAt);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteParticipant()
        {
            var participant = await CreateParticipantAsync();
            await _participantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            await _participantRepository.DeleteAsync(participant.Id);
            await _unitOfWork.CommitAsync();

            var deletedParticipant = await _participantRepository.GetByIdAsync(participant.Id);
            Assert.Null(deletedParticipant);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingParticipant_ShouldReturnTrue()
        {
            var participant = await CreateParticipantAsync();
            await _participantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            var result = await _participantRepository.ExistsAsync(participant.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingParticipant_ShouldReturnFalse()
        {
            var result = await _participantRepository.ExistsAsync(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task IsUserInChatAsync_WhenUserIsInChat_ShouldReturnTrue()
        {
            var chat = await CreateChatAsync();
            var user = await CreateUserAsync();
            var participant = await CreateParticipantAsync(chat: chat, user: user);
            await _participantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            var result = await _participantRepository.IsUserInChatAsync(chat.Id, user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task IsUserInChatAsync_WhenUserIsNotInChat_ShouldReturnFalse()
        {
            var chat = await CreateChatAsync();
            var user = await CreateUserAsync();
            await _unitOfWork.CommitAsync();

            var result = await _participantRepository.IsUserInChatAsync(chat.Id, user.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task GetParticipantCountAsync_ShouldReturnCorrectCount()
        {
            var chat = await CreateChatAsync();
            var participant1 = await CreateParticipantAsync(chat: chat);
            var participant2 = await CreateParticipantAsync(chat: chat);
            var otherParticipant = await CreateParticipantAsync();

            await _participantRepository.AddAsync(participant1);
            await _participantRepository.AddAsync(participant2);
            await _participantRepository.AddAsync(otherParticipant);
            await _unitOfWork.CommitAsync();

            var result = await _participantRepository.GetParticipantCountAsync(chat.Id);

            Assert.Equal(2, result);
        }
    }
} 