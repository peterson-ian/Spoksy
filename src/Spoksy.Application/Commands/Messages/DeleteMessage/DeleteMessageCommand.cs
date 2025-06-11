namespace Spoksy.Application.Commands.Messages.DeleteMessage
{
    public class DeleteMessageCommand
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
    }
}
