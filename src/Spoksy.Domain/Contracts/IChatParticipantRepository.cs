using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spoksy.Domain.Entities;

namespace Spoksy.Domain.Contracts
{
    public interface IChatParticipantRepository
    {
        Task<ChatParticipant?> GetByIdAsync(Guid id);
        Task<IEnumerable<ChatParticipant>> GetByChatIdAsync(Guid chatId);
        Task<IEnumerable<ChatParticipant>> GetByUserIdAsync(Guid userId);
        Task<ChatParticipant> AddAsync(ChatParticipant participant);
        Task<ChatParticipant> UpdateAsync(ChatParticipant participant);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsUserInChatAsync(Guid chatId, Guid userId);
        Task<int> GetParticipantCountAsync(Guid chatId);
    }
} 