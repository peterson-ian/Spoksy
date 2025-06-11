using Spoksy.Application.Commands.Users.UpdateUser;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Application.Commons.Results;
using Spoksy.Test.Infrastructure;
using Spoksy.Application.Responses;

namespace Spoksy.Test.Application.Commands.Users
{
    public class UpdateUserCommandTests : IntegrationTestBase
    {
        private readonly IUpdateUserCommandHandler _handler;
        private readonly IUserRepository _userRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private User _existingUser;

        public UpdateUserCommandTests() : base()
        {
            _userRepository = new UserRepository(_context);
            _userLanguageRepository = new UserLanguageRepository(_context);

            _handler = new UpdateUserCommandHandler(
                _userRepository,
                _userLanguageRepository,
                _unitOfWork
            );
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _existingUser = new User(
                "Original Name",
                "test@test.com",
                DateTime.UtcNow.AddYears(-22),
                Country.GetByCode("US"),
                "test_identity_id"
            );

            var userLanguage = new UserLanguage(_existingUser.Id, Language.Portuguese, ProficiencyLevel.Native);
            
            await _userRepository.AddAsync(_existingUser);
            await _userLanguageRepository.AddAsync(userLanguage);
            await _unitOfWork.CommitAsync();
        }

        [Fact]
        public async Task Handle_WithValidNameUpdate_ShouldUpdateUser()
        {
            var command = new UpdateUserCommand
            {
                Name = "Updated Name"
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.Name, result.Value.Name);

            var updatedUser = await _userRepository.GetByIdAsync(_existingUser.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(command.Name, updatedUser.Name);
            Assert.Equal(_existingUser.Email, updatedUser.Email); 
        }

        [Fact]
        public async Task Handle_WithValidCountryUpdate_ShouldUpdateUser()
        {
            var command = new UpdateUserCommand
            {
                CountryCode = "BR"
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.CountryCode, result.Value.CurrentCountry.Code);

            var updatedUser = await _userRepository.GetByIdAsync(_existingUser.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(command.CountryCode, updatedUser.CurrentCountry.Code);
            Assert.Equal(_existingUser.Name, updatedUser.Name); 
        }

        [Fact]
        public async Task Handle_WithBothNameAndCountryUpdate_ShouldUpdateUser()
        {
            var command = new UpdateUserCommand
            {
                Name = "Updated Name",
                CountryCode = "US"
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.Name, result.Value.Name);
            Assert.Equal(command.CountryCode, result.Value.CurrentCountry.Code);

            var updatedUser = await _userRepository.GetByIdAsync(_existingUser.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(command.Name, updatedUser.Name);
            Assert.Equal(command.CountryCode, updatedUser.CurrentCountry.Code);
        }

        [Fact]
        public async Task Handle_WithInvalidUserId_ShouldReturnNotFound()
        {
            var invalidId = Guid.NewGuid();
            var command = new UpdateUserCommand
            {
                Name = "Updated Name"
            };

            var result = await _handler.Handle(invalidId, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<UserDetailsResponse>);
            Assert.Contains("User not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithInvalidCountryCode_ShouldReturnValidationError()
        {
            var command = new UpdateUserCommand
            {
                CountryCode = "XX" 
            };

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.False(result.IsSuccess);
            Assert.True(result is NotFoundResult<UserDetailsResponse>);
            Assert.Contains("Country XX not found", result.Errors);

            var unchangedUser = await _userRepository.GetByIdAsync(_existingUser.Id);
            Assert.NotNull(unchangedUser);
            Assert.Equal(_existingUser.CurrentCountry.Code, unchangedUser.CurrentCountry.Code);
        }

        [Fact]
        public async Task Handle_WithEmptyCommand_ShouldNotModifyUser()
        {
            var command = new UpdateUserCommand();

            var result = await _handler.Handle(_existingUser.Id, command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_existingUser.Name, result.Value.Name);
            Assert.Equal(_existingUser.CurrentCountry.Code, result.Value.CurrentCountry.Code);

            var unchangedUser = await _userRepository.GetByIdAsync(_existingUser.Id);
            Assert.NotNull(unchangedUser);
            Assert.Equal(_existingUser.Name, unchangedUser.Name);
            Assert.Equal(_existingUser.CurrentCountry.Code, unchangedUser.CurrentCountry.Code);
        }
    }
} 