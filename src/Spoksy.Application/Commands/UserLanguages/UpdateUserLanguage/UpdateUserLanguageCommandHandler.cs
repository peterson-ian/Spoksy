using Spoksy.Application.Responses;
using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;

namespace Spoksy.Application.Commands.UserLanguages.UpdateUserLanguage
{
    public class UpdateUserLanguageCommandHandler : IUpdateUserLanguageCommandHandler
    {
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserLanguageCommandHandler(
            IUserLanguageRepository userLanguageRepository,
            IUnitOfWork unitOfWork)
        {
            _userLanguageRepository = userLanguageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserLanguageReponse>> Handle(Guid userId, UpdateUserLanguageCommand command)
        {
            var userLanguage = await _userLanguageRepository.GetByIdAsync(command.Id, userId);
            if (userLanguage == null)
                return ValidationResult<UserLanguageReponse>.Failure($"User language not found");

            userLanguage.UpdateProficiency(command.ProficiencyLevel);
            await _userLanguageRepository.UpdateAsync(userLanguage);
              
            await _unitOfWork.CommitAsync();

            var response = UserLanguageReponse.FromEntity(userLanguage);

            return Result<UserLanguageReponse>.Success(response);            
        }
    }
} 