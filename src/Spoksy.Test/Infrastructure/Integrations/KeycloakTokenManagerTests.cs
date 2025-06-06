using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Spoksy.Domain.Exceptions;
using Spoksy.Infrastructure.Integrations.IdentityProvider;
using System.Net;
using System.Text.Json;

namespace Spoksy.Test.Infrastructure.Integrations
{
    public class KeycloakTokenManagerTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly KeycloakConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly KeycloakTokenManager _tokenManager;

        public KeycloakTokenManagerTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _configuration = new KeycloakConfiguration
            {
                AuthServerUrl = "http://localhost:8080",
                Realm = "master",
                Resource = "spoksy-admin-client",
                Secret = "test-secret"
            };

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _tokenManager = new KeycloakTokenManager(
                _httpClient,
                Options.Create(_configuration)
            );
        }

        [Fact]
        public async Task GetAccessTokenAsync_ShouldReturnValidToken()
        {
            var accessToken = "valid-access-token";
            var expiresIn = 300; 

            SetupTokenResponse(accessToken, expiresIn);

            var result = await _tokenManager.GetAccessTokenAsync();

            Assert.Equal(accessToken, result);
            VerifyTokenRequest();
        }

        [Fact]
        public async Task GetAccessTokenAsync_WhenTokenNotExpired_ShouldReuseToken()
        {
            var accessToken = "valid-access-token";
            var expiresIn = 300; 

            SetupTokenResponse(accessToken, expiresIn);

            var firstResult = await _tokenManager.GetAccessTokenAsync();

            var secondResult = await _tokenManager.GetAccessTokenAsync();

            Assert.Equal(accessToken, firstResult);
            Assert.Equal(accessToken, secondResult);
            VerifyTokenRequest(Times.Once()); 
        }

        [Fact]
        public async Task GetAccessTokenAsync_WhenTokenExpired_ShouldGetNewToken()
        {
            var firstToken = "first-access-token";
            var secondToken = "second-access-token";
            var expiresIn = 0; 

            SetupTokenResponse(firstToken, expiresIn);

            var firstResult = await _tokenManager.GetAccessTokenAsync();

            SetupTokenResponse(secondToken, 300);

            var secondResult = await _tokenManager.GetAccessTokenAsync();

            Assert.Equal(firstToken, firstResult);
            Assert.Equal(secondToken, secondResult);
            VerifyTokenRequest(Times.Exactly(2)); 
        }

        [Fact]
        public async Task GetAccessTokenAsync_WhenKeycloakFails_ShouldThrowIntegrationException()
        {
            SetupHttpResponse(HttpStatusCode.BadRequest, "Invalid client credentials");

            await Assert.ThrowsAsync<IntegrationException>(() =>
                _tokenManager.GetAccessTokenAsync());
        }

        private void SetupTokenResponse(string accessToken, int expiresIn)
        {
            var tokenResponse = new
            {
                access_token = accessToken,
                expires_in = expiresIn,
                token_type = "Bearer"
            };

            var json = JsonSerializer.Serialize(tokenResponse);
            SetupHttpResponse(HttpStatusCode.OK, json);
        }

        private void SetupHttpResponse(HttpStatusCode statusCode, string content)
        {
            var response = new HttpResponseMessage(statusCode);
            if (!string.IsNullOrEmpty(content))
            {
                response.Content = new StringContent(content);
            }

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        private void VerifyTokenRequest(Times? times = null)
        {
            _httpMessageHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    times ?? Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/realms/master/protocol/openid-connect/token")),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
} 