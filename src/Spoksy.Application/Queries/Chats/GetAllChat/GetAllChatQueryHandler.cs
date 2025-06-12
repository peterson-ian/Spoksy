using Dapper;
using Spoksy.Application.Commons;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Queries.Chats.GetAllChat
{
    public class GetAllChatQueryHandler : IGetAllChatQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly int _pageSize = 20;

        public GetAllChatQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Result<PaginatedResponse<ChatIdentificatorResponse>>> Handle(Guid userId, int page, ChatFilter? chatFilter = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var offset = (page - 1) * _pageSize;

            var sql = @"
                WITH user_chats AS (
                    SELECT c.*
                    FROM chats c
                    JOIN chat_participants cp ON cp.chat_id = c.id
                    WHERE cp.user_id = @UserId
                    ORDER BY c.last_active_at DESC
                    LIMIT @Limit OFFSET @Offset 
                ),
                recipients AS (
                    SELECT 
                        cp.chat_id,
                        u.id AS recipient_user_id,
                        u.name AS recipient_name,
                        cp.join_at AS recipient_join_at,
                        cp.leave_at AS recipient_leave_at
                    FROM chat_participants cp
                    JOIN users u ON u.id = cp.user_id
                    WHERE cp.user_id != @UserId AND cp.chat_id IN (SELECT id FROM user_chats)
                ),
                last_messages AS (
                    SELECT
                        m.chat_id,
                        m.id AS last_message_id,
                        m.content AS last_message_content,
                        m.sent_at AS last_message_sent_at,
                        m.edit_at AS last_message_edit_at,
                        m.is_read AS last_message_is_read,
                        m.sender_id AS last_message_sender_id,
                        m.language AS last_message_language
                    FROM (
                        SELECT 
                            m.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY m.chat_id 
                                ORDER BY m.sent_at DESC, m.id DESC
                            ) AS rn
                        FROM messages m
                        WHERE m.chat_id IN (SELECT id FROM user_chats) AND NOT m.is_delete
                    ) m
                    WHERE m.rn = 1
                )
                SELECT 
                    uc.id,
                    uc.primary_language AS PrimaryLanguageCode,
                    uc.secondary_language AS SecondaryLanguageCode,
                    uc.status AS Status,
                    uc.created_at AS CreatedAt,
                    uc.last_active_at AS LastActivityAt,

                    r.recipient_user_id AS UserId,
                    r.recipient_name AS Name,
                    r.recipient_join_at AS JoinAt,
                    r.recipient_leave_at AS tLeaveAt,

                    lm.last_message_id AS Id,
                    lm.last_message_content AS Content,
                    lm.last_message_sent_at AS SentAt,
                    lm.last_message_edit_at AS EditAt,
                    lm.last_message_is_read AS IsRead,
                    lm.last_message_sender_id AS SenderId,
                    lm.last_message_language AS LanguageCode
                FROM user_chats uc
                INNER JOIN recipients r ON r.chat_id = uc.id
                INNER JOIN last_messages lm ON lm.chat_id = uc.id
                ORDER BY uc.last_active_at DESC;
            ";

            var chats = (await connection.QueryAsync<ChatIdentificatorResponse, ChatParticipantResponse, MessageResponse, ChatIdentificatorResponse>(
                 sql,
                 (chat, recipient, lastMessage) =>
                 {
                     chat.PrimaryLanguage = Language.GetByCode(chat.PrimaryLanguageCode);
                     chat.SecondaryLanguage = Language.GetByCode(chat.SecondaryLanguageCode);
                     chat.Recipient = recipient;
                     chat.LastMessage = lastMessage;
                     if (chat.LastMessage != null && !string.IsNullOrWhiteSpace(chat.LastMessage.LanguageCode))
                     {
                         chat.LastMessage.Language = Language.GetByCode(chat.LastMessage.LanguageCode);
                     }

                     return chat;
                 },
                 param: new { UserId = userId, Offset = offset, Limit = _pageSize },
                 splitOn: "UserId,Id"
             )).ToList();

            var sqlCountTotal = @"
                SELECT 
                    COUNT(*) 
                FROM chats c 
                INNER JOIN chat_participants cp ON c.id = cp.chat_id AND cp.user_id = @UserId;
            ";

            var total = await connection.ExecuteScalarAsync<long>(sqlCountTotal, new { UserId = userId });

            var paginatedResult = new PaginatedResponse<ChatIdentificatorResponse>(
                items: chats,
                page: page,
                pageSize: _pageSize,
                totalItems: total
            );

            return Result<PaginatedResponse<ChatIdentificatorResponse>>.Success(paginatedResult);
        }
    }
}
