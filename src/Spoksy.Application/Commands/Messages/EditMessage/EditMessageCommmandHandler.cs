using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;

namespace Spoksy.Application.Commands.Messages.EditMessage
{
    public class EditMessageCommmandHandler : IEditMessageCommmandHandler
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatValidationService _chatValidationService;

        public EditMessageCommmandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork, IChatValidationService chatValidationService)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<MessageResponse>> Handle(Guid userId, EditMessageCommand command)
        {
            await _chatValidationService.EnsureChatAvailableAsync(command.ChatId);

            await _chatValidationService.EnsureUserAccessForChatAsync(userId, command.ChatId);

            var message = await _messageRepository.GetByIdForOwnerAsync(command.MessageId, userId);

            if (message == null)
                return NotFoundResult<MessageResponse>.Create("Message not found.");
            
            message.EditMessage(command.ChatId, userId, command.Content);

            await _messageRepository.UpdateAsync(message);
            await _unitOfWork.CommitAsync();

            var response = MessageResponse.FromEntity(message);
            return Result<MessageResponse>.Success(response);
        }
    }
}
