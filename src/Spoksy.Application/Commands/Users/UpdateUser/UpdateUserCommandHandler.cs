using Spoksy.Application.Responses;
using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Commands.Users.UpdateUser
{
    public class UpdateUserCommandHandler : IUpdateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserLanguageRepository _userLanguageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUserLanguageRepository userLanguageRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userLanguageRepository = userLanguageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserDetailsResponse>> Handle(Guid id, UpdateUserCommand command)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFoundResult<UserDetailsResponse>.Create("User not found");

            if (!string.IsNullOrEmpty(command.Name))
            {
                user.UpdateName(command.Name);
            }

            if (!string.IsNullOrEmpty(command.CountryCode))
            {
                var country = Country.GetByCode(command.CountryCode);
                if (country == null)
                    return ValidationResult<UserDetailsResponse>.Failure($"Country {command.CountryCode} not found");

                user.UpdateCurrentCountry(country);
            }

            await _unitOfWork.CommitAsync();

            var userLanguages = await _userLanguageRepository.GetUserLanguagesAsync(user.Id);

            var response = UserDetailsResponse.FromEntity(user, userLanguages.ToList());

            return Result<UserDetailsResponse>.Success(response);
        }
    }
} 