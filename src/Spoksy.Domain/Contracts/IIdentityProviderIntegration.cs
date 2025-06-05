using Spoksy.Domain.Entities;

namespace Spoksy.Domain.Contracts
{
    public interface IIdentityProviderIntegration
    {
        Task<string?> CreateUserAsync(string name, string email, bool isActive, string password);
        Task<bool> UpdateUserAsync(string userId, User user);
        Task<bool> ResetPasswordAsync(string userId, string newPassword);
        Task<bool> SetUserEnabledAsync(string userId, bool enabled);
        Task<bool> UpdateUserEmailAsync(string userId, string newEmail);
        Task<bool> DeleteUserAsync(string userId);
    }
}
