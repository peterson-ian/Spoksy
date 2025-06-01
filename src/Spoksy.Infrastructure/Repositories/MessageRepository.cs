using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;

namespace Spoksy.Infrastructure.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(DbContext context) : base(context)
        {
        }

        public async Task<Message?> GetLastMessageFromChatAsync(Guid chatId)
        {
            return await _dbSet
                .Where(ul => ul.ChatId == chatId)
                .OrderBy(ul => ul.SentAt)
                .LastOrDefaultAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatAsync(Guid chatId)
        {
            return await _dbSet
                .Where(ul => ul.ChatId == chatId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(ul => ul.SenderId == userId)
                .ToListAsync();
        }

        public async Task<int> GetUnreadMessagesCountAsync(Guid chatId, Guid userId)
        {
            return await _dbSet
                .Where(ul => ul.ChatId == chatId && ul.SenderId == userId && !ul.IsRead)
                .CountAsync();
        }

    }
}
