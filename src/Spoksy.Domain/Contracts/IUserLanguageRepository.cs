using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Contracts
{
    public interface IUserLanguageRepository : IGenericRepository<UserLanguage>
    {
        Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(Guid userId);
        Task<bool> HasLanguageAsync(Guid userId, Language language);
        Task<int> CountNativeLanguages(Guid userId);
        Task<int> CountNonNativeLanguages(Guid userId);
        Task<bool> HasNativeLanguage(Guid userId);
        Task<bool> HasNonNativeLanguage(Guid userId);
    }
} 