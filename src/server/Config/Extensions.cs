using System;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toucan.Data;
using Toucan.Server.Core;
using Toucan.Server.Formatters;
using Toucan.Server.Model;
using Toucan.Service;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        private const string DefaultEnvironment = "production";
        public static IConfigurationBuilder AddToucan(this IConfigurationBuilder builder)
        {
            builder.AddEnvironmentVariables("ASPNETCORE_");

            var env = builder.Build().GetSection(WebHostDefaults.EnvironmentKey).Value;

            if (string.IsNullOrWhiteSpace(env))
            {
                env = DefaultEnvironment;
                Console.WriteLine($"WARN: Required runtime variable ASPNETCORE_ENVIRONMENT not found. Default set to '{env}'");
            }

            builder.AddJsonFile($"app.{env}.json", optional: false);

            return builder;
        }

        public static void ConfigureAuthentication(this IServiceCollection services, TokenProviderConfig cfg, string[] areas)
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = (context) =>
                    {
                        return OnChallenge(context, areas);
                    }
                };
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "access_token";
                options.TicketDataFormat = new Model.TokenDataFormat(cfg.TokenSecurityAlgorithm, CookieAuthenticationDefaults.AuthenticationScheme, tokenValidationParameters);
            });
        }

        public static void ConfigureMvc(this IServiceCollection services, Config.AntiForgeryConfig xsrfConfig)
        {
            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
                options.Filters.Add(typeof(Filters.GlobalExceptionFilter));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, DateTimeInputFormatterOptions>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, DateTimeOutputFormatterOptions>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<LocalizationOptions>, LocalizationResolverOptions>());

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = xsrfConfig.CookieName;
                options.HeaderName = xsrfConfig.HeaderName;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ManagerUserAccounts, p => p.RequireRole(Toucan.Data.RoleTypes.Admin));
                options.AddPolicy(Policies.ManageSiteSettings, p => p.RequireRole(Toucan.Data.RoleTypes.Admin));
            });
        }

        private static Task OnChallenge(JwtBearerChallengeContext context, string[] areas)
        {
            if (context.AuthenticateFailure != null)
            {
                string location = CreateReturnLocation(context, areas);

                context.Response.Headers.Append(HeaderNames.Location, location);
                context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);

                if (context.Request.AcceptsJsonResponse())
                {
                    return Task.Factory.StartNew(() =>
                    {
                        context.Response.StatusCode = 401;
                        context.HandleResponse();
                    });
                }
            }

            return Task.Factory.StartNew(() => context.HandleResponse());
        }

        private static string CreateReturnLocation(JwtBearerChallengeContext context, string[] areas)
        {
            string locationHeader = context.Request.Headers[HeaderNames.Location];

            Uri referrer = new Uri(context.Request.Headers[HeaderNames.Referer]);
            Uri location = new Uri(locationHeader ?? referrer.ToString());

            string returnUrl = CreateReturnUrl(referrer, areas);

            string locationUri = QueryHelpers.AddQueryString("Login", "returnUrl", returnUrl);

            if (!string.IsNullOrEmpty(context.Error))
                locationUri = QueryHelpers.AddQueryString(locationUri, "errorCode", context.Error);

            if (!string.IsNullOrEmpty(context.AuthenticateFailure.Message))
                locationUri = QueryHelpers.AddQueryString(locationUri, "errorDesc", context.ErrorDescription);

            return locationUri;
        }

        private static string CreateReturnUrl(Uri referrer, string[] areas)
        {
            string areaPattern = string.Join(string.Empty, areas.Select(o => "/" + o));

            Regex regex = new Regex($"({areaPattern})");

            return regex.Replace(referrer.ToString(), string.Empty, 1);
        }
    }
}
