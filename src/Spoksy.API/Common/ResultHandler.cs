using Microsoft.AspNetCore.Mvc;
using Spoksy.API.Models;
using Spoksy.Application.Commons;
using Spoksy.Application.Commons.Results;

namespace Spoksy.API.Common
{
    public static class ResultHandler
    {
        public static IActionResult Handle<T>(this Result<T> result, ControllerBase controller, string successMessage = null)
        {
            return result.IsSuccess
                ? controller.Ok(new ApiResponse<T>(result.Value, successMessage))
                : HandleError(result, controller);
        }

        private static IActionResult HandleError<T>(Result<T> result, ControllerBase controller)
        {
            return result switch
            {
                NotFoundResult<T> => controller.NotFound(new ApiResponse<object>(result.Errors)),
                ValidationResult<T> validationResult => controller.BadRequest(new ApiResponse<object>(result.Errors)),
                ConflictResult<T> => controller.Conflict(new ApiResponse<object>(result.Errors)),
                UnauthorizedResult<T> => controller.Unauthorized(new ApiResponse<object>(result.Errors)),
                ForbiddenResult<T> => controller.StatusCode(403, new ApiResponse<object>(result.Errors)),
                _ => controller.BadRequest(new ApiResponse<object>(result.Errors))
            };
        }

    }
} 