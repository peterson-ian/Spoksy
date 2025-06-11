using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;

namespace Spoksy.Domain.Services
{
    public class ChatValidationService : IChatValidationService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;

        public ChatValidationService(IChatRepository chatRepository, IChatParticipantRepository chatParticipantRepository)
        {
            _chatRepository = chatRepository;
            _chatParticipantRepository = chatParticipantRepository;
        }

        public async Task EnsureChatAvailableAsync(Guid chatId)
        {
            if(chatId == Guid.Empty)
                throw new DomainException("Chat ID cannot be empty.");

            Chat chat = await _chatRepository.GetByIdAsync(chatId)
                ?? throw new DomainException($"Chat with ID {chatId} not found.");

            if (!chat.IsActive()) 
                throw new DomainException($"Chat with ID {chatId} is not active.");
        }

        public async Task EnsureUserAccessForChatAsync(Guid userId, Guid chatId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User ID cannot be empty.");

            if (chatId == Guid.Empty)
                throw new DomainException("Chat ID cannot be empty.");  

            ChatParticipant chatParticipant = await _chatParticipantRepository.GetByChatIdAndUserIdAsync(chatId, userId)
                ?? throw new DomainException($"User with ID {userId} is not a participant of chat with ID {chatId}.");

            if (chatParticipant.LeaveAt != null)
                throw new DomainException($"User with ID {userId} has left the chat with ID {chatId}.");
        }
    }
}
