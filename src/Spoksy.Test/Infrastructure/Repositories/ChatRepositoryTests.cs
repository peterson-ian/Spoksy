using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class ChatRepositoryTests : IntegrationTestBase
    {
        private readonly IChatRepository _chatRepository;

        public ChatRepositoryTests() : base()
        {
            _chatRepository = new ChatRepository(_context);
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

        [Fact]
        public async Task GetActiveChatsAsync_ShouldReturnOnlyActiveChats()
        {
            var activeChat1 = CreateChat();
            var activeChat2 = CreateChat();
            var inactiveChat = CreateChat();
            inactiveChat.CloseChat();
            await _chatRepository.AddAsync(activeChat1);
            await _chatRepository.AddAsync(activeChat2);
            await _chatRepository.AddAsync(inactiveChat);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.GetActiveChatsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, chat => Assert.Equal(ChatStatus.Active, chat.Status));
            Assert.Contains(result, c => c.Id == activeChat1.Id);
            Assert.Contains(result, c => c.Id == activeChat2.Id);
        }

        [Fact]
        public async Task GetChatsByLanguageAsync_ShouldReturnChatsWithMatchingLanguage()
        {
            var language = Language.GetByCode("pt");
            var chatWithPrimary = CreateChat(primaryLanguage: language);
            var chatWithSecondary = CreateChat(secondaryLanguage: language);
            var chatWithoutLanguage = CreateChat();
            await _chatRepository.AddAsync(chatWithPrimary);
            await _chatRepository.AddAsync(chatWithSecondary);
            await _chatRepository.AddAsync(chatWithoutLanguage);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.GetChatsByLanguageAsync(language);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == chatWithPrimary.Id);
            Assert.Contains(result, c => c.Id == chatWithSecondary.Id);
        }

        [Fact]
        public async Task GetChatsByLanguageAsync_WithNonExistingLanguage_ShouldReturnEmpty()
        {
            var chat = CreateChat();
            await _chatRepository.AddAsync(chat);
            await _unitOfWork.CommitAsync();

            var result = await _chatRepository.GetChatsByLanguageAsync(Language.GetByCode("ja"));

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
} 