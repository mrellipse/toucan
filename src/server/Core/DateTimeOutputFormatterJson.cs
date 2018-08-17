using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Toucan.Contract;
using Toucan.Server.Core;

namespace Toucan.Server.Formatters
{
    public class DateTimeOutputFormatterJson : JsonOutputFormatter
    {
        public DateTimeOutputFormatterJson(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool) : base(serializerSettings, charPool)
        {
        }
        
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var resolver = context.HttpContext.RequestServices.GetService<IHttpServiceContextResolver>();

            IDomainContext domainContext = resolver.Resolve();

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() },
                Converters = new List<JsonConverter>() { new DateTimeConverter(domainContext.SourceTimeZone) },
                Culture = domainContext.Culture,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            try
            {
                context.HttpContext.RequestAborted.ThrowIfCancellationRequested();
                return context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(context.Object, settings), selectedEncoding, context.HttpContext.RequestAborted);
            }
            catch (System.OperationCanceledException)
            {
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}