using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Responses
{
    public record UserLanguageReponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public DateTime StartedLearningOn { get; set; }

        public static UserLanguageReponse FromEntity(UserLanguage entity)
        {
            return new UserLanguageReponse
            {
                Id = entity.Id,
                Code = entity.Language.Code,
                Name = entity.Language.Name,
                ProficiencyLevel = entity.ProficiencyLevel,
                StartedLearningOn = entity.StartedLearningOn
            };
        }
    }
}
