using System;
using System.Linq;
using Toucan.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
    }
}