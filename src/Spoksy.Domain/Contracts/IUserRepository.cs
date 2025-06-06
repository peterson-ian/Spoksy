using Spoksy.Domain.Entities;
using System.Linq.Expressions;

namespace Spoksy.Domain.Contracts
{
    public interface IUserRepository 
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User> AddAsync(User entity);
        Task<User> UpdateAsync(User entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdentityProviderIdAsync(string identityProviderId);
        Task<bool> IsEmailUniqueAsync(string email);
    }
} 