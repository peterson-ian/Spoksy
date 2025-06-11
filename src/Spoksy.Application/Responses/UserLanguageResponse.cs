using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Responses
{
    public record UserLanguageResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public DateTime StartedLearningOn { get; set; }

        public static UserLanguageResponse FromEntity(UserLanguage entity)
        {
            return new UserLanguageResponse
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
