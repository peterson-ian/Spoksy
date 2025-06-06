namespace Spoksy.Infrastructure.Integrations.IdentityProvider
{
    public interface IKeycloakTokenManager 
    {
        Task<string> GetAccessTokenAsync();
    }
}
