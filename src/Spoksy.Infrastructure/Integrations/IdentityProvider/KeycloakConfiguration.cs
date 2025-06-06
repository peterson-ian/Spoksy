namespace Spoksy.Infrastructure.Integrations.IdentityProvider
{
    public class KeycloakConfiguration
    {
        public string Realm { get; set; } = string.Empty;
        public string AuthServerUrl { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
} 