using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Responses
{
    public record ChatIdentificatorResponse
    {
        public Guid Id { get; set; }
        public Language PrimaryLanguage { get; set; }
        public Language SecondaryLanguage { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChatStatus Status { get; set; }
        public DateTime LastActivityAt { get; set; }
        public MessageResponse? LastMessage { get; set; }
        public ChatParticipantResponse Recipient { get; set; }

        // for Dapper mapping the languages
        internal string PrimaryLanguageCode { get; set; } = default!;
        internal string SecondaryLanguageCode { get; set; } = default!;

        public static ChatIdentificatorResponse FromEntity(Chat chat, ChatParticipant recipient, User user, Message? lastMessage = null)
        {
            return new ChatIdentificatorResponse {
                Id = chat.Id,
                CreatedAt = chat.CreatedAt,
                LastActivityAt = chat.LastActivityAt,
                LastMessage = lastMessage != null ? MessageResponse.FromEntity(lastMessage) : null,
                PrimaryLanguage = chat.PrimaryLanguage,
                SecondaryLanguage = chat.SecondaryLanguage,
                Status = chat.Status,
                Recipient = ChatParticipantResponse.FromEntity(recipient, user) 
            };
        }


    }
}
