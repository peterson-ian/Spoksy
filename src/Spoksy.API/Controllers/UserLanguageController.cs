using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spoksy.API.Common;
using Spoksy.API.Models;
using Spoksy.Application.Commands.UserLanguages.CreateUserLanguage;
using Spoksy.Application.Commands.UserLanguages.UpdateUserLanguage;
using Spoksy.Application.Commands.UserLanguages.DeleteUserLanguage;
using Spoksy.Application.Queries.UserLanguages.GetAllUserLanguage;
using Spoksy.Application.Queries.UserLanguages.GetUserLanguage;
using Spoksy.Application.Responses;

namespace Spoksy.API.Controllers
{
    [Route("api/user/me/language/")]
    [ApiController]
    [Authorize]
    public class UserLanguageController : ControllerBase
    {
        private readonly IGetAllUserLanguageQueryHandler _getAllUserLanguageHandler;
        private readonly IGetUserLanguageQueryHandler _getUserLanguageHandler;
        private readonly ICreateUserLanguageCommandHandler _createUserLanguageHandler;
        private readonly IUpdateUserLanguageCommandHandler _updateUserLanguageHandler;
        private readonly IDeleteUserLanguageCommandHandler _deleteUserLanguageHandler;

        public UserLanguageController(
            IGetAllUserLanguageQueryHandler getAllUserLanguageHandler, 
            IGetUserLanguageQueryHandler getUserLanguageHandler, 
            ICreateUserLanguageCommandHandler createUserLanguageHandler, 
            IUpdateUserLanguageCommandHandler updateUserLanguageHandler, 
            IDeleteUserLanguageCommandHandler deleteUserLanguageHandler)
        {
            _getAllUserLanguageHandler = getAllUserLanguageHandler;
            _getUserLanguageHandler = getUserLanguageHandler;
            _createUserLanguageHandler = createUserLanguageHandler;
            _updateUserLanguageHandler = updateUserLanguageHandler;
            _deleteUserLanguageHandler = deleteUserLanguageHandler;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ApiResponse<List<UserLanguageResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _getAllUserLanguageHandler.Handle(userId!);

            return result.Handle(this);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserLanguageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _getUserLanguageHandler.Handle(userId!, id);

            return result.Handle(this);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserLanguageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateUserLanguageCommand command)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _createUserLanguageHandler.Handle(userId!, command);

            return result.Handle(this);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<UserLanguageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromBody] UpdateUserLanguageCommand command)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _updateUserLanguageHandler.Handle(userId!, command);

            return result.Handle(this);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserLanguageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value);

            var result = await _deleteUserLanguageHandler.Handle(userId!, id);

            return result.Handle(this);
        }
    }
}
