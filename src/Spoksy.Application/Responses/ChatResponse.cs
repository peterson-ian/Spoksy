using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Responses
{
    public record ChatResponse 
    {
        public Guid Id { get; set; }
        public Language PrimaryLanguage { get; set; }
        public Language SecondaryLanguage { get; set; }
        public ChatStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public ChatParticipantResponse Recipient { get; set; }
        public PaginatedResponse<MessageResponse> Messages { get; set; } = new PaginatedResponse<MessageResponse>();

        // for Dapper mapping the languages
        internal string PrimaryLanguageCode { get; set; } = default!;
        internal string SecondaryLanguageCode { get; set; } = default!;


        public static ChatResponse FromEntity(Chat chat, ChatParticipant recipient, User user, PaginatedResponse<MessageResponse> messages)
        {
            return new ChatResponse
            {
                Id = chat.Id,
                CreatedAt = chat.CreatedAt,
                LastActivityAt = chat.LastActivityAt,
                PrimaryLanguage = chat.PrimaryLanguage,
                SecondaryLanguage = chat.SecondaryLanguage,
                Status = chat.Status,
                Recipient = ChatParticipantResponse.FromEntity(recipient, user),
                Messages = messages
            };
        }

    }
}
