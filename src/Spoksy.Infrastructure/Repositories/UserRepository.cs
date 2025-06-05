using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;
using Polly;

namespace Spoksy.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<User>();
        }

        public async Task<User> AddAsync(User entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(x => x.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.Status == UserStatus.Active);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<User?> GetByIdentityProviderIdAsync(string identityProviderId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.IdentityProviderId == identityProviderId && u.Status == UserStatus.Active);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email && u.Status == UserStatus.Active);
        }

        public async Task<User> UpdateAsync(User entity)
        {
            _dbSet.Update(entity);
            return entity;
        }
    }
}
