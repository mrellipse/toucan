using System;
using Microsoft.AspNetCore.Http;
using Toucan.Contract;
using Toucan.Data.Model;
using Toucan.Service;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Toucan.Server
{
    public class HttpServiceContextResolver : IHttpServiceContextResolver
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly Config config;
        private readonly CultureService culture;
        private readonly HttpServiceContextFactory factory;
        private User cacheUser;

        public HttpServiceContextResolver(IOptions<Server.Config> config, IHttpContextAccessor contextAccessor, CultureService culture, HttpServiceContextFactory factory)
        {
            this.config = config.Value;
            this.contextAccessor = contextAccessor;
            this.culture = culture;
            this.factory = factory;
        }

        public IUser Resolve(HttpContext context, bool cache = true)
        {
            User user = cacheUser;

            if (user == null)
            {
                user = context.User.FromClaimsPrincipal();

                if (user == null)
                {
                    Console.WriteLine($"Resolved user from anonymous data");

                    user = new User()
                    {
                        CultureName = this.culture.GetFromRequest(context, CultureService.CultureName),
                        DisplayName = null,
                        Enabled = context.User.Identity.IsAuthenticated,
                        Username = context.User.Identity.Name,
                        UserId = 0,
                        TimeZoneId = this.culture.GetFromRequest(context, CultureService.TimeZoneId),
                        Verified = false
                    };
                }
                else
                {
                    // the culture and timezone from request may be more up to date than that in the claims principal
                    user.CultureName = this.culture.GetFromRequest(context, CultureService.CultureName);
                    user.TimeZoneId = this.culture.GetFromRequest(context, CultureService.TimeZoneId);

                    Console.WriteLine($"Resolved user #{user.UserId} from claims principal");
                }
                
                if (string.IsNullOrEmpty(user.CultureName))
                    user.CultureName = this.config.DefaultCulture;

                if (string.IsNullOrEmpty(user.TimeZoneId))
                    user.TimeZoneId = this.config.DefaultTimeZone;

                if (cache)
                    cacheUser = user;
            }

            return user;
        }

        public IDomainContext Resolve(bool cache = true)
        {
            IUser user = this.Resolve(this.contextAccessor.HttpContext, cache);

            return this.factory.Create(user);
        }
    }
}