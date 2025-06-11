using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;
using System.Collections.Generic;
using System.Reflection;

namespace Spoksy.Domain.Entities
{
    public class Chat : Entity
    {
        public Language PrimaryLanguage { get; private set; }
        public Language SecondaryLanguage { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ChatStatus Status { get; private set; }
        public DateTime LastActivityAt { get; private set; }

        private Chat() { }

        public Chat(Language primaryLanguage, Language secondaryLanguage)
        {
            Id = Guid.NewGuid();
            PrimaryLanguage = primaryLanguage ?? throw new DomainException("Primary language cannot be null");
            SecondaryLanguage = secondaryLanguage ?? throw new DomainException("Secondary language cannot be null"); ;
            CreatedAt = DateTime.UtcNow;
            LastActivityAt = DateTime.UtcNow;
            Status = ChatStatus.Active;
        }

        public void CloseChat()
        {
            Status = ChatStatus.Closed;
            UpdateLastAcitivity();
        }

        public void ArchiveChat()
        {
            Status = ChatStatus.Archived;
            UpdateLastAcitivity();
        }

        public void ReactivateChat()
        {
            Status = ChatStatus.Active;
            UpdateLastAcitivity();
        }

        public void UpdateLastAcitivity()
        {
            LastActivityAt = DateTime.UtcNow;
        }

        public bool IsLanguageSupported(Language language)
        {
            return language == PrimaryLanguage || language == SecondaryLanguage;
        }

        public bool IsActive()
        {
            return Status == ChatStatus.Active;
        }
    }

}