using Spoksy.Application.Commons;
using Spoksy.Domain.Contracts;

namespace Spoksy.Application.Queries.ChatParticipants.CheckParticipantCanAccessChat
{
    public class CheckParticipantCanAccessChatQueryHandler : ICheckParticipantCanAccessChatQueryHandler
    {
        private readonly IChatValidationService _chatValidationService;

        public CheckParticipantCanAccessChatQueryHandler(IChatValidationService chatValidationService)
        {
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<bool>> Handle(Guid userId, Guid chatId)
        {
            await _chatValidationService.EnsureChatAvailableAsync(chatId);

            await _chatValidationService.EnsureUserAccessForChatAsync(userId, chatId);

            return Result<bool>.Success(true);
        }
    }
}
