using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Spoksy.Domain.Contracts;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;

namespace Spoksy.Infrastructure.Integrations.IdentityProvider
{
    public class KeycloakIdentityProviderIntegration : IIdentityProviderIntegration
    {
        private readonly HttpClient _client;
        private readonly IKeycloakTokenManager _tokenManager;
        private readonly KeycloakConfiguration _configuration;
        private readonly string _usersEndpoint;

        public KeycloakIdentityProviderIntegration(
            HttpClient client,
            IKeycloakTokenManager tokenManager,
            IOptions<KeycloakConfiguration> options)
        {
            _client = client;
            _tokenManager = tokenManager;
            
            _configuration = options.Value;

            var baseUrl = _configuration.AuthServerUrl.TrimEnd('/');
            _usersEndpoint = $"{baseUrl}/admin/realms/{_configuration.Realm}/users";
        }

        public async Task<string?> CreateUserAsync(string name, string email, bool isActive, string password)
        {
            var accessToken = await _tokenManager.GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var random = new Random();
            int number = random.Next(1000, 9999);

            var keycloakUser = new
            {
                username = $"{name.Split(' ').FirstOrDefault().ToLower()}{number}" ,
                email = email,
                enabled = isActive,
                emailVerified = false,
                firstName = name.Split(' ').FirstOrDefault() ?? string.Empty,
                lastName = string.Join(" ", name.Split(' ').Skip(1)) ?? string.Empty,
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = password,
                        temporary = false
                    }
                }
            };

            var response = await _client.PostAsJsonAsync(_usersEndpoint, keycloakUser);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new IntegrationException("Failed to create user in Keycloak");
            }

            var location = response.Headers.Location;
            var userId = location?.Segments.Last().Trim();

            return userId;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var accessToken = await _tokenManager.GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.DeleteAsync($"{_usersEndpoint}/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new IntegrationException("Failed to delete user in Keycloak");
            }

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            var accessToken = await _tokenManager.GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var resetRequest = new
            {
                type = "password",
                value = newPassword,
                temporary = false
            };

            var response = await _client.PutAsJsonAsync($"{_usersEndpoint}/{userId}/reset-password", resetRequest);
            if (!response.IsSuccessStatusCode)
            {
                throw new IntegrationException("Failed to reset password in Keycloak");
            }

            return true;
        }

        public async Task<bool> SetUserEnabledAsync(string userId, bool enabled)
        {
            var accessToken = await _tokenManager.GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var updateRequest = new { enabled = enabled };
            var response = await _client.PutAsJsonAsync($"{_usersEndpoint}/{userId}", updateRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new IntegrationException("Failed to update user state in Keycloak");
            }

            return true;
        }

        public async Task<bool> UpdateUserAsync(string userId, User user)
        {
            var accessToken = await _tokenManager.GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var updateRequest = new
            {
                firstName = user.Name.Split(' ').FirstOrDefault() ?? string.Empty,
                lastName = string.Join(" ", user.Name.Split(' ').Skip(1)) ?? string.Empty,
                email = user.Email,
                enabled = user.IsActive()
            };

            var response = await _client.PutAsJsonAsync($"{_usersEndpoint}/{userId}", updateRequest);
            if (!response.IsSuccessStatusCode)
            {
                throw new IntegrationException("Failed to update user in Keycloak");
            }

            return true;
        }

        public async Task<bool> UpdateUserEmailAsync(string userId, string newEmail)
        {
            var accessToken = await _tokenManager.GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var updateRequest = new
            {
                email = newEmail,
                username = newEmail
            };

            var response = await _client.PutAsJsonAsync($"{_usersEndpoint}/{userId}", updateRequest);
            if (!response.IsSuccessStatusCode)
            {
                throw new IntegrationException("Failed to update user email in Keycloak");
            }

            return true;
        }
    }
}
