using System.Net;
using System.Text.Json;
using Spoksy.API.Models;
using Spoksy.Domain.Exceptions;

namespace Spoksy.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<object>(GetMessageForException(exception));

            switch (exception)
            {
                case DomainException:
                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    if (!_env.IsProduction())
                    {
                        response = new ApiResponse<object>("Internal Server Error", 
                            new[] { exception.Message, exception.StackTrace });
                    }
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private string GetMessageForException(Exception exception) => exception switch
        {
            DomainException => exception.Message,
            ArgumentException => "Invalid parameters",
            UnauthorizedAccessException => "Unauthorized access",
            KeyNotFoundException => "Resource not found",
            _ => "An unexpected error occurred"
        };
    }
}