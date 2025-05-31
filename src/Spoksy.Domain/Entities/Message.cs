using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Entities{
   
    public class Message : Entity
    {
        public Guid ChatId { get; private set; }
        public Guid SenderId { get; private set; }
        public string Content { get; private set; }
        public Language? Language { get; private set; }
        public DateTime SentAt { get; private set; }
        public DateTime? EditAt { get; private set; }
        public DateTime? DeleteAt { get; private set; }
        public bool IsRead { get; private set; }
        public bool IsEdit { get; private set; }
        public bool IsDelete { get; private set; }

        private const int MAX_MINUTES_TO_EDIT = 15;

        private const int MAX_MINUTES_TO_DELETE = 15;

        private Message() { } 

        public Message(Guid chatId, Guid senderId, string content, Language? language)
        {
            if (chatId == Guid.Empty)
                throw new DomainException("Chat ID cannot be empty");

            if (senderId == Guid.Empty)
                throw new DomainException("Sender ID cannot be empty");

            ValidateContent(content);

            Id = Guid.NewGuid();
            ChatId = chatId;
            SenderId = senderId;
            Content = content;
            Language = language;
            SentAt = DateTime.UtcNow;
            IsRead = false;
            IsDelete = false;
            IsEdit = false;
        }

        public void SetRead()
        {
            IsRead = true;
        }

        public void EditMessage(Guid chatId, Guid userId, string content)
        {
            if (chatId == Guid.Empty)
                throw new DomainException("ChatId cannot be null");

            if (userId == Guid.Empty)
                throw new DomainException("ChatId cannot be null");

            if (userId != SenderId)
                throw new DomainException("The user dont have access to edit the message");

            if (DateTime.UtcNow > SentAt.AddMinutes(MAX_MINUTES_TO_EDIT))
                throw new DomainException($"The message cannot edit, passed {MAX_MINUTES_TO_EDIT} minutes");

            ValidateContent(content);

            Content = content;
            IsEdit = true;
            EditAt = DateTime.UtcNow;
        }

        public void DeleteMessage(Guid chatId, Guid userId)
        {
            if (chatId == Guid.Empty)
                throw new DomainException("ChatId cannot be null");

            if (userId == Guid.Empty)
                throw new DomainException("ChatId cannot be null");

            if (userId != SenderId)
                throw new DomainException("The user dont have access to delete the message");

            if (DateTime.UtcNow > SentAt.AddMinutes(MAX_MINUTES_TO_DELETE))
                throw new DomainException($"The message cannot edit, passed {MAX_MINUTES_TO_DELETE} minutes");

            IsDelete = true;
            DeleteAt = DateTime.UtcNow;
        }

        private void ValidateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Message content cannot be empty");

            if (content.Length > 10000)
                throw new DomainException("Message cannot be longer than 10000 characters");
        }

    }

}