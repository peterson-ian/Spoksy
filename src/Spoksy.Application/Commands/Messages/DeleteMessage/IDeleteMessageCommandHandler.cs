using Spoksy.Application.Commons;

namespace Spoksy.Application.Commands.Messages.DeleteMessage
{
    public interface IDeleteMessageCommandHandler
    {
        Task<Result<string>> Handle(Guid userId, DeleteMessageCommand command);
    }
}
