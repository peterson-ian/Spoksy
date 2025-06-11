namespace Spoksy.Application.Commands.Messages.EditMessage
{
    public class EditMessageCommand
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        public string Content { get; set; }
    }
}
