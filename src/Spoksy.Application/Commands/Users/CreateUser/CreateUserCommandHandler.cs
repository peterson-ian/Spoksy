using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Spoksy.Domain.Services;
using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Responses;

namespace Spoksy.Application.Commands.Users.CreateUser
{
    public class CreateUserCommandHandler : ICreateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLanguageValidationService _userLanguageValidationService;
        private readonly IIdentityProviderIntegration _identityProviderIntegration;

        public CreateUserCommandHandler(
            IUserRepository userRepository, 
            IUserLanguageRepository userLanguageRepository, 
            IUnitOfWork unitOfWork, 
            IUserLanguageValidationService userLanguageValidationService, 
            IIdentityProviderIntegration identityProviderIntegration)
        {
            _userRepository = userRepository;
            _userLanguageRepository = userLanguageRepository;
            _unitOfWork = unitOfWork;
            _userLanguageValidationService = userLanguageValidationService;
            _identityProviderIntegration = identityProviderIntegration;
        }

        public async Task<Result<UserDetailsResponse>> Handle(CreateUserCommand command)
        {
            if (!await _userRepository.IsEmailUniqueAsync(command.Email))
                return ConflictResult<UserDetailsResponse>.Create("Email already exists");

            var country = Country.GetByCode(command.CountryCode);
            if (country == null)
                return NotFoundResult<UserDetailsResponse>.Create($"Country {command.CountryCode} not found");

            await _userLanguageValidationService.EnsureValidLanguagesForUserCreation(
                command.Languages.Where(x => x.ProficiencyLevel == ProficiencyLevel.Native).Select(x => Language.GetByCode(x.LanguageCode)).ToList(),
                command.Languages.Where(x => x.ProficiencyLevel != ProficiencyLevel.Native).Select(x => Language.GetByCode(x.LanguageCode)).ToList()
            );

            await _unitOfWork.BeginTransactionAsync();

            string identityProviderId = await _identityProviderIntegration.CreateUserAsync(command.Name, command.Email, true, command.Passsword); ;
            try
            {
                var user = new User(command.Name, command.Email, command.BirthDate, country, identityProviderId);
                await _userRepository.AddAsync(user);

                var userLanguages = new List<UserLanguage>();
                foreach (var language in command.Languages)
                {
                    var userLanguage = new UserLanguage(user.Id, Language.GetByCode(language.LanguageCode), language.ProficiencyLevel);
                    await _userLanguageRepository.AddAsync(userLanguage);
                    userLanguages.Add(userLanguage);
                }

                await _unitOfWork.CommitAsync();

                var response = UserDetailsResponse.FromEntity(user, userLanguages);

                return Result<UserDetailsResponse>.Success(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                await _identityProviderIntegration.DeleteUserAsync(identityProviderId);
                
                throw new ApplicationException("An error occurred while creating the user");
            }
        }
    }
}
