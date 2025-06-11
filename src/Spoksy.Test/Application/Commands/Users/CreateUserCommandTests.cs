using Moq;
using Spoksy.Application.Commands.Users.CreateUser;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Application.Commons.Results;
using Spoksy.Test.Infrastructure;
using Spoksy.Application.Responses;
using Spoksy.Domain.Exceptions;
using System.Runtime.Intrinsics.Arm;

namespace Spoksy.Test.Application.Commands.Users
{
    public class CreateUserCommandTests : IntegrationTestBase
    {
        private readonly ICreateUserCommandHandler _handler;
        private readonly IUserRepository _userRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly Mock<IIdentityProviderIntegration> _identityProviderMock;
        private readonly IUserLanguageValidationService _userLanguageValidationService;

        public CreateUserCommandTests() : base()
        {
            _userRepository = new UserRepository(_context);
            _userLanguageRepository = new UserLanguageRepository(_context);
            _identityProviderMock = new Mock<IIdentityProviderIntegration>();
            _userLanguageValidationService = new UserLanguageValidationService(_userLanguageRepository);

            _identityProviderMock.Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync("test_identity_id");

            _handler = new CreateUserCommandHandler(
                _userRepository,
                _userLanguageRepository,
                _unitOfWork,
                _userLanguageValidationService,
                _identityProviderMock.Object
            );
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateUser()
        {
            var command = new CreateUserCommand
            {
                Name = "Test User",
                Email = "test@test.com",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                CountryCode = "BR",
                Passsword = "Test@123",
                Languages = new List<UserLanguageDto>
                {
                    new() { LanguageCode = "pt", ProficiencyLevel = ProficiencyLevel.Native },
                    new() { LanguageCode = "en", ProficiencyLevel = ProficiencyLevel.Intermediate }
                }
            };

            var result = await _handler.Handle(command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.Name, result.Value.Name);
            Assert.Equal(command.Email, result.Value.Email);
            Assert.Equal(2, result.Value.Languages.Count);

            var user = await _userRepository.GetByEmailAsync(command.Email);
            Assert.NotNull(user);
            Assert.Equal(command.Name, user.Name);

            var languages = await _userLanguageRepository.GetUserLanguagesAsync(user.Id);
            Assert.Equal(2, languages.Count());

            _identityProviderMock.Verify(x => x.CreateUserAsync(
                command.Name,
                command.Email,
                true,
                command.Passsword
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_WithDuplicateEmail_ShouldReturnConflict()
        {
            var command = new CreateUserCommand
            {
                Name = "Test User",
                Email = "duplicate@test.com",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                CountryCode = "BR",
                Passsword = "Test@123",
                Languages = new List<UserLanguageDto>
                {
                    new() { LanguageCode = "pt", ProficiencyLevel = ProficiencyLevel.Native },
                    new() { LanguageCode = "en", ProficiencyLevel = ProficiencyLevel.Intermediate }
                }
            };

            await _handler.Handle(command);

            var result = await _handler.Handle(command);

            Assert.False(result.IsSuccess);
            Assert.True(result is ConflictResult<UserDetailsResponse>);
            Assert.Contains("Email already exists", result.Errors);
        }

        [Fact]
        public async Task Handle_WithInvalidCountry_ShouldReturnNotFoundResult()
        {
            var command = new CreateUserCommand
            {
                Name = "Test User",
                Email = "test@test.com",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                CountryCode = "XX", 
                Passsword = "Test@123",
                Languages = new List<UserLanguageDto>
                {
                    new() { LanguageCode = "pt", ProficiencyLevel = ProficiencyLevel.Native },
                    new() { LanguageCode = "en", ProficiencyLevel = ProficiencyLevel.Intermediate }
                }
            };

            var result = await _handler.Handle(command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<UserDetailsResponse>);
            Assert.Contains("Country XX not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithoutNativeLanguage_ShouldReturnError()
        {
            var command = new CreateUserCommand
            {
                Name = "Test User",
                Email = "test@test.com",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                CountryCode = "BR",
                Passsword = "Test@123",
                Languages = new List<UserLanguageDto>
                {
                    new() { LanguageCode = "en", ProficiencyLevel = ProficiencyLevel.Intermediate },
                    new() { LanguageCode = "es", ProficiencyLevel = ProficiencyLevel.Beginner }
                }
            };

            var result = await Assert.ThrowsAsync<DomainException>(async () => await _handler.Handle(command));

            Assert.Contains("At least one native language is required.", result.Message);
        }

        [Fact]
        public async Task Handle_WithoutNonNativeLanguage_ShouldThrowException()
        {
            var command = new CreateUserCommand
            {
                Name = "Test User",
                Email = "test@test.com",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                CountryCode = "BR",
                Passsword = "Test@123",
                Languages = new List<UserLanguageDto>
                {
                    new() { LanguageCode = "pt", ProficiencyLevel = ProficiencyLevel.Native },
                    new() { LanguageCode = "en", ProficiencyLevel = ProficiencyLevel.Native }
                }
            };

            var result = await Assert.ThrowsAsync<DomainException>(async () => await _handler.Handle(command));

            Assert.Equal("At least one non-native language is required.", result.Message);
        }

        [Fact]
        public async Task Handle_WhenIdentityProviderFails_ShouldThrowException()
        {
            _identityProviderMock.Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Identity provider error"));

            var command = new CreateUserCommand
            {
                Name = "Test User",
                Email = "test@test.com",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                CountryCode = "BR",
                Passsword = "Test@123",
                Languages = new List<UserLanguageDto>
                {
                    new() { LanguageCode = "pt", ProficiencyLevel = ProficiencyLevel.Native },
                    new() { LanguageCode = "en", ProficiencyLevel = ProficiencyLevel.Intermediate }
                }
            };

            await Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(command));

            var user = await _userRepository.GetByEmailAsync(command.Email);
            Assert.Null(user);
        }
    }
} 