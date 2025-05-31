using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Entities
{
    public class UserLanguage : Entity
    {
        public Guid UserId { get; private set; }
        public Language Language { get; private set; }
        public ProficiencyLevel ProficiencyLevel { get; private set; }
        public DateTime StartedLearningOn { get; private set; }

        private UserLanguage() { }

        public UserLanguage(Guid userId, Language language, ProficiencyLevel proficiencyLevel)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Language = language ?? throw new DomainException("Language cannot be null");
            ProficiencyLevel = proficiencyLevel;
            StartedLearningOn = DateTime.UtcNow;
        }

        public void UpdateProficiency(ProficiencyLevel newLevel)
        {
            if (newLevel == ProficiencyLevel)
                throw new DomainException("New proficiency level must be different from the current one");

            ProficiencyLevel = newLevel;
        }

        public void UpdateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User ID cannot be empty");

            UserId = userId;
        }
    }
}
