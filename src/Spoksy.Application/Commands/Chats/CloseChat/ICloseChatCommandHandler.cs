using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Commands.Chats.CloseChat
{
    public interface ICloseChatCommandHandler
    {
        Task<Result<string>> Handle(Guid userId, Guid chatId);
    }
}
 