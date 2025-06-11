using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Commands.Chats.CreateChat
{
    public class CreateChatCommandHandler : ICreateChatCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKnowledgeExchangeValidationService _knowledgeExchangeValidationService;

        public CreateChatCommandHandler(
            IUserRepository userRepository,
            IChatParticipantRepository chatParticipantRepository,
            IChatRepository chatRepository,
            IUnitOfWork unitOfWork,
            IKnowledgeExchangeValidationService knowledgeExchangeValidationService)
        {
            _userRepository = userRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _chatRepository = chatRepository;
            _unitOfWork = unitOfWork;
            _knowledgeExchangeValidationService = knowledgeExchangeValidationService;
        }

        public async Task<Result<ChatIdentificatorResponse>> Handle(Guid userId, CreateChatCommand command)
        {
            var firstUser = await _userRepository.GetByIdAsync(command.FirstParticipant);
            var secondUser = await _userRepository.GetByIdAsync(command.SecondParticipant);

            if (firstUser == null)
                return NotFoundResult<ChatIdentificatorResponse>.Create($"User {command.FirstParticipant} not found");

            if (secondUser == null)
                return NotFoundResult<ChatIdentificatorResponse>.Create($"User {command.SecondParticipant} not found");

            var primaryLanguage = Language.GetByCode(command.PrimaryLanguageCode);
            var secondaryLanguage = Language.GetByCode(command.SecondaryLanguageCode);

            if (primaryLanguage == null)
                return NotFoundResult<ChatIdentificatorResponse>.Create($"Language {command.PrimaryLanguageCode} not found");

            if (secondaryLanguage == null)
                return NotFoundResult<ChatIdentificatorResponse>.Create($"Language {command.SecondaryLanguageCode} not found");


            await _knowledgeExchangeValidationService.EnsureKnowledgeExchangeAsync(firstUser.Id, secondUser.Id, primaryLanguage, secondaryLanguage);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                Chat chat = new Chat(primaryLanguage, secondaryLanguage);
                await _chatRepository.AddAsync(chat);

                ChatParticipant firstParticipant = new ChatParticipant(firstUser.Id, chat.Id);
                ChatParticipant secondParticipant = new ChatParticipant(secondUser.Id, chat.Id);
                await _chatParticipantRepository.AddAsync(firstParticipant);
                await _chatParticipantRepository.AddAsync(secondParticipant);

                await _unitOfWork.CommitAsync();

                var recipientUser = userId == command.FirstParticipant ? secondUser : firstUser;
                var recipientParticipant = userId == command.FirstParticipant ? secondParticipant : firstParticipant;

                var response = ChatIdentificatorResponse.FromEntity(chat, recipientParticipant, recipientUser);
                return ValidationResult<ChatIdentificatorResponse>.Success(response);

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new ApplicationException("An error occurred while creating the chat");
            }
        }
    }
}
