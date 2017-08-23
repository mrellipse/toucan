using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static void ConfigureMvc(this IServiceCollection services, Config.AntiForgeryConfig xsrfConfig)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(Filters.GlobalExceptionFilter));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

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
    }
}
