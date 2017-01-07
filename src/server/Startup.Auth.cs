using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Toucan.Service;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static void UseTokenBasedAuthentication(this IApplicationBuilder app, TokenProviderConfig cfg)
        {
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(cfg.TokenSecurityKey));
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // The signing key must match!
                IssuerSigningKey = signingKey,
                ValidateIssuer = true, // Validate the JWT Issuer (iss) claim
                ValidIssuer = cfg.TokenIssuer,
                ValidateAudience = true, // Validate the JWT Audience (aud) claim
                ValidAudience = cfg.TokenAudience,
                ValidateLifetime = true, // Validate the token expiry
                ClockSkew = TimeSpan.Zero // If you want to allow a certain amount of clock drift, set that here:
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                SaveToken = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new CustomJwtDataFormat(cfg.TokenSecurityAlgorithm, "Cookie", tokenValidationParameters)
            });
        }

    }
}
