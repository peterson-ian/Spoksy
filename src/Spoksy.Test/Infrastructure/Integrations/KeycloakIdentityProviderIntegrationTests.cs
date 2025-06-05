using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;
using Spoksy.Domain.ValueObjects;
using Spoksy.Infrastructure.Integrations.IdentityProvider;
using System.Net;
using System.Text.Json;

namespace Spoksy.Test.Infrastructure.Integrations
{
    public class KeycloakIdentityProviderIntegrationTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<IKeycloakTokenManager> _tokenManagerMock;
        private readonly KeycloakConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly KeycloakIdentityProviderIntegration _integration;

        public KeycloakIdentityProviderIntegrationTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _tokenManagerMock = new Mock<IKeycloakTokenManager>();
            _configuration = new KeycloakConfiguration
            {
                AuthServerUrl = "http://localhost:8080",
                Realm = "master",
                Resource = "spoksy-admin-client",
                Secret = "test-secret"
            };

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _integration = new KeycloakIdentityProviderIntegration(
                _httpClient,
                _tokenManagerMock.Object,
                Options.Create(_configuration)
            );

            _tokenManagerMock.Setup(x => x.GetAccessTokenAsync())
                .ReturnsAsync("test-token");
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldReturnUserId()
        {
            var name = "John Doe";
            var email = "john.doe@example.com";
            var password = "StrongP@ssw0rd";
            var userId = Guid.NewGuid().ToString();

            SetupHttpResponse(HttpStatusCode.Created, "", new Dictionary<string, string>
            {
                { "Location", $"http://localhost:8080/admin/realms/master/users/{userId}" }
            });

            var result = await _integration.CreateUserAsync(name, email, true, password);

            Assert.Equal(userId, result);
            VerifyHttpRequest(HttpMethod.Post, "/admin/realms/master/users");
        }

        [Fact]
        public async Task CreateUserAsync_WhenKeycloakFails_ShouldThrowIntegrationException()
        {
            var name = "John Doe";
            var email = "john.doe@example.com";
            var password = "StrongP@ssw0rd";

            SetupHttpResponse(HttpStatusCode.BadRequest, "Invalid data");

            await Assert.ThrowsAsync<IntegrationException>(() =>
                _integration.CreateUserAsync(name, email, true, password));
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidUserId_ShouldReturnTrue()
        {
            var userId = Guid.NewGuid().ToString();
            SetupHttpResponse(HttpStatusCode.NoContent, "");

            var result = await _integration.DeleteUserAsync(userId);

            Assert.True(result);
            VerifyHttpRequest(HttpMethod.Delete, $"/admin/realms/master/users/{userId}");
        }

        [Fact]
        public async Task DeleteUserAsync_WhenKeycloakFails_ShouldThrowIntegrationException()
        {
            var userId = Guid.NewGuid().ToString();
            SetupHttpResponse(HttpStatusCode.NotFound, "User not found");

            await Assert.ThrowsAsync<IntegrationException>(() =>
                _integration.DeleteUserAsync(userId));
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidData_ShouldReturnTrue()
        {
            var userId = Guid.NewGuid().ToString();
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                userId
            );

            SetupHttpResponse(HttpStatusCode.NoContent, "");

            var result = await _integration.UpdateUserAsync(userId, user);

            Assert.True(result);
            VerifyHttpRequest(HttpMethod.Put, $"/admin/realms/master/users/{userId}");
        }

        [Fact]
        public async Task UpdateUserAsync_WhenKeycloakFails_ShouldThrowIntegrationException()
        {
            var userId = Guid.NewGuid().ToString();
            var user = new User(
                "John Doe",
                "john.doe@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("US"),
                userId
            );

            SetupHttpResponse(HttpStatusCode.BadRequest, "Invalid data");

            await Assert.ThrowsAsync<IntegrationException>(() =>
                _integration.UpdateUserAsync(userId, user));
        }

        [Fact]
        public async Task ResetPasswordAsync_WithValidData_ShouldReturnTrue()
        {
            var userId = Guid.NewGuid().ToString();
            var newPassword = "NewStrongP@ssw0rd";

            SetupHttpResponse(HttpStatusCode.NoContent, "");

            var result = await _integration.ResetPasswordAsync(userId, newPassword);

            Assert.True(result);
            VerifyHttpRequest(HttpMethod.Put, $"/admin/realms/master/users/{userId}/reset-password");
        }

        [Fact]
        public async Task ResetPasswordAsync_WhenKeycloakFails_ShouldThrowIntegrationException()
        {
            var userId = Guid.NewGuid().ToString();
            var newPassword = "NewStrongP@ssw0rd";

            SetupHttpResponse(HttpStatusCode.BadRequest, "Invalid password");

            await Assert.ThrowsAsync<IntegrationException>(() =>
                _integration.ResetPasswordAsync(userId, newPassword));
        }

        [Fact]
        public async Task SetUserEnabledAsync_WithValidData_ShouldReturnTrue()
        {
            var userId = Guid.NewGuid().ToString();
            SetupHttpResponse(HttpStatusCode.NoContent, "");

            var result = await _integration.SetUserEnabledAsync(userId, true);

            Assert.True(result);
            VerifyHttpRequest(HttpMethod.Put, $"/admin/realms/master/users/{userId}");
        }

        [Fact]
        public async Task SetUserEnabledAsync_WhenKeycloakFails_ShouldThrowIntegrationException()
        {
            var userId = Guid.NewGuid().ToString();
            SetupHttpResponse(HttpStatusCode.BadRequest, "Invalid request");

            await Assert.ThrowsAsync<IntegrationException>(() =>
                _integration.SetUserEnabledAsync(userId, true));
        }

        [Fact]
        public async Task UpdateUserEmailAsync_WithValidData_ShouldReturnTrue()
        {
            var userId = Guid.NewGuid().ToString();
            var newEmail = "newemail@example.com";

            SetupHttpResponse(HttpStatusCode.NoContent, "");

            var result = await _integration.UpdateUserEmailAsync(userId, newEmail);

            Assert.True(result);
            VerifyHttpRequest(HttpMethod.Put, $"/admin/realms/master/users/{userId}");
        }

        [Fact]
        public async Task UpdateUserEmailAsync_WhenKeycloakFails_ShouldThrowIntegrationException()
        {
            var userId = Guid.NewGuid().ToString();
            var newEmail = "newemail@example.com";

            SetupHttpResponse(HttpStatusCode.BadRequest, "Invalid email");

            await Assert.ThrowsAsync<IntegrationException>(() =>
                _integration.UpdateUserEmailAsync(userId, newEmail));
        }

        private void SetupHttpResponse(HttpStatusCode statusCode, string content, Dictionary<string, string>? headers = null)
        {
            var response = new HttpResponseMessage(statusCode);
            if (!string.IsNullOrEmpty(content))
            {
                response.Content = new StringContent(content);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    response.Headers.Add(header.Key, header.Value);
                }
            }

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        private void VerifyHttpRequest(HttpMethod method, string path)
        {
            _httpMessageHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri.ToString().EndsWith(path)),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
} 