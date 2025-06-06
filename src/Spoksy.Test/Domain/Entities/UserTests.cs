using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Test.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_WithValidData_ShouldCreateUserSuccessfully()
        {
            var name = "John Doe";
            var email = "john.doe@example.com";
            var birthDate = DateTime.UtcNow.AddYears(-25);
            var country = Country.GetByCode("US");
            var identityProviderId = Guid.NewGuid().ToString();

            var user = new User(name, email, birthDate, country, identityProviderId);

            Assert.NotNull(user);
            Assert.Equal(name, user.Name);
            Assert.Equal(email, user.Email);
            Assert.Equal(birthDate, user.BirthDate);
            Assert.Equal(country, user.CurrentCountry);
            Assert.Equal(identityProviderId, user.IdentityProviderId);
            Assert.Equal(UserStatus.Active, user.Status);
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.NotNull(user.CreatedAt);
            Assert.NotNull(user.LastActivityAt);
        }

        [Fact]
        public void CreateUser_WithDifferentStatus_ShouldSetStatusCorrectly()
        {
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                Guid.NewGuid().ToString(),
                UserStatus.Banned);

            Assert.Equal(UserStatus.Banned, user.Status);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateUser_WithInvalidName_ShouldThrowDomainException(string invalidName)
        {
            var email = "john.doe@example.com";
            var birthDate = DateTime.UtcNow.AddYears(-25);
            var country = Country.GetByCode("US");
            var identityProviderId = Guid.NewGuid().ToString();

            var exception = Assert.Throws<DomainException>(() =>
                new User(invalidName, email, birthDate, country, identityProviderId));
            Assert.Equal("Name cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("no-at-symbol.com")]
        [InlineData("missingdomain@")]
        [InlineData("@missingusername.com")]
        [InlineData("username@.com")]
        public void CreateUser_WithInvalidEmail_ShouldThrowDomainException(string invalidEmail)
        {
            var name = "John Doe";
            var birthDate = DateTime.UtcNow.AddYears(-25);
            var country = Country.GetByCode("US");
            var identityProviderId = Guid.NewGuid().ToString();

            var exception = Assert.Throws<DomainException>(() =>
                new User(name, invalidEmail, birthDate, country, identityProviderId));

            Assert.Equal("Email invalid", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateUser_WithEmptyNullEmail_ShouldThrowDomainException(string invalidEmail)
        {
            var name = "John Doe";
            var birthDate = DateTime.UtcNow.AddYears(-25);
            var country = Country.GetByCode("US");
            var identityProviderId = Guid.NewGuid().ToString();

            var exception = Assert.Throws<DomainException>(() =>
                new User(name, invalidEmail, birthDate, country, identityProviderId));

            Assert.Equal("Email cannot be empty", exception.Message);
        }


        [Fact]
        public void CreateUser_WithAgeBelowMinimum_ShouldThrowDomainException()
        {
            var name = "John Doe";
            var email = "john.doe@example.com";
            var birthDate = DateTime.UtcNow.AddYears(-15);
            var country = Country.GetByCode("US");
            var identityProviderId = Guid.NewGuid().ToString();

            var exception = Assert.Throws<DomainException>(() =>
                new User(name, email, birthDate, country, identityProviderId));
            Assert.Contains("User must be at least 21 years old", exception.Message);
        }

        [Fact]
        public void UpdateName_WithValidName_ShouldUpdateSuccessfully()
        {
            var identityProviderId = Guid.NewGuid().ToString();

            var user = new User("John Doe", "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25), Country.GetByCode("US"), identityProviderId);
            var newName = "Jane Doe";

            user.UpdateName(newName);

            Assert.Equal(newName, user.Name);
        }

        [Fact]
        public void UpdateName_WhenUserIsInactive_ShouldThrowDomainException()
        {
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                Guid.NewGuid().ToString());

            user.DeactivateUser();

            var exception = Assert.Throws<DomainException>(() => user.UpdateName("Jane Doe"));

            Assert.Equal("Operation not allowed. User is not active.", exception.Message);
        }


        [Fact]
        public void UpdateEmail_WithValidEmail_ShouldUpdateSuccessfully()
        {
            var identityProviderId = Guid.NewGuid().ToString();

            var user = new User("John Doe", "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25), Country.GetByCode("US"), identityProviderId);
            var newEmail = "jane.doe@example.com";

            user.UpdateEmail(newEmail);

            Assert.Equal(newEmail, user.Email);
        }

        [Fact]
        public void UpdateEmail_WhenUserIsSuspended_ShouldThrowDomainException()
        {
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                Guid.NewGuid().ToString());

            user.SuspendUser();

            var exception = Assert.Throws<DomainException>(() => user.UpdateEmail("jane.doe@example.com"));

            Assert.Equal("Operation not allowed. User is not active.", exception.Message);
        }

        [Fact]
        public void UpdateCurrentCountry_WithValidCountry_ShouldUpdateSuccessfully()
        {
            var identityProviderId = Guid.NewGuid().ToString();

            var user = new User("John Doe", "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25), Country.GetByCode("US"), identityProviderId);
            var newCountry = Country.GetByCode("BR");

            user.UpdateCurrentCountry(newCountry);

            Assert.Equal(newCountry, user.CurrentCountry);
        }


        [Fact]
        public void UpdateCurrentCountry_WhenUserIsBanned_ShouldThrowDomainException()
        {
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                Guid.NewGuid().ToString());

            user.BanUser();

            var exception = Assert.Throws<DomainException>(() => user.UpdateCurrentCountry(Country.GetByCode("BR")));

            Assert.Equal("Operation not allowed. User is not active.", exception.Message);
        }

        [Fact]
        public void BanUser_ShouldSetStatusToBanned()
        {
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                Guid.NewGuid().ToString());

            user.BanUser();

            Assert.Equal(UserStatus.Banned, user.Status);
            Assert.NotNull(user.LastActivityAt);
        }

        [Fact]
        public void SuspendUser_ShouldSetStatusToSuspended()
        {
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                Guid.NewGuid().ToString());

            user.SuspendUser();

            Assert.Equal(UserStatus.Suspended, user.Status);
            Assert.NotNull(user.LastActivityAt);
        }

        [Fact]
        public void DeactivateUser_ShouldSetStatusToInactive()
        {
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                Guid.NewGuid().ToString());

            user.DeactivateUser();

            Assert.Equal(UserStatus.Inactive, user.Status);
            Assert.NotNull(user.LastActivityAt);
        }

        [Fact]
        public void ReactivateUser_ShouldSetStatusToActive()
        {
            var user = new User(
                 "John Doe",
                 "john.doe@example.com",
                 DateTime.UtcNow.AddYears(-25),
                 Country.GetByCode("US"),
                 Guid.NewGuid().ToString());

            user.DeactivateUser();
            user.ReactivateUser();

            Assert.Equal(UserStatus.Active, user.Status);
            Assert.NotNull(user.LastActivityAt);
        }

    }
} 