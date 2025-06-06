using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spoksy.API.Common;
using Spoksy.API.Models;
using Spoksy.Application.Commands.Users.CreateUser;
using Spoksy.Application.Commands.Users.DeactivateUser;
using Spoksy.Application.Commands.Users.UpdateUser;
using Spoksy.Application.Queries.Users.GetUser;
using Spoksy.Application.Responses;

namespace Spoksy.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserControllers : ControllerBase
    {
        private readonly ICreateUserCommandHandler _createUserHandler;
        private readonly IUpdateUserCommandHandler _updateUserHandler;
        private readonly IDeactivateUserCommandHandler _deactivateUserHandler;

        private readonly IGetUserQueryHandler _getUserHandler;

        public UserControllers(ICreateUserCommandHandler createUserHandler, IUpdateUserCommandHandler updateUserHandler, IDeactivateUserCommandHandler deactivateUserHandler, IGetUserQueryHandler getUserHandler)
        {
            _createUserHandler = createUserHandler;
            _updateUserHandler = updateUserHandler;
            _deactivateUserHandler = deactivateUserHandler;
            _getUserHandler = getUserHandler;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser()
        {
            var userId = (Guid) HttpContext.Items["UserId"];

            var result = await _getUserHandler.Handle(userId!);

            return result.Handle(this);
        }

        [HttpPut("me")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUser(UpdateUserCommand updateUserCommand)
        {
            var userId = (Guid) HttpContext.Items["UserId"];

            var result = await _updateUserHandler.Handle(userId!, updateUserCommand);

            return result.Handle(this);
        }

        [HttpPost("me/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUser()
        {
            var userId = (Guid) HttpContext.Items["UserId"];

            var result = await _deactivateUserHandler.Handle(userId!);

            return result.Handle(this);
        }


        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<UserDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var result = await _createUserHandler.Handle(command);

            return result.Handle(this);
        }
    }
}
