using Microsoft.Extensions.Options;
using Spoksy.Domain.Exceptions;
using System.Net.Http.Json;
using System.Text.Json;

namespace Spoksy.Infrastructure.Integrations.IdentityProvider
{
    public class KeycloakTokenManager : IKeycloakTokenManager
    {
        private readonly HttpClient _client;
        private readonly KeycloakConfiguration _configuration;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private readonly string _tokenEndpoint;
        private string? _accessToken;
        private string? _refreshToken;
        private DateTime _expiresAt;

        public KeycloakTokenManager(HttpClient client, IOptions<KeycloakConfiguration> options)
        {
            _client = client;
            _configuration = options.Value;
            var baseUrl = _configuration.AuthServerUrl.TrimEnd('/');
            _tokenEndpoint = $"{baseUrl}/realms/{_configuration.Realm}/protocol/openid-connect/token";
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (IsTokenValid())
            {
                return _accessToken!;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (IsTokenValid())
                {
                    return _accessToken!;
                }

                return await GetNewAccessTokenAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(_accessToken) && _expiresAt > DateTime.UtcNow.AddMinutes(1);
        }

        private async Task<string> GetNewAccessTokenAsync()
        {
            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _configuration.Resource,
                ["client_secret"] = _configuration.Secret
            };

            var response = await _client.PostAsync(_tokenEndpoint, new FormUrlEncodedContent(tokenRequest));
            if (!response.IsSuccessStatusCode)
            {
                throw new IntegrationException("Failed to authenticate with Keycloak");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            UpdateTokens(tokenResponse);
            return _accessToken!;
        }

        private void UpdateTokens(JsonElement tokenResponse)
        {
            _accessToken = tokenResponse.GetProperty("access_token").GetString();

            var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();
            _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
        }
    }

}
