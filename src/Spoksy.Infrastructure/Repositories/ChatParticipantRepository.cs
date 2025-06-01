using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Contracts;

namespace Spoksy.Infrastructure.Repositories
{
    public class ChatParticipantRepository : GenericRepository<ChatParticipant>, IChatParticipantRepository
    {
        public ChatParticipantRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChatParticipant>> GetParticipantsByChatAsync(Guid chatId)
        {
            return await _dbSet
                .Where(cp => cp.ChatId == chatId)
                .ToListAsync();
        }

        public async Task<bool> HasParticipantInChatAsync(Guid chatId, Guid userId)
        {
            return await _dbSet.AnyAsync(cp => 
                cp.ChatId == chatId && 
                cp.UserId == userId && 
                cp.LeaveAt == null);
        }

        public async Task<ChatParticipant?> GetParticipantAsync(Guid chatId, Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(cp => 
                cp.ChatId == chatId && 
                cp.UserId == userId);
        }

    }
} 