
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Spoksy.Infrastructure.Configurations
{
    public static class AuthenticationConfigurations
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authority = $"{configuration["AUTH_SERVER_URL"].TrimEnd('/')}/realms/{configuration["AUTH_REALM"]}" ?? throw new ArgumentException("Invalid authority");
            var audience = configuration["AUTH_CLIENT"] ?? throw new ArgumentException("Invalid audience");

             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authority,
                        ValidAudience = audience,
                        ClockSkew = TimeSpan.FromMinutes(2) 
                    };
                });

            return services;
        }

    }
}
