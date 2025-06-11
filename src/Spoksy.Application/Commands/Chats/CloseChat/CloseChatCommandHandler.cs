using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Domain.Contracts;

namespace Spoksy.Application.Commands.Chats.CloseChat
{
    public class CloseChatCommandHandler : ICloseChatCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatValidationService _chatValidationService;

        public CloseChatCommandHandler(
            IUserRepository userRepository, 
            IChatParticipantRepository chatParticipantRepository, 
            IChatRepository chatRepository, IUnitOfWork unitOfWork,
            IChatValidationService chatValidationService)
        {
            _userRepository = userRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _chatRepository = chatRepository;
            _unitOfWork = unitOfWork;
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<string>> Handle(Guid userId, Guid chatId)
        {
            await _chatValidationService.EnsureChatAvailableAsync(chatId);

            await _chatValidationService.EnsureUserAccessForChatAsync(userId, chatId);
                
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat == null)
                return NotFoundResult<string>.Create($"Chat with ID {chatId} not found.");

            chat.CloseChat();
            await _chatRepository.UpdateAsync(chat);
            await _unitOfWork.CommitAsync();

            return Result<string>.Success($"Chat with ID {chatId} has been closed successfully.");
        }
    }
}
