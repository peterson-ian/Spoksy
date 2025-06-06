using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Commands.UserLanguages.UpdateUserLanguage
{
    public class UpdateUserLanguageCommand
    {
        public Guid Id { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
    }

} 