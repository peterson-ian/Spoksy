using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Queries.Chats.GetAllChat
{
    public interface IGetAllChatQueryHandler
    {
        Task<Result<PaginatedResponse<ChatIdentificatorResponse>>> Handle(Guid userId, int page, ChatFilter? chatFilter = null);
    }
}
