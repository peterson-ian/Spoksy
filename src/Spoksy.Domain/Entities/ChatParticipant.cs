using Spoksy.Domain.Exceptions;

namespace Spoksy.Domain.Entities
{
    public class ChatParticipant : Entity
    {
        public Guid UserId { get; private set; }
        public Guid ChatId { get; private set; }
        public DateTime JoinAt { get; private set; }
        public DateTime? LeaveAt { get; private set; }

        public ChatParticipant() { }

        public ChatParticipant(Guid userId, Guid chatId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User ID cannot be empty");
            if (chatId == Guid.Empty)
                throw new DomainException("Chat ID cannot be empty");

            Id = Guid.NewGuid();
            UserId = userId;
            ChatId = chatId;
            JoinAt = DateTime.UtcNow;
            LeaveAt = null;
        }

        public void Leave()
        {
            LeaveAt = DateTime.UtcNow;
        }
    }
}
