using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Commands.Messages.EditMessage
{
    public interface IEditMessageCommmandHandler
    {
        Task<Result<MessageResponse>> Handle(Guid userId, EditMessageCommand command);
    }
}
