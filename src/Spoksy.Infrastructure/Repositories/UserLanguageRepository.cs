using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.Repositories
{
    public class UserLanguageRepository : IUserLanguageRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<UserLanguage> _dbSet;

        public UserLanguageRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<UserLanguage>();
        }

        public async Task<UserLanguage> AddAsync(UserLanguage entity)
        {
            _dbSet.Add(entity);
            return entity;
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

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var entity = await GetByIdAsync(id, userId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, Guid userId)
        {
            return await _dbSet.AnyAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task<UserLanguage?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
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

        public async Task<UserLanguage> UpdateAsync(UserLanguage entity)
        {
            _dbSet.Update(entity);
            return entity;
        }
    }
} 