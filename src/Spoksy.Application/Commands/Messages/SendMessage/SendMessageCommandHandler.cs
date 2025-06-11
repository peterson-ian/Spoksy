using Spoksy.Application.Commons;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;

namespace Spoksy.Application.Commands.Messages.SendMessage
{
    public class SendMessageCommandHandler : ISendMessageCommandHandler
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatValidationService _chatValidationService;

        public SendMessageCommandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork, IChatValidationService chatValidationService)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<MessageResponse>> Handle(Guid userId, SendMessageCommand command)
        {
            await _chatValidationService.EnsureChatAvailableAsync(command.ChatId);

            await _chatValidationService.EnsureUserAccessForChatAsync(userId, command.ChatId);

            Message message =  new Message(command.ChatId, userId, command.Content);
            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var response = MessageResponse.FromEntity(message);
            return Result<MessageResponse>.Success(response);
        }
    }
}
