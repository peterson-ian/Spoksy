using Polly;
using Polly.Extensions.Http;

namespace Spoksy.Infrastructure.Integrations.IdentityProvider
{
    public static class KeycloakResiliencePolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> RetryPolicy =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(3, retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy =>
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));

        public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicies() =>
            Policy.WrapAsync(RetryPolicy, CircuitBreakerPolicy, TimeoutPolicy);
    }
} 