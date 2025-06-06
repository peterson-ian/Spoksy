using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;

namespace Spoksy.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<Message> _dbSet;

        public MessageRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<Message>();
        }

        public async Task<Message?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(Guid chatId)
        {
            return await _dbSet.Where(m => m.ChatId == chatId).OrderByDescending(m => m.SentAt).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(m => m.SenderId == userId).OrderByDescending(m => m.SentAt).ToListAsync();
                
        }

        public async Task<Message> AddAsync(Message message)
        {
            await _dbSet.AddAsync(message);
            return message;
        }

        public async Task<Message> UpdateAsync(Message message)
        {
            _dbSet.Update(message);
            return message;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(m => m.Id == id);
        }

        public async Task<bool> IsUserMessageOwnerAsync(Guid messageId, Guid userId)
        {
            return await _dbSet.AnyAsync(m => m.Id == messageId && m.SenderId == userId);
        }

        public async Task<int> GetChatMessageCountAsync(Guid chatId)
        {
            return await _dbSet.CountAsync(m => m.ChatId == chatId);
        }

        public async Task<Message?> GetByIdForOwnerAsync(Guid messageId, Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId);
        }
    }
}
