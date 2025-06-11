using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Services;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Commands.UserLanguages.CreateUserLanguage
{
    public class CreateUserLanguageCommandHandler : ICreateUserLanguageCommandHandler
    {
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLanguageValidationService _userLanguageValidationService;

        public CreateUserLanguageCommandHandler(
            IUserLanguageRepository userLanguageRepository,
            IUnitOfWork unitOfWork,
            IUserLanguageValidationService userLanguageValidationService)
        {
            _userLanguageRepository = userLanguageRepository;
            _unitOfWork = unitOfWork;
            _userLanguageValidationService = userLanguageValidationService;
        }

        public async Task<Result<UserLanguageResponse>> Handle(Guid userId, CreateUserLanguageCommand command)
        {
            var languageEntity = Language.GetByCode(command.LanguageCode);
            if (languageEntity == null)
                return NotFoundResult<UserLanguageResponse>.Create($"Language {command.LanguageCode} not found");

            if (await _userLanguageRepository.HasLanguageAsync(userId, languageEntity))
                return ConflictResult<UserLanguageResponse>.Create($"Language {languageEntity.Name}({languageEntity.Code}) is already registered for this user");

            var userLanguage = new UserLanguage(userId, languageEntity, command.ProficiencyLevel);
            await _userLanguageRepository.AddAsync(userLanguage);                

            await _unitOfWork.CommitAsync();

            var response =  UserLanguageResponse.FromEntity(userLanguage);

            return Result<UserLanguageResponse>.Success(response); 
        }
    }
} 