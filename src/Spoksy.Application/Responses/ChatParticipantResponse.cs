using Spoksy.Domain.Entities;

namespace Spoksy.Application.Responses
{
    public record ChatParticipantResponse
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime JoinAt { get; set; }
        public DateTime? LeaveAt { get; set; }

        public static ChatParticipantResponse FromEntity(ChatParticipant chat, User user)
        {
            return new ChatParticipantResponse
            {
                UserId = chat.UserId,
                Name = user.Name,
                JoinAt = chat.JoinAt,
                LeaveAt = chat.LeaveAt
            };
        }

    }
}
