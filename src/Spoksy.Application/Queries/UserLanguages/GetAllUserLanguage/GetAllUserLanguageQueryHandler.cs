using Dapper;
using Spoksy.Application.Responses;
using Spoksy.Application.Commons;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Queries.UserLanguages.GetAllUserLanguage
{
    public class GetAllUserLanguageQueryHandler : IGetAllUserLanguageQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public GetAllUserLanguageQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Result<List<UserLanguageResponse>>> Handle(Guid userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT 
                    ul.id,
                    ul.language as Code,
                    ul.proficiency_level as ProficiencyLevel,
                    ul.started_learning_on as StartedLearningOn
                FROM user_languages ul
                WHERE ul.user_id = @UserId";

            var languages = await connection.QueryAsync<UserLanguageResponse>(
                sql,
                new { UserId = userId });

            var result = languages.Select(language =>
            {
                language.Name = Language.GetByCode(language.Code)?.Name ?? "Unknown";
                return language;
            }).ToList();

            return Result<List<UserLanguageResponse>>.Success(result.ToList());
        }
    
    }
}
