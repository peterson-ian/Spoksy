using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Contracts
{
    public interface IUserLanguageValidationService
    {
        Task EnsureValidLanguagesForUserCreation(List<Language> nativeLanguages, List<Language> nonNativeLanguages);
        Task EnsureLanguageCanBeRemoved(Guid userId, Guid languageId);
        Task EnsureUserLanguageRequirements(Guid userId);
    }
}
