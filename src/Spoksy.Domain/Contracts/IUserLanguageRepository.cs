using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Contracts
{
    public interface IUserLanguageRepository 
    {
        Task<UserLanguage?> GetByIdAsync(Guid id, Guid userId);
        Task<UserLanguage> AddAsync(UserLanguage entity);
        Task<UserLanguage> UpdateAsync(UserLanguage entity);
        Task DeleteAsync(Guid id, Guid userId);
        Task<bool> ExistsAsync(Guid id, Guid userId);
        Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(Guid userId);
        Task<bool> HasLanguageAsync(Guid userId, Language language);
        Task<int> CountNativeLanguages(Guid userId);
        Task<int> CountNonNativeLanguages(Guid userId);
        Task<bool> HasNativeLanguage(Guid userId);
        Task<bool> HasNonNativeLanguage(Guid userId);
    }
} 