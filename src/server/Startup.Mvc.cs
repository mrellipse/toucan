using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Server
{
    public static partial class Extensions
    {

        public static void ConfigureMvc(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(Filters.GlobalExceptionFilter));
            });
        }

        public static void UseHtml5HistoryMode(this IApplicationBuilder app, string webRoot, string[] areas)
        {
            app.Use(async (context, next) =>
            {
                string area = context.Request.Path.ToString().Split('/')[1];
                string fileName = fileName = $"{webRoot}\\index.html";

                if (areas.Any(o => string.Equals(o, area, StringComparison.CurrentCultureIgnoreCase)))
                    fileName = $"{webRoot}\\{area.ToLower()}.html";
                
                await context.Response.SendFileAsync(new FileInfo(fileName));
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
