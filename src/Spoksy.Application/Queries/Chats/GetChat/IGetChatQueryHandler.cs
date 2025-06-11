using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Queries.Chats.GetChat
{
    public interface IGetChatQueryHandler
    {
        Task<Result<ChatResponse>> Handle(Guid userId, Guid chatId);
    }
}
