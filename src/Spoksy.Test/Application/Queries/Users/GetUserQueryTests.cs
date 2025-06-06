using Spoksy.Application.Queries.Users.GetUser;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Test.Infrastructure;

namespace Spoksy.Test.Application.Queries.Users
{
    public class GetUserQueryTests : IntegrationTestBase
    {
        private readonly IGetUserQueryHandler _handler;
        private readonly IUserRepository _userRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private User _activeUser;
        private User _inactiveUser;

        public GetUserQueryTests() : base()
        {
            _handler = new GetUserQueryHandler(_dbConnectionFactory);
            _userRepository = new UserRepository(_context);
            _userLanguageRepository = new UserLanguageRepository(_context);
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _activeUser = new User(
                "Active User",
                "active@test.com",
                DateTime.UtcNow.AddYears(-20),
                Country.GetByCode("BR"),
                "active_identity_id"
            );

            var activeUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(_activeUser.Id, Language.Portuguese, ProficiencyLevel.Native),
                new UserLanguage(_activeUser.Id, Language.English, ProficiencyLevel.Intermediate)
            };

            await _userRepository.AddAsync(_activeUser);
            foreach (var language in activeUserLanguages)
            {
                await _userLanguageRepository.AddAsync(language);
            }

            _inactiveUser = new User(
                "Inactive User",
                "inactive@test.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                "inactive_identity_id"
            );
            _inactiveUser.DeactivateUser();

            var inactiveUserLanguage = new UserLanguage(_inactiveUser.Id, Language.English, ProficiencyLevel.Native);

            await _userRepository.AddAsync(_inactiveUser);
            await _userLanguageRepository.AddAsync(inactiveUserLanguage);

            await _unitOfWork.CommitAsync();
        }

        [Fact]
        public async Task Handle_WithValidActiveUserId_ShouldReturnUser()
        {
            var result = await _handler.Handle(_activeUser.Id);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_activeUser.Id, result.Value.Id);
            Assert.Equal(_activeUser.Name, result.Value.Name);
            Assert.Equal(_activeUser.Email, result.Value.Email);
            Assert.Equal(_activeUser.CurrentCountry.Code, result.Value.CurrentCountry.Code);
            Assert.Equal(2, result.Value.Languages.Count);

            Assert.Contains(result.Value.Languages, l => l.Code == "pt" && l.ProficiencyLevel == ProficiencyLevel.Native);
            Assert.Contains(result.Value.Languages, l => l.Code == "en" && l.ProficiencyLevel == ProficiencyLevel.Intermediate);
        }

        [Fact]
        public async Task Handle_WithInactiveUserId_ShouldReturnNotFound()
        {
            var result = await _handler.Handle(_inactiveUser.Id);

            Assert.False(result.IsSuccess);
            Assert.Contains("User not found.", result.Errors);
        }

        [Fact]
        public async Task Handle_WithNonExistentUserId_ShouldReturnNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var result = await _handler.Handle(nonExistentId);

            Assert.False(result.IsSuccess);
            Assert.Contains("User not found.", result.Errors);
        }

    }
} 