using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Responses
{
    public record UserDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public Country CurrentCountry { get; set; }
        public List<UserLanguageResponse> Languages { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public UserStatus? Status { get; set; }

        // for Dapper mapping the current Country
        internal string CountryCode { get; set; } = default!;

        public static UserDetailsResponse FromEntity(User user, List<UserLanguage> languages)
        {
            return new UserDetailsResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                BirthDate = user.BirthDate,
                CurrentCountry = user.CurrentCountry,
                CountryCode = user.CurrentCountry.Code,
                Languages = languages
                                .Select(l => UserLanguageResponse.FromEntity(l))
                                .ToList(),
                CreatedAt = user.CreatedAt,
                LastActivityAt = user.LastActivityAt,
                Status = user.Status
            };
        }

    }
}
