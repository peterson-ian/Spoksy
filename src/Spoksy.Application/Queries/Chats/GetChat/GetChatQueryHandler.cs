using Dapper;
using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;
using Spoksy.Application.Queries.Messages.GetAllMessage;
using Spoksy.Application.Responses;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Application.Queries.Chats.GetChat
{
    public class GetChatQueryHandler : IGetChatQueryHandler
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IGetAllMessageQueryHandler _getAllMessageQueryHandler;
        private readonly IChatValidationService _chatValidationService;

        public GetChatQueryHandler(IDbConnectionFactory connectionFactory, IGetAllMessageQueryHandler getAllMessageQueryHandler, IChatValidationService chatValidationService)
        {
            _connectionFactory = connectionFactory;
            _getAllMessageQueryHandler = getAllMessageQueryHandler;
            _chatValidationService = chatValidationService;
        }

        public async Task<Result<ChatResponse>> Handle(Guid userId, Guid chatId)
        {
            await _chatValidationService.EnsureUserAccessForChatAsync(userId, chatId);

            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                 SELECT 
                    c.id,
                    c.primary_language AS PrimaryLanguageCode,
                    c.secondary_language AS SecondaryLanguageCode,
                    c.status AS Status,
                    c.created_at AS CreatedAt,
                    c.last_active_at AS LastActivityAt,
                    cp.id AS Id,
                    cp.user_id AS UserId,
                    cp.join_at AS JoinAt,
                    cp.leave_at AS LeaveAt,
                    u.name AS Name
                FROM chats c
                JOIN chat_participants cp ON cp.chat_id = c.id
                JOIN users u ON u.id = cp.user_id
                WHERE c.id = @ChatId AND u.id != @UserId";

            var chatDictionary = new Dictionary<Guid, ChatResponse>();

            var chat = await connection.QueryAsync<ChatResponse, ChatParticipantResponse, ChatResponse>(
                sql,
                (chat, participant) =>
                {
                    if (!chatDictionary.TryGetValue(chat.Id, out var chatEntry))
                    {
                        chatEntry = chat;
                        chatEntry.PrimaryLanguage = Language.GetByCode(chat.PrimaryLanguageCode);
                        chatEntry.SecondaryLanguage = Language.GetByCode(chat.SecondaryLanguageCode);
                        chatDictionary.Add(chat.Id, chatEntry);
                        chatEntry.Recipient = participant;
                    }

                    return chatEntry;
                },
                new { UserId = userId, ChatId = chatId },
                splitOn: "Id"
            );

            if (chatDictionary.Count == 0)
            {
                return NotFoundResult<ChatResponse>.Create("Chat not found.");
            }

            var result = chatDictionary.Values.FirstOrDefault()!;
            var messagesResult = await _getAllMessageQueryHandler.Handle(userId, chatId, 1);
            result.Messages = messagesResult.Value;

            return Result<ChatResponse>.Success(chatDictionary.Values.FirstOrDefault()!);
        }
    }
}
