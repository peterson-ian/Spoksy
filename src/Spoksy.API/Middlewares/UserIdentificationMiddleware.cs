using Spoksy.API.Models;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace Spoksy.API.Middlewares
{
    public class UserIdentificationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserIdentificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var isAuthorized = endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;

            if (!isAuthorized)
            {
                await _next(context);
                return;
            }

            var providerId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(providerId))
            {
                await CreateApiResponse(context, 401, "JWT is missing a provider id claim");
                return;
            }

            var userRepository = context.RequestServices.GetRequiredService<IUserRepository>();
            var user = await userRepository.GetByIdentityProviderIdAsync(providerId);

            if (user == null)
            {
                await CreateApiResponse(context, 401, "User not found");
                return;
            }

            if (user.Status != UserStatus.Active)
            {
                await CreateApiResponse(context, 403, $"User is {user.Status}");
                return;
            }

            context.Items["UserId"] = user.Id;

            await _next(context);
        }

        public async Task CreateApiResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<object>(message);
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}