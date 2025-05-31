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
            var phone = "+1234567890";
            var birthDate = DateTime.UtcNow.AddYears(-25);
            var country = Country.GetByCode("US");

            var user = new User(name, email, birthDate, country);

            Assert.NotNull(user);
            Assert.Equal(name, user.Name);
            Assert.Equal(email, user.Email);
            Assert.Equal(birthDate, user.BirthDate);
            Assert.Equal(country, user.CurrentCountry);
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.NotNull(user.CreatedAt);
            Assert.NotNull(user.LastActiveAt);
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

            var exception = Assert.Throws<DomainException>(() =>
                new User(invalidName, email, birthDate, country));
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

            var exception = Assert.Throws<DomainException>(() =>
                new User(name, invalidEmail, birthDate, country));

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

            var exception = Assert.Throws<DomainException>(() =>
                new User(name, invalidEmail, birthDate, country));

            Assert.Equal("Email cannot be empty", exception.Message);
        }


        [Fact]
        public void CreateUser_WithAgeBelowMinimum_ShouldThrowDomainException()
        {
            var name = "John Doe";
            var email = "john.doe@example.com";
            var birthDate = DateTime.UtcNow.AddYears(-15);
            var country = Country.GetByCode("US");

            var exception = Assert.Throws<DomainException>(() =>
                new User(name, email, birthDate, country));
            Assert.Contains("User must be at least 21 years old", exception.Message);
        }

        [Fact]
        public void UpdateName_WithValidName_ShouldUpdateSuccessfully()
        {
            var user = new User("John Doe", "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25), Country.GetByCode("US"));
            var newName = "Jane Doe";

            user.UpdateName(newName);

            Assert.Equal(newName, user.Name);
        }

        [Fact]
        public void UpdateEmail_WithValidEmail_ShouldUpdateSuccessfully()
        {
            var user = new User("John Doe", "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25), Country.GetByCode("US"));
            var newEmail = "jane.doe@example.com";

            user.UpdateEmail(newEmail);

            Assert.Equal(newEmail, user.Email);
        }

        [Fact]
        public void UpdateCurrentCountry_WithValidCountry_ShouldUpdateSuccessfully()
        {
            var user = new User("John Doe", "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25), Country.GetByCode("US"));
            var newCountry = Country.GetByCode("BR");

            user.UpdateCurrentCountry(newCountry);

            Assert.Equal(newCountry, user.CurrentCountry);
        }
    }
} 