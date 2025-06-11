using Dapper;
using Spoksy.Application.Commons;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Queries.Messages.GetAllMessage
{
    public class GetAllMessageQueryHandler : IGetAllMessageQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IChatValidationService _chatValidationService;
        private readonly int _pageSize = 20;

        public GetAllMessageQueryHandler(IDbConnectionFactory connectionFactory, IChatValidationService chatValidationService)
        {
            _connectionFactory = connectionFactory;
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<PaginatedResponse<MessageResponse>>> Handle(Guid userId, Guid chatId, int page)
        {
            await _chatValidationService.EnsureUserAccessForChatAsync(userId, chatId);

            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                 SELECT 
                    m.id AS Id,
                    m.content,
                    m.sender_id AS SenderId,
                    m.sent_at AS SentAt,
                    m.edit_at AS EditAt,
                    m.language AS LanguageCode,
                    m.is_read AS IsRead
                FROM messages m
                WHERE m.chat_id = @ChatId AND NOT m.is_delete
                ORDER BY m.sent_at DESC
                OFFSET @Offset LIMIT @Limit;
            ";

            var offset = (page - 1) * _pageSize;
            var messages = (await connection.QueryAsync<MessageResponse>(
                sql,
                new { ChatId = chatId, Offset = offset, Limit = _pageSize }
            )).Select(m =>
            {
                if (!string.IsNullOrWhiteSpace(m.LanguageCode))
                    m.Language = Language.GetByCode(m.LanguageCode);
                return m;
            }).ToList();

            const string sqlCountTotal = @"
                SELECT COUNT(*) 
                FROM messages m 
                WHERE m.chat_id = @ChatId AND NOT m.is_delete;
            ";

            var total = await connection.ExecuteScalarAsync<long>(sqlCountTotal, new { ChatId = chatId });

            var result = new PaginatedResponse<MessageResponse>(
                items: messages,
                page: page,
                pageSize: _pageSize,
                totalItems: total
            );

            return Result<PaginatedResponse<MessageResponse>>.Success(result);
        }
    }
}
