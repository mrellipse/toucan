using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
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
                options.CookieName = xsrfConfig.CookieName;
                options.HeaderName = xsrfConfig.HeaderName;
                options.RequireSsl = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ManagerUserAccounts, p => p.RequireRole(Toucan.Data.RoleTypes.Admin));
                options.AddPolicy(Policies.ManageSiteSettings, p => p.RequireRole(Toucan.Data.RoleTypes.Admin));
            });
        }

        public static void UseHtml5HistoryMode(this IApplicationBuilder app, string webRoot, string[] areas)
        {
            app.Use(async (context, next) =>
            {
                string path = context.Request.Path.ToString();

                if (!path.EndsWith(".ico"))
                {
                    string area = path.ToString().Split('/')[1];
                    string fileName = fileName = $"{webRoot}\\index.html";

                    if (!string.IsNullOrEmpty(area) && areas.Any(o => string.Equals(o, area, StringComparison.CurrentCultureIgnoreCase)))
                        fileName = $"{webRoot}\\{area.ToLower()}.html";

                    await context.Response.SendFileAsync(new FileInfo(fileName));
                }
                else
                {
                    await next.Invoke();
                }
            });
        }

        public static void UseAntiforgery(this IApplicationBuilder app, string cookieName)
        {
            app.Use(async (context, next) =>
            {
                string path = context.Request.Path.ToString();

                if (path.EndsWith(".html") || path.EndsWith("/"))
                {
                    IAntiforgery antiforgeryService = context.RequestServices.GetRequiredService<IAntiforgery>();

                    var tokenSet = antiforgeryService.GetAndStoreTokens(context);
                    if (tokenSet.RequestToken != null)
                    {
                        context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken, new CookieOptions() { HttpOnly = false });
                    }
                }

                await next.Invoke();
            });
        }
        private static async Task SendFileAsync(this HttpResponse response, FileInfo file)
        {
            HttpContext context = response.HttpContext;

            string physicalPath = file.FullName;
            var length = file.Length;
            var sendFile = context.Features.Get<IHttpSendFileFeature>();

            if (sendFile != null && !string.IsNullOrEmpty(physicalPath))
            {
                await sendFile.SendFileAsync(physicalPath, 0, length, CancellationToken.None);
                return;
            }

            try
            {
                using (var readStream = file.OpenRead())
                {
                    // Larger StreamCopyBufferSize is required because in case of FileStream readStream isn't going to be buffering
                    await StreamCopyOperation.CopyToAsync(readStream, response.Body, length, 64 * 1024, context.RequestAborted);
                }
            }
            catch (OperationCanceledException)
            {
                context.Abort();
            }
        }
    }
}
