using Spoksy.Domain.Entities;

namespace Spoksy.Domain.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdentityProviderIdAsync(string identityProviderId);
        Task<bool> IsEmailUniqueAsync(string email);
    }
} 