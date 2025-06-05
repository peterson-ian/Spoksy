using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class ChatRepositoryTests : IntegrationTestBase
    {
        private readonly IChatRepository _chatRepository;
        private readonly DbSet<ChatParticipant> _participants;
        private readonly DbSet<User> _users;

        public ChatRepositoryTests() : base()
        {
            _chatRepository = new ChatRepository(_context);
            _participants = _context.Set<ChatParticipant>();
            _users = _context.Set<User>();
        }

        private Chat CreateChat(
            Language primaryLanguage = null,
            Language secondaryLanguage = null
        )
        {
            return new Chat(
                primaryLanguage ?? Language.GetByCode("en"),
                secondaryLanguage ?? Language.GetByCode("es")
            );
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

        private async Task<ChatParticipant> CreateParticipantAsync(Chat chat, User user)
        {
            var participant = new ChatParticipant(user.Id, chat.Id);
            await _participants.AddAsync(participant);
            return participant;
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnChat()
        {
            var chat =  CreateChat();
            await _chatRepository.AddAsync(chat);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.GetByIdAsync(chat.Id);

            Assert.NotNull(result);
            Assert.Equal(chat.Id, result.Id);
            Assert.Equal(chat.PrimaryLanguage, result.PrimaryLanguage);
            Assert.Equal(chat.SecondaryLanguage, result.SecondaryLanguage);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            var result = await _chatRepository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserChatsAsync_ShouldReturnUserChats()
        {
            var user = await CreateUserAsync();
            var otherUser = await CreateUserAsync();
            var chat1 = CreateChat();
            var chat2 = CreateChat();
            var otherChat = CreateChat();

            await _chatRepository.AddAsync(chat1);
            await _chatRepository.AddAsync(chat2);
            await _chatRepository.AddAsync(otherChat);

            await CreateParticipantAsync(chat1, user);
            await CreateParticipantAsync(chat2, user);
            await CreateParticipantAsync(otherChat, otherUser);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.GetUserChatsAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == chat1.Id);
            Assert.Contains(result, c => c.Id == chat2.Id);
        }

        [Fact]
        public async Task GetActivesUserChatsByAsync_ShouldReturnOnlyActiveUserChats()
        {
            var user = await CreateUserAsync();
            var activeChat = CreateChat();
            var inactiveChat = CreateChat();
            inactiveChat.CloseChat();

            await _chatRepository.AddAsync(activeChat);
            await _chatRepository.AddAsync(inactiveChat);
            await _unitOfWork.CommitAsync();


            await CreateParticipantAsync(activeChat, user);
            await CreateParticipantAsync(inactiveChat, user);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.GetActivesUserChatsByAsync(user.Id);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(activeChat.Id, result.First().Id);
            Assert.Equal(ChatStatus.Active, result.First().Status);
        }

        [Fact]
        public async Task GetUserChatsByLanguageAsync_ShouldReturnUserChatsWithMatchingLanguage()
        {
            var user = await CreateUserAsync();
            var language = Language.GetByCode("pt");
            var chatWithPrimary = CreateChat(primaryLanguage: language);
            var chatWithSecondary = CreateChat(secondaryLanguage: language);
            var chatWithoutLanguage = CreateChat();

            await _chatRepository.AddAsync(chatWithPrimary);
            await _chatRepository.AddAsync(chatWithSecondary);
            await _chatRepository.AddAsync(chatWithoutLanguage);

            await CreateParticipantAsync(chatWithPrimary, user);
            await CreateParticipantAsync(chatWithSecondary, user);
            await CreateParticipantAsync(chatWithoutLanguage, user);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.GetUserChatsByLanguageAsync(user.Id, language);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == chatWithPrimary.Id);
            Assert.Contains(result, c => c.Id == chatWithSecondary.Id);
        }

        [Fact]
        public async Task AddAsync_ShouldCreateAndReturnChat()
        {
            var chat = CreateChat();

            var result = await _chatRepository.AddAsync(chat);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var savedChat = await _chatRepository.GetByIdAsync(result.Id);
            Assert.NotNull(savedChat);
            Assert.Equal(chat.PrimaryLanguage, savedChat.PrimaryLanguage);
            Assert.Equal(chat.SecondaryLanguage, savedChat.SecondaryLanguage);
        }

       
        [Fact]
        public async Task ExistsAsync_WithExistingChat_ShouldReturnTrue()
        {
            var chat = CreateChat();
            await _chatRepository.AddAsync(chat);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.ExistsAsync(chat.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingChat_ShouldReturnFalse()
        {
            var result = await _chatRepository.ExistsAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }
} 