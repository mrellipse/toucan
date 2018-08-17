using System;
using System.Buffers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;
using Toucan.Contract;
using Toucan.Server.Core;

namespace Toucan.Server.Formatters
{
    public class DateTimeInputFormatterJson : JsonInputFormatter
    {
        public DateTimeInputFormatterJson(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, bool suppressInputFormatterBuffering) : base(logger, serializerSettings, charPool, objectPoolProvider, suppressInputFormatterBuffering)
        {

        }

        public DateTimeInputFormatterJson(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider) : base(logger, serializerSettings, charPool, objectPoolProvider)
        {

        }

        public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
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

            var value = await context.HttpContext.Request.GetRawBodyStringAsync(encoding);

            var @object = JsonConvert.DeserializeObject(value, context.Metadata.ModelType, settings);

            return InputFormatterResult.Success(@object);
        }
    }
}