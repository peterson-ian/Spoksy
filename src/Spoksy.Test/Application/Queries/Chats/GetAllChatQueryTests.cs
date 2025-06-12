using Spoksy.Application.Commands.Chats.CloseChat;
using Spoksy.Application.Queries.Chats.GetAllChat;
using Spoksy.Application.Queries.UserLanguages.GetUserLanguage;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Test.Infrastructure;

namespace Spoksy.Test.Application.Queries.Chats
{
    public class GetAllChatQueryTests : IntegrationTestBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IGetAllChatQueryHandler _handler;
        private Chat _existingChat;
        private User _existingFirstUser;
        private User _existingSecondUser;
        private Message _existingMessage;

        public GetAllChatQueryTests() : base()
        {
            _userRepository = new UserRepository(_context);
            _messageRepository = new MessageRepository(_context);
            _chatParticipantRepository = new ChatParticipantRepository(_context);
            _chatRepository = new ChatRepository(_context);
            _userLanguageRepository = new UserLanguageRepository(_context);
            _handler = new GetAllChatQueryHandler(_dbConnectionFactory);
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _existingFirstUser = new User(
                "User One",
                "test-one@test.com",
                DateTime.UtcNow.AddYears(-20),
                Country.GetByCode("BR"),
                "test_identity_id"
            );

            _existingSecondUser = new User(
                "User Two",
                "test-two@test.com",
                DateTime.UtcNow.AddYears(-22),
                Country.GetByCode("US"),
                "test_identity_id"
            );

            await _context.Users.AddAsync(_existingFirstUser);
            await _context.Users.AddAsync(_existingSecondUser);

            var _existingFirstUserLanguages = new List<UserLanguage>()
            {
                new UserLanguage(_existingFirstUser.Id, Language.GetByCode("pt"), ProficiencyLevel.Native),
                new UserLanguage(_existingFirstUser.Id, Language.GetByCode("en"), ProficiencyLevel.Beginner),
            };

            var _existingSecondUserLanguages = new List<UserLanguage>()
            {
                new UserLanguage(_existingSecondUser.Id, Language.GetByCode("en"), ProficiencyLevel.Native),
                new UserLanguage(_existingSecondUser.Id, Language.GetByCode("pt"), ProficiencyLevel.Intermediate),
            };

            foreach (var language in _existingFirstUserLanguages)
            {
                await _userLanguageRepository.AddAsync(language);
            }

            foreach (var language in _existingSecondUserLanguages)
            {
                await _userLanguageRepository.AddAsync(language);
            }

            _existingChat = new Chat(Language.English, Language.Portuguese);
            await _chatRepository.AddAsync(_existingChat);

            await _chatParticipantRepository.AddAsync(new ChatParticipant(_existingFirstUser.Id, _existingChat.Id));
            await _chatParticipantRepository.AddAsync(new ChatParticipant(_existingSecondUser.Id, _existingChat.Id));

            _existingMessage = new Message(
                _existingChat.Id,
                _existingFirstUser.Id,
                "Hello, how are you?"
                );

            await _messageRepository.AddAsync(_existingMessage);

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnChats()
        {
            var result = await _handler.Handle(_existingFirstUser.Id, 1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value.Items);
            Assert.Equal(_existingChat.Id, result.Value.Items.First().Id);
            Assert.Equal(_existingMessage.Content, result.Value.Items.First().LastMessage.Content);
        }
    }
}
