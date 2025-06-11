using Spoksy.Application.Commands.Chats.CreateChat;
using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Commands.Messages.SendMessage
{
    public interface ISendMessageCommandHandler
    {
        Task<Result<MessageResponse>> Handle(Guid userId, SendMessageCommand command);
    }
}
