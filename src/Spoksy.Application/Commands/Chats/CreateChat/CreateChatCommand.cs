using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Commands.Chats.CreateChat
{
    public class CreateChatCommand
    {
        public Guid FirstParticipant { get; set; }
        public Guid SecondParticipant { get; set; }
        public string PrimaryLanguageCode { get; set; }
        public string SecondaryLanguageCode { get; set; }
    }
}
