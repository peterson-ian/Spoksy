using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Responses
{
    public record MessageResponse
    {
        public Guid Id { get; set; }
        public string Content { get;  set; }
        public Guid SenderId { get;  set; }
        public Language? Language { get;  set; }
        public DateTime SentAt { get;  set; }
        public DateTime? EditAt { get;  set; }
        public bool? IsRead { get;  set; }

        // for Dapper mapping the language
        internal string LanguageCode { get; set; } = default!;

        public static MessageResponse FromEntity(Message message)
        {
            return new MessageResponse
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                Language = message.Language,
                IsRead = message.IsRead,
                SentAt = message.SentAt,
                EditAt = message.EditAt 
            };
        }
    }
}
