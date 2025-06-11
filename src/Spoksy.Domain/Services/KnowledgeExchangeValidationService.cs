using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Services
{
    public class KnowledgeExchangeValidationService : IKnowledgeExchangeValidationService
    {
        private readonly IUserLanguageValidationService _userLanguageValidationService;
        private readonly IUserLanguageRepository _userLanguageRepository;

        public KnowledgeExchangeValidationService(IUserLanguageValidationService userLanguageValidationService, IUserLanguageRepository userLanguageRepository)
        {
            _userLanguageValidationService = userLanguageValidationService;
            _userLanguageRepository = userLanguageRepository;
        }

        public async Task EnsureKnowledgeExchangeAsync(
            Guid firstUserId,
            Guid secondUserId,
            Language languageA,
            Language languageB)
        {
            if (firstUserId == Guid.Empty)
                throw new DomainException("First user ID cannot be empty");

            if (secondUserId == Guid.Empty)
                throw new DomainException("Second user ID cannot be empty");

            if (firstUserId == secondUserId)
                throw new DomainException("Users must be different for knowledge exchange");

            if (languageA == null)
                throw new DomainException("Language A cannot be null");

            if (languageB == null)
                throw new DomainException("Language B cannot be null");

            if (languageA == languageB)
                throw new DomainException("Languages must be different for knowledge exchange");

            await _userLanguageValidationService.EnsureUserLanguageRequirements(firstUserId);
            await _userLanguageValidationService.EnsureUserLanguageRequirements(secondUserId);
            
            var firstUserLanguages = await _userLanguageRepository.GetUserLanguagesAsync(firstUserId);
            var secondUserLanguages = await _userLanguageRepository.GetUserLanguagesAsync(secondUserId);

            var firstUserLangA = firstUserLanguages.FirstOrDefault(ul => ul.Language == languageA);
            var firstUserLangB = firstUserLanguages.FirstOrDefault(ul => ul.Language == languageB);
            var secondUserLangA = secondUserLanguages.FirstOrDefault(ul => ul.Language == languageA);
            var secondUserLangB = secondUserLanguages.FirstOrDefault(ul => ul.Language == languageB);

            if (firstUserLangA == null || firstUserLangB == null)
                throw new DomainException($"User {firstUserId} must have both selected languages");

            if (secondUserLangA == null || secondUserLangB == null)
                throw new DomainException($"User {secondUserId} must have both selected languages");

            bool canFirstUserTeachLangA = firstUserLangA.ProficiencyLevel >= secondUserLangA.ProficiencyLevel;
            bool canFirstUserLearnLangB = firstUserLangB.ProficiencyLevel <= secondUserLangB.ProficiencyLevel;

            bool canFirstUserTeachLangB = firstUserLangB.ProficiencyLevel >= secondUserLangB.ProficiencyLevel;
            bool canFirstUserLearnLangA = firstUserLangA.ProficiencyLevel <= secondUserLangA.ProficiencyLevel;

            bool hasValidExchangeInLangAtoB = canFirstUserTeachLangA && canFirstUserLearnLangB;
            bool hasValidExchangeInLangBtoA = canFirstUserTeachLangB && canFirstUserLearnLangA;

            if (!hasValidExchangeInLangAtoB && !hasValidExchangeInLangBtoA)
                throw new DomainException("Users do not have complementary proficiency levels for knowledge exchange");
        }

    }
} 