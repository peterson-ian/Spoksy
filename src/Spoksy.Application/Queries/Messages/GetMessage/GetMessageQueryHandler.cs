using Dapper;
using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Queries.Messages.GetMessage
{
    public class GetMessageQueryHandler : IGetMessageQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IChatValidationService _chatValidationService;

        public GetMessageQueryHandler(IDbConnectionFactory connectionFactory, IChatValidationService chatValidationService)
        {
            _connectionFactory = connectionFactory;
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<MessageResponse>> Handle(Guid userId, Guid chatId, Guid messageId)
        {
            await _chatValidationService.EnsureUserAccessForChatAsync(userId, chatId);

            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                    SELECT 
                        m.id,
                        m.content,
                        m.sender_id AS SenderId,
                        m.sent_at AS SentAt,
                        m.edit_at AS EditAt,
                        m.language AS LanguageCode,
                        m.is_read AS IsRead
                    FROM messages m
                    WHERE m.chat_id = @ChatId AND NOT m.is_delete
                        AND m.id = @MessageId"
            ;

            var message = await connection.QueryFirstOrDefaultAsync<MessageResponse>(
                sql,
                new { ChatId = chatId, MessageId = messageId });

            if (message is null)
                return NotFoundResult<MessageResponse>.Failure("Message not found");

            if (!string.IsNullOrWhiteSpace(message.LanguageCode))
                message.Language = Language.GetByCode(message.LanguageCode);

            return Result<MessageResponse>.Success(message);
        }
    }
}
