using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Queries.Chats.GetAllChat
{
    public class ChatFilter
    {
        public ChatStatus? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? LanguageCode { get; set; }
    }
}
