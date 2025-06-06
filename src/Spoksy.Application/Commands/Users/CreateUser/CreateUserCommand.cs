using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Commands.Users.CreateUser
{
    public class CreateUserCommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string CountryCode { get; set; }
        public string Passsword { get; set; }
        public List<UserLanguageDto> Languages { get; set; }
    }

    public class UserLanguageDto
    {
        public string LanguageCode { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; } 
    }

}

