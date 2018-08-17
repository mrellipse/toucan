using System;
using System.Linq;
using Toucan.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        private static string MediaTypeJson = "application/json";
        public static bool AcceptsJsonResponse(this HttpRequest request)
        {
            var contentTypes = request.GetTypedHeaders().Accept;

            return contentTypes.Any(o => o.MediaType == MediaTypeJson || o.MediaType.Value.ToLower().Contains("json"));
        }

        public static string GetFriendlyBrowserName(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].ToString();

            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                if (userAgent.Contains("Edge/"))
                    return "Edge";

                if (userAgent.Contains("Trident/"))
                    return "IE";

                if (userAgent.Contains("Firefox/"))
                    return "Firefox";

                if (userAgent.Contains("OPR/"))
                    return "Opera";

                if (userAgent.Contains("Safari/"))
                    return "Safari";

                if (userAgent.Contains("Chrome/"))
                    return "Chromium";
            }

            return "Unknown";
        }

        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null, Stream inputStream = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (inputStream == null)
                inputStream = request.Body;

            using (StreamReader reader = new StreamReader(inputStream, encoding))
                return await reader.ReadToEndAsync();
        }

        public static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request, Stream inputStream = null)
        {
            if (inputStream == null)
                inputStream = request.Body;

            using (var ms = new MemoryStream(2048))
            {
                await inputStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}