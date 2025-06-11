using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Services;

namespace Spoksy.Application.Commands.UserLanguages.DeleteUserLanguage 
{ 
    public class DeleteUserLanguageCommandHandler : IDeleteUserLanguageCommandHandler
    {
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLanguageValidationService _userLanguageValidationService;

        public DeleteUserLanguageCommandHandler(
            IUserLanguageRepository userLanguageRepository,
            IUnitOfWork unitOfWork,
            IUserLanguageValidationService userLanguageValidationService)
        {
            _userLanguageRepository = userLanguageRepository;
            _unitOfWork = unitOfWork;
            _userLanguageValidationService = userLanguageValidationService;
        }

        public async Task<Result<string>> Handle(Guid userId, Guid id)
        {  
            var userLanguage = await _userLanguageRepository.GetByIdAsync(id, userId);
            if (userLanguage == null)
                return NotFoundResult<string>.Create($"User language not found");

            await _userLanguageValidationService.EnsureLanguageCanBeRemoved(userId, id);

            await _userLanguageRepository.DeleteAsync(userLanguage.Id, userId);
            await _unitOfWork.CommitAsync();

            return Result<string>.Success($"Successfully deleted the language");
        }

    }
} 