using Spoksy.Application.Commands.Chats.CloseChat;
using Spoksy.Application.Commands.Messages.EditMessage;
using Spoksy.Application.Commands.Messages.SendMessage;
using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Test.Infrastructure;

namespace Spoksy.Test.Application.Commands.Messages
{
    public class EditMessageCommandTests : IntegrationTestBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatValidationService _chatValidationService;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IEditMessageCommmandHandler _handler;
        private Message _existingMessage;
        private Chat _existingChat;
        private User _existingFirstUser;
        private User _existingSecondUser;

        public EditMessageCommandTests() : base()
        {
            _userRepository = new UserRepository(_context);
            _messageRepository = new MessageRepository(_context);
            _userLanguageRepository = new UserLanguageRepository(_context);
            _chatParticipantRepository = new ChatParticipantRepository(_context);
            _chatRepository = new ChatRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
            _chatValidationService = new ChatValidationService(_chatRepository, _chatParticipantRepository);
            _handler = new EditMessageCommmandHandler(
                _messageRepository,
                _unitOfWork,
                _chatValidationService
            );
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
                "Initial message content."
            );

            await _messageRepository.AddAsync(_existingMessage);

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldEditMessage()
        {
            var command = new EditMessageCommand
            {
                ChatId = _existingChat.Id,
                MessageId = _existingMessage.Id,
                Content = "Edited message content."
            };

            var result = await _handler.Handle(_existingFirstUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Edited message content.", result.Value.Content);
            Assert.Equal(_existingFirstUser.Id, result.Value.SenderId);
            Assert.NotNull(result.Value.EditAt);
        }

        [Fact]
        public async Task Handle_WithInvalidUserId_ShouldDomainException()
        {
            var userId = Guid.NewGuid();
            var command = new EditMessageCommand
            {
                ChatId = _existingChat.Id,
                MessageId = _existingMessage.Id,
                Content = "Edited message content."
            };

            var exception = await Assert.ThrowsAsync<DomainException>(
               async () => await _handler.Handle(userId, command)
            );

            Assert.Equal($"User with ID {userId} is not a participant of chat with ID {_existingChat.Id}.", exception.Message);
        }

        [Fact]
        public async Task Handle_WithNotOwnerMesssage_ShouldNotFoundResult()
        {
            var command = new EditMessageCommand
            {
                ChatId = _existingChat.Id,
                MessageId = _existingMessage.Id,
                Content = "Edited message content."
            };

            var result = await _handler.Handle(_existingSecondUser.Id, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<MessageResponse>);
            Assert.Contains("Message not found.", result.Errors);
        }

        [Fact]
        public async Task Handle_WithInvalidChatId_ShouldDomainException()
        {
            var chatId = Guid.NewGuid();
            var command = new EditMessageCommand
            {
                ChatId = chatId,
                MessageId = _existingMessage.Id,
                Content = "Edited message content."
            };


            var exception = await Assert.ThrowsAsync<DomainException>(
               async () => await _handler.Handle(_existingFirstUser.Id, command)
            );

            Assert.Equal($"Chat with ID {chatId} not found.", exception.Message);
        }
    }
}
