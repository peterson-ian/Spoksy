using Spoksy.Domain.Entities;

namespace Spoksy.Domain.Contracts
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByChatAsync(Guid chatId);
        Task<IEnumerable<Message>> GetMessagesByUserAsync(Guid userId);
        Task<int> GetUnreadMessagesCountAsync(Guid chatId, Guid userId);
        Task<Message?> GetLastMessageFromChatAsync(Guid chatId);
    }
} 