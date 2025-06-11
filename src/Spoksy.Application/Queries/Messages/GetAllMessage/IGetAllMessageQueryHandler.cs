using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Queries.Messages.GetAllMessage
{
    public interface IGetAllMessageQueryHandler
    {
        Task<Result<PaginatedResponse<MessageResponse>>> Handle(Guid userId, Guid chatId, int page);
    }
}
