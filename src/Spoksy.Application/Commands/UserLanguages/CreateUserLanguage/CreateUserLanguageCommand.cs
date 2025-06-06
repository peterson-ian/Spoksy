using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Commands.UserLanguages.CreateUserLanguage
{
    public class CreateUserLanguageCommand
    {
        public string LanguageCode { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
    }

} 