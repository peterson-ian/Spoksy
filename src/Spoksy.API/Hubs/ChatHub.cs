using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Spoksy.Application.Commands.Messages.SendMessage;
using Spoksy.Application.Commands.Messages.EditMessage;
using Spoksy.Application.Queries.ChatParticipants.CheckParticipantCanAccessChat;
using Spoksy.Application.Commands.Messages.DeleteMessage;

namespace Spoksy.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ISendMessageCommandHandler _sendMessageHandler;
        private readonly IEditMessageCommmandHandler _editMessageHandler;
        private readonly IDeleteMessageCommandHandler _deleteMessageHandler;
        private readonly ICheckParticipantCanAccessChatQueryHandler _checkParticipantCanAccessChatHandler;

        public ChatHub(
            ISendMessageCommandHandler sendMessageHandler, 
            IEditMessageCommmandHandler editMessageHandler, 
            IDeleteMessageCommandHandler deleteMessageHandler, 
            ICheckParticipantCanAccessChatQueryHandler checkParticipantCanAccessChatHandler)
        {
            _sendMessageHandler = sendMessageHandler;
            _editMessageHandler = editMessageHandler;
            _deleteMessageHandler = deleteMessageHandler;
            _checkParticipantCanAccessChatHandler = checkParticipantCanAccessChatHandler;
        }

        public async Task JoinChat(Guid chatId)
        {
            var userId = Guid.Parse(Context.User.FindFirst("user_id")?.Value);

            var result = await _checkParticipantCanAccessChatHandler.Handle(userId, chatId);

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task SendMessage(Guid chatId, string message)
        {
            var userId = Guid.Parse(Context.User.FindFirst("user_id")?.Value);

            SendMessageCommand command = new SendMessageCommand
            {
                ChatId = chatId,
                Content = message
            };
            var result = await _sendMessageHandler.Handle(userId, command);

            if(result.IsSuccess)
                await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", result);
        }

        public async Task EditMessage(Guid chatId, Guid messageId, string message)
        {
            var userId = Guid.Parse(Context.User.FindFirst("user_id")?.Value);

            EditMessageCommand command = new EditMessageCommand
            {
                ChatId = chatId,
                MessageId = messageId,
                Content = message
            };
            var result = await _editMessageHandler.Handle(userId, command);

            if (result.IsSuccess)
                await Clients.Group(chatId.ToString()).SendAsync("MessageEdited", result);
        }

        public async Task DeleteMessage(Guid chatId, Guid messageId)
        {
            var userId = Guid.Parse(Context.User.FindFirst("user_id")?.Value);

            DeleteMessageCommand command = new DeleteMessageCommand
            {
                ChatId = chatId,
                MessageId = messageId
            };
            var result = await _deleteMessageHandler.Handle(userId, command);

            if (result.IsSuccess)
                await Clients.Group(chatId.ToString()).SendAsync("MessageDeleted", result);
        }

        public async Task LeaveChat(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }

}