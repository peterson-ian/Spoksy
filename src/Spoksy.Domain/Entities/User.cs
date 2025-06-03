using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;
using System.Net.Mail;

namespace Spoksy.Domain.Entities
{
    public class User : Entity
    {
        public string IdentityProviderId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime BirthDate { get; private set; }
        public Country CurrentCountry { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastActivityAt { get; private set; }
        public UserStatus Status { get; private set; }
        private User() { }

        public User(string name, string email, DateTime birthDate, Country currentCountry, string identityProviderId, UserStatus status = UserStatus.Active)
        {
            ValidateName(name);
            ValidateEmail(email);
            ValidateCountry(currentCountry);
            ValidateAge(birthDate, currentCountry.MinimumAge);

            if (string.IsNullOrWhiteSpace(identityProviderId))
                throw new DomainException("Identity Provider ID cannot be empty");

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            CurrentCountry = currentCountry;
            BirthDate = birthDate;
            CreatedAt = DateTime.UtcNow;
            LastActivityAt = DateTime.UtcNow;
            IdentityProviderId = identityProviderId;
            Status = status;
        }
        public void BanUser()
        {
            Status = UserStatus.Banned;
            UpdateLastActivity();
        }

        public void SuspendUser()
        {
            Status = UserStatus.Suspended;
            UpdateLastActivity();
        }

        public void DeactivateUser()
        {
            Status = UserStatus.Inactive;
            UpdateLastActivity();
        }

        public void ReactivateUser()
        {
            Status = UserStatus.Active;
            UpdateLastActivity();
        }

        public bool IsActive()
        {
            return Status == UserStatus.Active;
        }

        public void UpdateLastActivity()
        {
            LastActivityAt = DateTime.UtcNow;
        }

        public void UpdateName(string newName)
        {
            EnsureUserIsActive();

            ValidateName(newName);
            Name = newName;
            UpdateLastActivity();
        }

        public void UpdateEmail(string newEmail)
        {
            EnsureUserIsActive();

            ValidateEmail(newEmail);
            Email = newEmail;
            UpdateLastActivity();
        }

        public void UpdateCurrentCountry(Country newCountry)
        {
            EnsureUserIsActive();

            if (newCountry == null)
                throw new DomainException("Country cannot be null");

            ValidateAge(BirthDate, newCountry.MinimumAge);
            CurrentCountry = newCountry;
            UpdateLastActivity();
        }

        private void ValidateAge(DateTime birthDate, int minimumAge)
        {
            var age = DateTime.UtcNow.Year - birthDate.Year;
            if (birthDate.Date > DateTime.UtcNow.AddYears(-age))
                age--;

            if (age < minimumAge)
                throw new DomainException($"User must be at least {minimumAge} years old following the laws of the current country.");
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty");

            if (name.Length < 3)
                throw new DomainException("Name must be at least 3 characters long");

            if (name.Length > 100)
                throw new DomainException("Name cannot be longer than 100 characters");
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            try
            {
                var addr = new MailAddress(email);
                if (addr.Address != email)
                    throw new DomainException("Email invalid");
            }
            catch
            {
                throw new DomainException("Email invalid");
            }
        }

        private void ValidateCountry(Country currentCountry)
        {
            if (currentCountry == null)
                throw new DomainException("Current Country cannot be null");
        }

        private void EnsureUserIsActive()
        {
            if (!IsActive())
                throw new DomainException("Operation not allowed. User is not active.");
        }

    }
}