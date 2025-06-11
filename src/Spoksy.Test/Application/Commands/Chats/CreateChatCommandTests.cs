using Spoksy.Application.Commands.Chats.CreateChat;
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

namespace Spoksy.Test.Application.Commands.Chats
{
    public class CreateChatCommandTests : IntegrationTestBase
    {
        private readonly ICreateChatCommandHandler _handler;
        private readonly IUserRepository _userRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IUserLanguageValidationService _userLanguageValidationService;
        private readonly IKnowledgeExchangeValidationService _knowledgeExchangeValidationService;
        private User _existingFirstUser;
        private User _existingSecondUser;
        private List<UserLanguage> _existingFirstUserLanguages = new List<UserLanguage>();
        private List<UserLanguage> _existingSecondUserLanguages = new List<UserLanguage>();

        public CreateChatCommandTests() : base()
        {
            _userRepository = new UserRepository(_context);
            _chatParticipantRepository = new ChatParticipantRepository(_context);
            _chatRepository = new ChatRepository(_context);
            _userLanguageRepository = new UserLanguageRepository(_context);
            _userLanguageValidationService = new UserLanguageValidationService(_userLanguageRepository);
            _knowledgeExchangeValidationService = new KnowledgeExchangeValidationService(_userLanguageValidationService, _userLanguageRepository);
            _handler = new CreateChatCommandHandler(
                _userRepository,
                _chatParticipantRepository,
                _chatRepository,
                _unitOfWork,
                _knowledgeExchangeValidationService
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

            _existingFirstUserLanguages = new List<UserLanguage>()
            {
                new UserLanguage(_existingFirstUser.Id, Language.GetByCode("pt"), ProficiencyLevel.Native),
                new UserLanguage(_existingFirstUser.Id, Language.GetByCode("en"), ProficiencyLevel.Beginner),
                new UserLanguage(_existingFirstUser.Id, Language.GetByCode("es"), ProficiencyLevel.Beginner)
            };

            _existingSecondUserLanguages = new List<UserLanguage>()
            {
                new UserLanguage(_existingSecondUser.Id, Language.GetByCode("en"), ProficiencyLevel.Native),
                new UserLanguage(_existingSecondUser.Id, Language.GetByCode("pt"), ProficiencyLevel.Intermediate),
                new UserLanguage(_existingSecondUser.Id, Language.GetByCode("es"), ProficiencyLevel.Intermediate)
            };

            foreach (var language in _existingFirstUserLanguages)
            {
                await _userLanguageRepository.AddAsync(language);
            }

            foreach (var language in _existingSecondUserLanguages)
            {
                await _userLanguageRepository.AddAsync(language);
            }

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateChat()
        {
            var command = new CreateChatCommand
            {
                FirstParticipant = _existingFirstUser.Id,
                SecondParticipant = _existingSecondUser.Id,
                PrimaryLanguageCode = "pt",
                SecondaryLanguageCode = "en"
            };

            var result = await _handler.Handle(_existingFirstUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("pt", result.Value.PrimaryLanguage.Code);
            Assert.Equal("en", result.Value.SecondaryLanguage.Code);
            Assert.Equal(ChatStatus.Active, result.Value.Status);
        }

        [Fact]
        public async Task Handle_WithInvalidUserId_ShouldNotFoundResult()
        {
            var command = new CreateChatCommand
            {
                FirstParticipant = Guid.NewGuid(),
                SecondParticipant = _existingSecondUser.Id,
                PrimaryLanguageCode = "pt",
                SecondaryLanguageCode = "en"
            };

            var result = await _handler.Handle(_existingFirstUser.Id, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<ChatIdentificatorResponse>);
            Assert.Contains($"User {command.FirstParticipant} not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithInvalidExchange_ShouldDomainException()
        {
            var command = new CreateChatCommand
            {
                FirstParticipant = _existingFirstUser.Id,
                SecondParticipant = _existingSecondUser.Id,
                PrimaryLanguageCode = "es",
                SecondaryLanguageCode = "en"
            };

            var exception = await Assert.ThrowsAsync<DomainException>(
                async () =>  await _handler.Handle(_existingFirstUser.Id, command)
            );

            Assert.Equal("Users do not have complementary proficiency levels for knowledge exchange", exception.Message);
        }

        [Fact]
        public async Task Handle_WithInvalidLanguage_ShouldNotFoundResult()
        {
            var command = new CreateChatCommand
            {
                FirstParticipant = _existingSecondUser.Id,
                SecondParticipant = _existingSecondUser.Id,
                PrimaryLanguageCode = "po",
                SecondaryLanguageCode = "en"
            };

            var result = await _handler.Handle(_existingFirstUser.Id, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<ChatIdentificatorResponse>);
            Assert.Contains($"Language po not found", result.Errors);
        }

    }
}
