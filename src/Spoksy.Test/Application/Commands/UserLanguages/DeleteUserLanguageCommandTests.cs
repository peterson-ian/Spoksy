using Spoksy.Application.Commands.UserLanguages.DeleteUserLanguage;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Application.Commons.Results;
using Spoksy.Test.Infrastructure;
using Spoksy.Domain.Exceptions;

namespace Spoksy.Test.Application.Commands.UserLanguages
{
    public class DeleteUserLanguageCommandTests : IntegrationTestBase
    {
        private readonly IDeleteUserLanguageCommandHandler _handler;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly UserLanguageValidationService _userLanguageValidationService;
        private User _existingUser;
        private UserLanguage _existingUserLanguage;
        private UserLanguage _nativeLanguage;
        private UserLanguage _extraUserLanguage;

        public DeleteUserLanguageCommandTests() : base()
        {
            _userLanguageRepository = new UserLanguageRepository(_context);
            _userLanguageValidationService = new UserLanguageValidationService(_userLanguageRepository);

            _handler = new DeleteUserLanguageCommandHandler(
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

            _nativeLanguage = new UserLanguage(
                _existingUser.Id,
                Language.Portuguese,
                ProficiencyLevel.Native
            );

            _existingUserLanguage = new UserLanguage(
                _existingUser.Id,
                Language.English,
                ProficiencyLevel.Intermediate
            );

            _extraUserLanguage = new UserLanguage(
               _existingUser.Id,
               Language.Spanish,
               ProficiencyLevel.Intermediate
           );


            await _context.UserLanguages.AddAsync(_nativeLanguage);
            await _context.UserLanguages.AddAsync(_existingUserLanguage);
            await _context.UserLanguages.AddAsync(_extraUserLanguage);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldDeleteUserLanguage()
        {
            var result = await _handler.Handle(_existingUser.Id, _extraUserLanguage.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal("Successfully deleted the language", result.Value);

            var deletedLanguage = await _userLanguageRepository.GetByIdAsync(_extraUserLanguage.Id, _existingUser.Id);
            Assert.Null(deletedLanguage);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldReturnValidationError()
        {
            var result = await _handler.Handle(_existingUser.Id, Guid.NewGuid());

            Assert.False(result.IsSuccess);
            Assert.True(result is ValidationResult<string>);
            Assert.Contains("User language not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithDifferentUserId_ShouldReturnValidationError()
        {
            var result = await _handler.Handle(Guid.NewGuid(), _existingUserLanguage.Id);

            Assert.False(result.IsSuccess);
            Assert.True(result is ValidationResult<string>);
            Assert.Contains("User language not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithNativeLanguage_ShouldThrowDomainException()
        {
            var exception = await Assert.ThrowsAsync<DomainException>(
                async () => await _handler.Handle(_existingUser.Id, _nativeLanguage.Id)
            );

            Assert.Equal("You must have at least two native languages to remove one of them", exception.Message);

            var nativeLanguage = await _userLanguageRepository.GetByIdAsync(_nativeLanguage.Id, _existingUser.Id);
            Assert.NotNull(nativeLanguage);
        }

        [Fact]
        public async Task Handle_WithLastNonNativeLanguage_ShouldThrowDomainException()
        {
            _context.UserLanguages.Remove(_nativeLanguage);
            await _context.SaveChangesAsync();

            await _handler.Handle(_existingUser.Id, _extraUserLanguage.Id);

            var exception = await Assert.ThrowsAsync<DomainException>(
                async () => await _handler.Handle(_existingUser.Id, _existingUserLanguage.Id)
            );

            Assert.Equal("You must have at least two non-native languages to remove one of them", exception.Message);

            var language = await _userLanguageRepository.GetByIdAsync(_existingUserLanguage.Id, _existingUser.Id);
            Assert.NotNull(language);
        }
    }
} 