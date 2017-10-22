
using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Toucan.Contract;
using Toucan.Server.Core;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace Toucan.Server.Formatters
{
    public class DateTimeInputFormatterJson: JsonInputFormatter
    {
        public DateTimeInputFormatterJson(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, bool suppressInputFormatterBuffering) : base(logger, serializerSettings, charPool, objectPoolProvider, suppressInputFormatterBuffering)
        {
        }

        public DateTimeInputFormatterJson(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider) : base(logger, serializerSettings, charPool, objectPoolProvider)
        {

        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var resolver = context.HttpContext.RequestServices.GetService<IHttpServiceContextResolver>();

            IDomainContext domainContext = resolver.Resolve();

            this.SerializerSettings.Culture = domainContext.Culture;
            this.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            this.SerializerSettings.Converters.Add(new DateTimeConverter(domainContext.SourceTimeZone));

            return base.ReadRequestBodyAsync(context, encoding);
        }
    }
}