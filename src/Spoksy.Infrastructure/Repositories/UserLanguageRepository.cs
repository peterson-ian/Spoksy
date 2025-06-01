using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.Repositories
{
    public class UserLanguageRepository : GenericRepository<UserLanguage>, IUserLanguageRepository
    {
        public UserLanguageRepository(DbContext context) : base(context)
        {
        }

        public async Task<int> CountNativeLanguages(Guid userId)
        {
            return await _dbSet
                .Where(ul => ul.UserId == userId && ul.ProficiencyLevel == ProficiencyLevel.Native)
                .CountAsync();
        }

        public async Task<int> CountNonNativeLanguages(Guid userId)
        {
            return await _dbSet
                .Where(ul => ul.UserId == userId && ul.ProficiencyLevel != ProficiencyLevel.Native)
                .CountAsync();
        }

        public async Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(Guid userId)
        {
            return await _dbSet
                .Where(ul => ul.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> HasLanguageAsync(Guid userId, Language language)
        {
            return await _dbSet.AnyAsync(ul => 
                ul.UserId == userId && 
                ul.Language == language);
        }

        public async Task<bool> HasNativeLanguage(Guid userId)
        {
            return await _dbSet
                .Where(ul => ul.UserId == userId && ul.ProficiencyLevel == ProficiencyLevel.Native)
                .AnyAsync();
        }

        public async Task<bool> HasNonNativeLanguage(Guid userId)
        {
            return await _dbSet
                .Where(ul => ul.UserId == userId && ul.ProficiencyLevel != ProficiencyLevel.Native)
                .AnyAsync();
        }
    }
} 