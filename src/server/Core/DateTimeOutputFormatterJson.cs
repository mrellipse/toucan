
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

            this.SerializerSettings.Culture = domainContext.Culture;
            this.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            this.SerializerSettings.Converters.Add(new DateTimeConverter(domainContext.SourceTimeZone));
            
            return base.WriteResponseBodyAsync(context, selectedEncoding);
        }
    }
}