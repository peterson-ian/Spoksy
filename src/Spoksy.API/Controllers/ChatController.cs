using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spoksy.API.Common;
using Spoksy.API.Models;
using Spoksy.Application.Commands.Chats.CreateChat;
using Spoksy.Application.Queries.Chats.GetAllChat;
using Spoksy.Application.Queries.Chats.GetChat;
using Spoksy.Application.Queries.Messages.GetAllMessage;
using Spoksy.Application.Queries.Messages.GetMessage;
using Spoksy.Application.Responses;
using Spoksy.Domain.Entities;

namespace Spoksy.API.Controllers
{
    [Route("api/chat/")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ICreateChatCommandHandler _createChatHandler;

        private readonly IGetChatQueryHandler _getChatQueryHandler;
        private readonly IGetAllChatQueryHandler _getAllChatQueryHandler;
        private readonly IGetMessageQueryHandler _getMessageQueryHandler;
        private readonly IGetAllMessageQueryHandler _getAllMessageQueryHandler;

        public ChatController(
            ICreateChatCommandHandler createChatHandler, 
            IGetChatQueryHandler getChatQueryHandler, 
            IGetAllChatQueryHandler getAllChatQueryHandler, 
            IGetMessageQueryHandler getMessageQueryHandler, 
            IGetAllMessageQueryHandler getAllMessageQueryHandler)
        {
            _createChatHandler = createChatHandler;
            _getChatQueryHandler = getChatQueryHandler;
            _getAllChatQueryHandler = getAllChatQueryHandler;
            _getMessageQueryHandler = getMessageQueryHandler;
            _getAllMessageQueryHandler = getAllMessageQueryHandler;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ChatIdentificatorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateChatCommand command)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _createChatHandler.Handle(userId!, command);

            return result.Handle(this);
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ChatIdentificatorResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int page)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _getAllChatQueryHandler.Handle(userId!, page);

            return result.Handle(this);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ChatResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _getChatQueryHandler.Handle(userId!, id);

            return result.Handle(this);
        }

        [HttpGet("{id}/message")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<MessageResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMessage(Guid id, [FromQuery] int page = 1)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _getAllMessageQueryHandler.Handle(userId!, id, page);

            return result.Handle(this);
        }

        [HttpGet("{id}/message/{messageId}")]
        [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMessageById(Guid id, Guid messageId)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _getMessageQueryHandler.Handle(userId!, id, messageId);

            return result.Handle(this);
        }

    }
}
