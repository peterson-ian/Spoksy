using Spoksy.Application.Commands.UserLanguages.UpdateUserLanguage;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Application.Commons.Results;
using Spoksy.Test.Infrastructure;
using Spoksy.Application.Responses;

namespace Spoksy.Test.Application.Commands.UserLanguages
{
    public class UpdateUserLanguageCommandTests : IntegrationTestBase
    {
        private readonly IUpdateUserLanguageCommandHandler _handler;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private User _existingUser;
        private UserLanguage _existingUserLanguage;

        public UpdateUserLanguageCommandTests() : base()
        {
            _userLanguageRepository = new UserLanguageRepository(_context);

            _handler = new UpdateUserLanguageCommandHandler(
                _userLanguageRepository,
                _unitOfWork
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

            _existingUserLanguage = new UserLanguage(
                _existingUser.Id,
                Language.English,
                ProficiencyLevel.Beginner
            );

            await _context.UserLanguages.AddAsync(_existingUserLanguage);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldUpdateUserLanguage()
        {
            var command = new UpdateUserLanguageCommand
            {
                Id = _existingUserLanguage.Id,
                ProficiencyLevel = ProficiencyLevel.Advanced
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.ProficiencyLevel, result.Value.ProficiencyLevel);

            var updatedLanguage = await _userLanguageRepository.GetByIdAsync(_existingUserLanguage.Id, _existingUser.Id);
            Assert.NotNull(updatedLanguage);
            Assert.Equal(command.ProficiencyLevel, updatedLanguage.ProficiencyLevel);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldReturnValidationError()
        {
            var command = new UpdateUserLanguageCommand
            {
                Id = Guid.NewGuid(),
                ProficiencyLevel = ProficiencyLevel.Advanced
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<UserLanguageResponse>);
            Assert.Contains("User language not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithDifferentUserId_ShouldReturnValidationError()
        {
            var command = new UpdateUserLanguageCommand
            {
                Id = _existingUserLanguage.Id,
                ProficiencyLevel = ProficiencyLevel.Advanced
            };

            var result = await _handler.Handle(Guid.NewGuid(), command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<UserLanguageResponse>);
            Assert.Contains("User language not found", result.Errors);
        }
    }
} 