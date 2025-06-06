using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Contracts
{
    public interface IChatRepository
    {
        Task<Chat?> GetByIdAsync(Guid id);
        Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId);
        Task<IEnumerable<Chat>> GetActivesUserChatsByAsync(Guid userId);
        Task<IEnumerable<Chat>> GetUserChatsByLanguageAsync(Guid userId, Language language);
        Task<Chat> AddAsync(Chat chat);
        Task<Chat> UpdateAsync(Chat chat);
        Task<bool> ExistsAsync(Guid id);
    }
} 