using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Toucan.Contract;
using Toucan.Server.Model;

namespace Toucan.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ContentController : Controller
    {
        private ILocalizationService localization;
        private readonly IDomainContextResolver resolver;

        public ContentController(IDomainContextResolver resolver, ILocalizationService localization)
        {
            this.localization = localization;
            this.resolver = resolver;
        }

        [HttpGet()]
        [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
        public async Task<object> RikerIpsum([FromQuery]DateTime clientTime)
        {
            IDomainContext context = this.resolver.Resolve();
            ILocalizationDictionary dict = this.localization.CreateDictionary(context);

            DateTime roundTripTime = TimeZoneInfo.ConvertTimeFromUtc(clientTime, context.SourceTimeZone);

            var payload = new Model.Payload<object>()
            {
                Data = !context.User.Enabled ? dict["home.body.0"].Value : dict["home.body.1"].Value,
                Message = new PayloadMessage()
                {
                    MessageType = PayloadMessageType.Info,
                    Text = string.Format(dict["home.text"].Value, roundTripTime.ToString("hh:mm tt"))
                }
            };

            return await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1 * 1000);
                return payload;
            });
        }
    }
}
