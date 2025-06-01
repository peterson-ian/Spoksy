using Spoksy.Domain.Entities;

namespace Spoksy.Domain.Contracts
{
    public interface IChatParticipantRepository : IGenericRepository<ChatParticipant>
    {
        Task<IEnumerable<ChatParticipant>> GetParticipantsByChatAsync(Guid chatId);
        Task<bool> HasParticipantInChatAsync(Guid chatId, Guid userId);
        Task<ChatParticipant?> GetParticipantAsync(Guid chatId, Guid userId);
    }
} 