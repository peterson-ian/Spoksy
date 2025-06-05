using System;
using System.Linq;
using System.Threading.Tasks;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql.Replication.PgOutput.Messages;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class MessageRepositoryTests : IntegrationTestBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly DbSet<Chat> _chats;
        private readonly DbSet<User> _users;

        public MessageRepositoryTests() : base()
        {
            _messageRepository = new MessageRepository(_context);
            _chats = _context.Set<Chat>();
            _users = _context.Set<User>();
        }

        private async Task<Chat> CreateChatAsync()
        {
            var chat = new Chat(
                Language.GetByCode("en"),
                Language.GetByCode("es")
            );
            await _chats.AddAsync(chat);
            return chat;
        }

        private async Task<User> CreateUserAsync(string name = "Test User", string email = "test@example.com")
        {
            var user = new User(
                name,
                email,
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("BR"),
                Guid.NewGuid().ToString()
            );
            await _users.AddAsync(user);
            return user;
        }

        private async Task<Message> CreateMessageAsync(
            Chat chat = null,
            User sender = null,
            string content = "Test message"
        )
        {
            chat ??= await CreateChatAsync();
            sender ??= await CreateUserAsync();
            
            return new Message(chat.Id, sender.Id, content);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnMessage()
        {
            var message = await CreateMessageAsync();
            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetByIdAsync(message.Id);

            Assert.NotNull(result);
            Assert.Equal(message.Id, result.Id);
            Assert.Equal(message.ChatId, result.ChatId);
            Assert.Equal(message.SenderId, result.SenderId);
            Assert.Equal(message.Content, result.Content);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            var result = await _messageRepository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByChatIdAsync_ShouldReturnAllMessagesForChat()
        {
            var chat = await CreateChatAsync();
            var message1 = await CreateMessageAsync(chat: chat);
            var message2 = await CreateMessageAsync(chat: chat);
            var otherMessage = await CreateMessageAsync();

            await _messageRepository.AddAsync(message1);
            await _messageRepository.AddAsync(message2);
            await _messageRepository.AddAsync(otherMessage);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetByChatIdAsync(chat.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Id == message1.Id);
            Assert.Contains(result, m => m.Id == message2.Id);
            Assert.All(result, m => Assert.Equal(chat.Id, m.ChatId));
            Assert.True(result.SequenceEqual(result.OrderByDescending(m => m.SentAt)));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnAllMessagesFromUser()
        {
            var sender = await CreateUserAsync();
            var message1 = await CreateMessageAsync(sender: sender);
            var message2 = await CreateMessageAsync(sender: sender);
            var otherMessage = await CreateMessageAsync();

            await _messageRepository.AddAsync(message1);
            await _messageRepository.AddAsync(message2);
            await _messageRepository.AddAsync(otherMessage);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetByUserIdAsync(sender.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Id == message1.Id);
            Assert.Contains(result, m => m.Id == message2.Id);
            Assert.All(result, m => Assert.Equal(sender.Id, m.SenderId));
            Assert.True(result.SequenceEqual(result.OrderByDescending(m => m.SentAt)));
        }

        [Fact]
        public async Task AddAsync_ShouldCreateAndReturnMessage()
        {
            var message = await CreateMessageAsync();

            var result = await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var savedMessage = await _messageRepository.GetByIdAsync(result.Id);
            Assert.NotNull(savedMessage);
            Assert.Equal(message.ChatId, savedMessage.ChatId);
            Assert.Equal(message.SenderId, savedMessage.SenderId);
            Assert.Equal(message.Content, savedMessage.Content);
        }

        [Fact]
        public async Task EditMessage_ShouldUpdateMessage()
        {
            var message = await CreateMessageAsync();
            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var newContent = "Updated content";
            message.EditMessage(message.ChatId, message.SenderId, newContent);

            var result = await _messageRepository.UpdateAsync(message);
            await _unitOfWork.CommitAsync();

            Assert.NotNull(result);
            var updatedMessage = await _messageRepository.GetByIdAsync(message.Id);
            Assert.NotNull(updatedMessage);
            Assert.Equal(newContent, updatedMessage.Content);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingMessage_ShouldReturnTrue()
        {
            var message = await CreateMessageAsync();
            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.ExistsAsync(message.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingMessage_ShouldReturnFalse()
        {
            var result = await _messageRepository.ExistsAsync(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task IsUserMessageOwnerAsync_WhenUserIsOwner_ShouldReturnTrue()
        {
            var sender = await CreateUserAsync();
            var message = await CreateMessageAsync(sender: sender);
            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.IsUserMessageOwnerAsync(message.Id, sender.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task IsUserMessageOwnerAsync_WhenUserIsNotOwner_ShouldReturnFalse()
        {
            var message = await CreateMessageAsync();
            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.IsUserMessageOwnerAsync(message.Id, Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task GetChatMessageCountAsync_ShouldReturnCorrectCount()
        {
            var chat = await CreateChatAsync();
            var message1 = await CreateMessageAsync(chat: chat);
            var message2 = await CreateMessageAsync(chat: chat);
            var otherMessage = await CreateMessageAsync();

            await _messageRepository.AddAsync(message1);
            await _messageRepository.AddAsync(message2);
            await _messageRepository.AddAsync(otherMessage);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetChatMessageCountAsync(chat.Id);

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetByIdForOwnerAsync_ShouldReturnMessage()
        {
            var chat = await CreateChatAsync();
            var message = await CreateMessageAsync(chat: chat);

            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetByIdForOwnerAsync(message.Id, message.SenderId);

            Assert.NotNull(result);
            Assert.Equal(result.Content, message.Content);
            Assert.Equal(result.SenderId, message.SenderId);
        }

        [Fact]
        public async Task GetByIdForOwnerAsync_ShouldReturnNull()
        {
            var chat = await CreateChatAsync();
            var message = await CreateMessageAsync(chat: chat);

            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var result = await _messageRepository.GetByIdForOwnerAsync(message.Id, Guid.NewGuid());

            Assert.Null(result);
        }
    }
} 