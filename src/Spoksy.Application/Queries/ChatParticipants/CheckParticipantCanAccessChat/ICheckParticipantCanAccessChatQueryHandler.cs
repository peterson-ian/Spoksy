using Spoksy.Application.Commons;

namespace Spoksy.Application.Queries.ChatParticipants.CheckParticipantCanAccessChat
{
    public interface ICheckParticipantCanAccessChatQueryHandler
    {
        Task<Result<bool>> Handle(Guid userId, Guid chatId);
    }
}
