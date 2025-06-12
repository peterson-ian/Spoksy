using Spoksy.Application.Queries.Chats.GetAllChat;
using Spoksy.Application.Queries.Messages.GetAllMessage;
using Spoksy.Application.Queries.Messages.GetMessage;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Test.Infrastructure;

namespace Spoksy.Test.Application.Queries.Messages
{
    public class GetlMessageQueryTests : IntegrationTestBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatValidationService _chatValidationService;
        private readonly IChatRepository _chatRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IGetMessageQueryHandler _handler;
        private Chat _existingChat;
        private User _existingFirstUser;
        private User _existingSecondUser;
        private List<Message> _existingMessages;

        public GetlMessageQueryTests() : base()
        {
            _userRepository = new UserRepository(_context);
            _messageRepository = new MessageRepository(_context);
            _chatParticipantRepository = new ChatParticipantRepository(_context);
            _chatRepository = new ChatRepository(_context);
            _chatValidationService = new ChatValidationService(_chatRepository, _chatParticipantRepository);
            _userLanguageRepository = new UserLanguageRepository(_context);
            _handler = new GetMessageQueryHandler(_dbConnectionFactory, _chatValidationService);
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

            _existingMessages = new List<Message>
            {
                new Message( _existingChat.Id, _existingFirstUser.Id, "Hello!", Language.English),
                new Message( _existingChat.Id, _existingSecondUser.Id, "Olá!", Language.Portuguese),
                new Message( _existingChat.Id, _existingFirstUser.Id, "How are you?", Language.English),
                new Message( _existingChat.Id, _existingSecondUser.Id, "Estou bem, e você?", Language.Portuguese)
            };
    
            foreach (var message in _existingMessages)
            {
                await _messageRepository.AddAsync(message);
            }

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnChat()
        {
            var result = await _handler.Handle(_existingFirstUser.Id, _existingChat.Id, _existingMessages.First().Id);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_existingMessages.First().Id, _existingMessages.First().Id);
            Assert.Equal(_existingMessages.First().Content, _existingMessages.First().Content);

        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldReturnNotFoundResult()
        {
            var result = await _handler.Handle(_existingFirstUser.Id, _existingChat.Id, Guid.NewGuid());

            Assert.False(result.IsSuccess);
            Assert.Contains("Message not found", result.Errors);

        }
    }
}
