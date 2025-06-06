using Spoksy.Application.Queries.UserLanguages.GetAllUserLanguage;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Test.Infrastructure;

namespace Spoksy.Test.Application.Queries.UserLanguages
{
    public class GetAllUserLanguageQueryTests : IntegrationTestBase
    {
        private readonly IGetAllUserLanguageQueryHandler _handler;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private User _existingUser;
        private List<UserLanguage> _existingUserLanguages;

        public GetAllUserLanguageQueryTests() : base()
        {
            _handler = new GetAllUserLanguageQueryHandler(_dbConnectionFactory);
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

            _existingUserLanguages = new List<UserLanguage>
            {
                new UserLanguage(_existingUser.Id, Language.Portuguese, ProficiencyLevel.Native),
                new UserLanguage(_existingUser.Id, Language.English, ProficiencyLevel.Intermediate),
                new UserLanguage(_existingUser.Id, Language.Spanish, ProficiencyLevel.Beginner)
            };

            foreach (var language in _existingUserLanguages)
            {
                await _context.UserLanguages.AddAsync(language);
            }

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_WithValidUserId_ShouldReturnAllUserLanguages()
        {
            var result = await _handler.Handle(_existingUser.Id);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_existingUserLanguages.Count, result.Value.Count);

            foreach (var expectedLanguage in _existingUserLanguages)
            {
                var actualLanguage = result.Value.FirstOrDefault(l => l.Id == expectedLanguage.Id);
                Assert.NotNull(actualLanguage);
                Assert.Equal(expectedLanguage.Language.Code, actualLanguage.Code);
                Assert.Equal(expectedLanguage.Language.Name, actualLanguage.Name);
                Assert.Equal(expectedLanguage.ProficiencyLevel, actualLanguage.ProficiencyLevel);
                Assert.Equal(expectedLanguage.StartedLearningOn, actualLanguage.StartedLearningOn);
            }
        }

        [Fact]
        public async Task Handle_WithInvalidUserId_ShouldReturnEmptyList()
        {
            var result = await _handler.Handle(Guid.NewGuid());

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

    }
} 