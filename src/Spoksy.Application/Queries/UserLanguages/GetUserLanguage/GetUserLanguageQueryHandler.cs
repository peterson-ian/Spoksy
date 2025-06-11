using Dapper;
using Spoksy.Application.Responses;
using Spoksy.Application.Commons;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;
using Spoksy.Application.Commons.Results;

namespace Spoksy.Application.Queries.UserLanguages.GetUserLanguage
{
    public class GetUserLanguageQueryHandler : IGetUserLanguageQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public GetUserLanguageQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Result<UserLanguageResponse>> Handle(Guid userId, Guid userLanguageId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                    SELECT 
                        ul.id,
                        ul.language AS Code,
                        ul.proficiency_level AS ProficiencyLevel,
                        ul.started_learning_on AS StartedLearningOn
                    FROM user_languages ul
                    WHERE ul.user_id = @UserId 
                      AND ul.id = @UserLanguageId";

            var language = await connection.QueryFirstOrDefaultAsync<UserLanguageResponse>(
                sql,
                new { UserId = userId, UserLanguageId = userLanguageId });

            if (language is null)
                return NotFoundResult<UserLanguageResponse>.Failure("User language not found");

            language.Name = Language.GetByCode(language.Code)?.Name ?? "Unknown";

            return Result<UserLanguageResponse>.Success(language);
        }
    
    }
}
