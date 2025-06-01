using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.Repositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        public ChatRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Chat>> GetActiveChatsAsync()
        {
            return await _dbSet
                .Where(c => c.Status == ChatStatus.Active)
                .AsNoTracking()
                 .ToListAsync();

        }

        public async Task<IEnumerable<Chat>> GetChatsByLanguageAsync(Language language)
        {
            return await _dbSet
                .Where(c => c.PrimaryLanguage == language || c.SecondaryLanguage == language)
                .AsNoTracking()
                .ToListAsync();
        }
    }
} 