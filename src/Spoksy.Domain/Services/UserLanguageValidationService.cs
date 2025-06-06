using Spoksy.Domain.Contracts;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Services
{
    public class UserLanguageValidationService
    {
        private readonly IUserLanguageRepository _repository;

        public UserLanguageValidationService(IUserLanguageRepository repository)
        {
            _repository = repository;
        }

        public async Task EnsureValidLanguagesForUserCreation(List<Language> nativeLanguages, List<Language> nonNativeLanguages)
        {
            if (nativeLanguages == null || nativeLanguages.Where(l => l != null).Count() == 0)
                throw new DomainException("At least one native language is required.");

            if (nonNativeLanguages == null || nonNativeLanguages.Where(l => l != null).Count() == 0)
                throw new DomainException("At least one non-native language is required.");
        }

        public async Task EnsureLanguageCanBeRemoved(Guid userId, Guid languageId)
        {
            var languageToRemove = await _repository.GetByIdAsync(languageId, userId);

            if (languageToRemove == null)
                throw new DomainException("Language not found");

            bool languageToRemoveIsNative = languageToRemove.ProficiencyLevel == ProficiencyLevel.Native;
            int languageCount = languageToRemoveIsNative 
                ? await _repository.CountNativeLanguages(userId) 
                : await _repository.CountNonNativeLanguages(userId);

            if (languageCount < 2)
            {
                string error = languageToRemoveIsNative
                    ? "You must have at least two native languages to remove one of them"
                    : "You must have at least two non-native languages to remove one of them";

                throw new DomainException(error);
            }
        }

        public async Task EnsureUserLanguageRequirements(Guid userId)
        {
            if (!await _repository.HasNativeLanguage(userId))
                throw new DomainException("User must have at least one native language");
            
            if (!await _repository.HasNonNativeLanguage(userId))
                throw new DomainException("User must have at least one language to learn");
        }
    }
}
