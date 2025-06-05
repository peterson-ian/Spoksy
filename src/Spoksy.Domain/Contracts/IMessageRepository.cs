using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spoksy.Domain.Entities;

namespace Spoksy.Domain.Contracts
{
    public interface IMessageRepository
    {
        Task<Message?> GetByIdAsync(Guid id);
        Task<Message?> GetByIdForOwnerAsync(Guid messageId, Guid userId);
        Task<IEnumerable<Message>> GetByChatIdAsync(Guid chatId);
        Task<IEnumerable<Message>> GetByUserIdAsync(Guid userId);
        Task<Message> AddAsync(Message message);
        Task<Message> UpdateAsync(Message message);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsUserMessageOwnerAsync(Guid messageId, Guid userId);
        Task<int> GetChatMessageCountAsync(Guid chatId);
    }
} 