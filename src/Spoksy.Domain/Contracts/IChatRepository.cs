using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Contracts
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<IEnumerable<Chat>> GetActiveChatsAsync();
        Task<IEnumerable<Chat>> GetChatsByLanguageAsync(Language language);
    }
} 