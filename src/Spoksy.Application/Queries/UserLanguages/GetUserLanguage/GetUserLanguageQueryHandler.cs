using Dapper;
using Spoksy.Application.Responses;
using Spoksy.Application.Commons;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Queries.UserLanguages.GetUserLanguage
{
    public class GetUserLanguageQueryHandler : IGetUserLanguageQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public GetUserLanguageQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Result<UserLanguageReponse>> Handle(Guid userId, Guid userLanguageId)
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

            var language = await connection.QueryFirstOrDefaultAsync<UserLanguageReponse>(
                sql,
                new { UserId = userId, UserLanguageId = userLanguageId });

            if (language is null)
                return Result<UserLanguageReponse>.Failure("User language not found");

            language.Name = Language.GetByCode(language.Code)?.Name ?? "Unknown";

            return Result<UserLanguageReponse>.Success(language);
        }
    
    }
}
