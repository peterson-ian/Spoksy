using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;

namespace Spoksy.Application.Commands.Messages.DeleteMessage
{
    public class DeleteMessageCommandHandler : IDeleteMessageCommandHandler
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatValidationService _chatValidationService;

        public DeleteMessageCommandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork, IChatValidationService chatValidationService)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<string>> Handle(Guid userId, DeleteMessageCommand command)
        {
            await _chatValidationService.EnsureChatAvailableAsync(command.ChatId);

            await _chatValidationService.EnsureUserAccessForChatAsync(userId, command.ChatId);

            var message = await _messageRepository.GetByIdForOwnerAsync(command.MessageId, userId);

            if (message == null)
                return NotFoundResult<string>.Create("Message not found.");

            message.DeleteMessage(command.ChatId, userId);
            await _messageRepository.UpdateAsync(message);
            await _unitOfWork.CommitAsync();

            return Result<string>.Success("Message deleted successfully.");
        }
    }
}
