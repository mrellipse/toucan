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
using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static void UseAntiforgeryMiddleware(this IApplicationBuilder app, string clientName)
        {
            app.Use(async (context, next) =>
            {
                bool isAuthenticated = context.User.Identity.IsAuthenticated;

                if (isAuthenticated)
                {
                    string path = context.Request.Path.ToString();

                    IAntiforgery antiForgeryService = context.RequestServices.GetRequiredService<IAntiforgery>();

                    var tokenSet = antiForgeryService.GetAndStoreTokens(context);

                    if (tokenSet.RequestToken != null)
                    {
                        context.Response.Cookies.Append(clientName, tokenSet.RequestToken, new CookieOptions() { HttpOnly = false, Secure = true });
                    }
                }
                await next.Invoke();
            });
        }
    }
}
