using Spoksy.Application.Commons;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Commands.Chats.CreateChat
{
    public interface ICreateChatCommandHandler
    {
        Task<Result<ChatIdentificatorResponse>> Handle(Guid userId, CreateChatCommand command);
    }
}
