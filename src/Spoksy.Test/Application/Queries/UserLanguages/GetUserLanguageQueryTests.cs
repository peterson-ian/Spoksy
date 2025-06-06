using Spoksy.Application.Queries.UserLanguages.GetUserLanguage;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Test.Infrastructure;

namespace Spoksy.Test.Application.Queries.UserLanguages
{
    public class GetUserLanguageQueryTests : IntegrationTestBase
    {
        private readonly IGetUserLanguageQueryHandler _handler;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private User _existingUser;
        private UserLanguage _existingUserLanguage;

        public GetUserLanguageQueryTests() : base()
        {
            _handler = new GetUserLanguageQueryHandler(_dbConnectionFactory);
            _userLanguageRepository = new UserLanguageRepository(_context);
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
                ProficiencyLevel.Intermediate
            );

            await _context.UserLanguages.AddAsync(_existingUserLanguage);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidIds_ShouldReturnUserLanguage()
        {
            var result = await _handler.Handle(_existingUser.Id, _existingUserLanguage.Id);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_existingUserLanguage.Id, result.Value.Id);
            Assert.Equal(_existingUserLanguage.Language.Code, result.Value.Code);
            Assert.Equal(_existingUserLanguage.Language.Name, result.Value.Name);
            Assert.Equal(_existingUserLanguage.ProficiencyLevel, result.Value.ProficiencyLevel);
            Assert.Equal(_existingUserLanguage.StartedLearningOn, result.Value.StartedLearningOn);
        }

        [Fact]
        public async Task Handle_WithInvalidUserId_ShouldReturnNotFound()
        {
            var result = await _handler.Handle(Guid.NewGuid(), _existingUserLanguage.Id);

            Assert.False(result.IsSuccess);
            Assert.Contains("User language not found", result.Errors);
        }

        [Fact]
        public async Task Handle_WithInvalidUserLanguageId_ShouldReturnNotFound()
        {
            var result = await _handler.Handle(_existingUser.Id, Guid.NewGuid());

            Assert.False(result.IsSuccess);
            Assert.Contains("User language not found", result.Errors);
        }
    }
} 