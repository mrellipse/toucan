using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Toucan.Service;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static void UseTokenBasedAuthentication(this IApplicationBuilder app, TokenProviderConfig cfg, string[] areas)
        {
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(cfg.TokenSecurityKey));

            Func<JwtBearerChallengeContext, Task> onChallenge = (JwtBearerChallengeContext context) =>
            {
                if (context.Response.StatusCode == 302)
                {
                    Uri location = context.Response.GetTypedHeaders().Location;
                    string referrer = context.Request.Headers[HeaderNames.Referer];

                    if (location != null && !string.IsNullOrEmpty(referrer))
                    {
                        string locationUri = new UriBuilder(location.Scheme, location.Host, location.Port, "Login").ToString();
                        string returnUrl = CreateReturnUrl(referrer, areas);

                        context.Response.Headers.Remove(HeaderNames.Location);

                        locationUri = QueryHelpers.AddQueryString(locationUri, "returnUrl", returnUrl);

                        if (!string.IsNullOrEmpty(context.Error))
                            locationUri = QueryHelpers.AddQueryString(locationUri, "errorCode", context.Error);

                        if (!string.IsNullOrEmpty(context.ErrorDescription))
                            locationUri = QueryHelpers.AddQueryString(locationUri, "errorDesc", context.ErrorDescription);

                        context.Response.Headers.Append(HeaderNames.Location, locationUri);
                        context.Response.StatusCode = 200;
                    }
                }

                context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);

                return Task.Factory.StartNew(() => context.HandleResponse());
            };

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
                    OnChallenge = onChallenge
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

        private static string CreateReturnUrl(string referrer, string[] areas)
        {
            string areaPattern = string.Join(string.Empty, areas.Select(o => "/" + o));
            string pattern = $"({areaPattern})";
            Regex rex = new Regex(pattern);
            return rex.Replace(referrer, string.Empty, 1);
        }
    }
}
