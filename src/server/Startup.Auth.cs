using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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
                TokenValidationParameters = tokenValidationParameters,
                Events = new JwtBearerEvents
                {
                    OnChallenge = OnChallenge
                }
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new Model.TokenDataFormat(cfg.TokenSecurityAlgorithm, "Cookie", tokenValidationParameters)
            });
        }

        /// Do a custom response status code return url to match the original 'referrer', rather than the endpoint being called ...
        private static async Task OnChallenge(JwtBearerChallengeContext context)
        {
            if (context.Response.StatusCode == 302)
            {
                Uri location = context.Response.GetTypedHeaders().Location;
                string referrer = context.Request.Headers[HeaderNames.Referer];

                if (location != null && !string.IsNullOrEmpty(referrer))
                {
                    context.Response.Headers.Remove(HeaderNames.Location);

                    UriBuilder mod = new UriBuilder(location.Scheme, location.Host, location.Port, "Login");
                    mod.Query = "returnUrl=" + referrer;

                    if (!string.IsNullOrEmpty(context.Error))
                    {
                        mod.Query = mod.Query + "&errorCode=" + context.Error;
                    }

                    if (!string.IsNullOrEmpty(context.ErrorDescription))
                    {
                        mod.Query = mod.Query + "&errorDesc=" + context.ErrorDescription;
                    }

                    context.Response.Headers.Append(HeaderNames.Location, mod.Uri.ToString());
                    context.Response.StatusCode = 200;
                }
            }

            context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);

            await Task.Factory.StartNew(() => context.HandleResponse());
        }
    }
}
