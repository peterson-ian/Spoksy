using Moq;
using Spoksy.Application.Commands.UserLanguages.CreateUserLanguage;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Application.Commons.Results;
using Spoksy.Test.Infrastructure;
using Spoksy.Application.Responses;

namespace Spoksy.Test.Application.Commands.UserLanguages
{
    public class CreateUserLanguageCommandTests : IntegrationTestBase
    {
        private readonly ICreateUserLanguageCommandHandler _handler;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly UserLanguageValidationService _userLanguageValidationService;
        private User _existingUser;

        public CreateUserLanguageCommandTests() : base()
        {
            _userLanguageRepository = new UserLanguageRepository(_context);
            _userLanguageValidationService = new UserLanguageValidationService(_userLanguageRepository);

            _handler = new CreateUserLanguageCommandHandler(
                _userLanguageRepository,
                _unitOfWork,
                _userLanguageValidationService
            );
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _existingUser = new User(
                "Test User",
                "test@test.com",
                DateTime.UtcNow.AddYears(-20),
                Country.GetByCode("BR"),
                "test_identity_id"
            );

            await _context.Users.AddAsync(_existingUser);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateUserLanguage()
        {
            var command = new CreateUserLanguageCommand
            {
                LanguageCode = "en",
                ProficiencyLevel = ProficiencyLevel.Intermediate
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.LanguageCode, result.Value.Code);
            Assert.Equal(command.ProficiencyLevel, result.Value.ProficiencyLevel);

            var userLanguage = await _userLanguageRepository.GetUserLanguagesAsync(_existingUser.Id);
            Assert.Single(userLanguage);
            Assert.Equal(command.LanguageCode, userLanguage.First().Language.Code);
            Assert.Equal(command.ProficiencyLevel, userLanguage.First().ProficiencyLevel);
        }

        [Fact]
        public async Task Handle_WithInvalidLanguageCode_ShouldReturnValidationError()
        {
            var command = new CreateUserLanguageCommand
            {
                LanguageCode = "xx",
                ProficiencyLevel = ProficiencyLevel.Intermediate
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is ValidationResult<UserLanguageReponse>);
            Assert.Contains($"Language {command.LanguageCode} not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithDuplicateLanguage_ShouldReturnValidationError()
        {
            var command = new CreateUserLanguageCommand
            {
                LanguageCode = "en",
                ProficiencyLevel = ProficiencyLevel.Intermediate
            };

            await _handler.Handle(_existingUser.Id, command);
            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is ValidationResult<UserLanguageReponse>);
            Assert.Contains($"Language English(en) is already registered for this user", result.Errors);
        }
    }
} 