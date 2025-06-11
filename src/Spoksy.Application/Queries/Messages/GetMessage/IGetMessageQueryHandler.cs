using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Queries.Messages.GetMessage
{
    public interface IGetMessageQueryHandler
    {
        Task<Result<MessageResponse>> Handle(Guid userId, Guid chatId, Guid messageId);
    }
}
