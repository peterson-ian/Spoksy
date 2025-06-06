using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;

namespace Spoksy.Infrastructure.Repositories
{
    public class ChatParticipantRepository : IChatParticipantRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<ChatParticipant> _dbSet;

        public ChatParticipantRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<ChatParticipant>();
        }

        public async Task<ChatParticipant?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ChatParticipant>> GetByChatIdAsync(Guid chatId)
        {
            return await _dbSet.Where(p => p.ChatId == chatId).ToListAsync();
        }

        public async Task<IEnumerable<ChatParticipant>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<ChatParticipant> AddAsync(ChatParticipant participant)
        {
            await _dbSet.AddAsync(participant);
            return participant;
        }

        public async Task<ChatParticipant> UpdateAsync(ChatParticipant participant)
        {
            _dbSet.Update(participant);
            return participant;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId)
        {
            return await _dbSet.AnyAsync(p => p.ChatId == chatId && p.UserId == userId);
        }

        public async Task<int> GetParticipantCountAsync(Guid chatId)
        {
            return await _dbSet.CountAsync(p => p.ChatId == chatId);
        }
    }
} 