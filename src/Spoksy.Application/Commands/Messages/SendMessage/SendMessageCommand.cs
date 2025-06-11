namespace Spoksy.Application.Commands.Messages.SendMessage
{
    public class SendMessageCommand
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; }
    }
}
