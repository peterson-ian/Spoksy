using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<Chat> _dbSet;

        public ChatRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<Chat>();
        }

        public async Task<Chat?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Chat> AddAsync(Chat chat)
        {
            await _dbSet.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<Chat> UpdateAsync(Chat chat)
        {
            var existingChat = await GetByIdAsync(chat.Id);
            if (existingChat == null)
                return null;

            _context.Entry(existingChat).CurrentValues.SetValues(chat);
            await _context.SaveChangesAsync();
            return existingChat;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Chat>> GetActivesUserChatsByAsync(Guid userId)
        {
            var participants = _context.Set<ChatParticipant>();

            return await _dbSet
                .Where(chat => participants
                    .Any(p => p.ChatId == chat.Id && p.UserId == userId) && chat.Status == ChatStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> GetUserChatsByLanguageAsync(Guid userId, Language language)
        {
            var participants = _context.Set<ChatParticipant>();

            return await _dbSet
                .Where(chat => participants
                    .Any(p => p.ChatId == chat.Id && p.UserId == userId) && (chat.PrimaryLanguage == language || chat.SecondaryLanguage == language) )
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId)
        {
            var participants = _context.Set<ChatParticipant>();

            return await _dbSet
                .Where(chat => participants
                    .Any(p => p.ChatId == chat.Id && p.UserId == userId))
                .AsNoTracking()
                .ToListAsync();
        }
    }
} 