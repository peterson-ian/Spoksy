using Dapper;
using Spoksy.Application.Responses;
using Spoksy.Application.Commons;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;
using Spoksy.Application.Commons.Results;

namespace Spoksy.Application.Queries.Users.GetUser
{
    public class GetUserQueryHandler : IGetUserQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetUserQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Result<UserDetailsResponse>> Handle(Guid userId)
        {
            using var connection = _connectionFactory.CreateConnection();
           
            const string sql = @"
                SELECT 
                    u.id,
                    u.name,
                    u.email,
                    u.birth_date as BirthDate,
                    u.current_country as CountryCode,
                    u.status as Status,
                    u.created_at as CreatedAt,
                    u.last_active_at as LastActivityAt,
                    ul.id as Id,
                    ul.language as Code,
                    ul.proficiency_level as ProficiencyLevel,
                    ul.started_learning_on as StartedLearningOn
                FROM users u
                LEFT JOIN user_languages ul ON u.id = ul.user_id
                WHERE u.status = 'Active' and u.id = @UserId";

            var userDictionary = new Dictionary<Guid, UserDetailsResponse>();

            var users = await connection.QueryAsync<UserDetailsResponse, UserLanguageResponse, UserDetailsResponse>(
                sql,
                (user, language) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.CurrentCountry = Country.GetByCode(user.CountryCode);
                        userDictionary.Add(user.Id, userEntry);
                    }

                    if (language != null)
                    {
                        language.Name = Language.GetByCode(language.Code)?.Name ?? "Unknown";
                        userEntry.Languages.Add(language);
                    }

                    return userEntry;
                },
                new { UserId = userId },
                splitOn: "Id"
            );

            if (userDictionary.Count == 0)
            {
                return NotFoundResult<UserDetailsResponse>.Create("User not found.");
            }
            

           return Result<UserDetailsResponse>.Success(userDictionary.Values.FirstOrDefault()!);

        }
    }
} 