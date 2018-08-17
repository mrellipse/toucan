using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Toucan.Contract;
using Toucan.Contract.Security;
using Toucan.Server.Model;
using Toucan.Server.Security;
using Toucan.Service.Security;

namespace Toucan.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ContentController : ControllerBase
    {
        public ContentController(IDomainContextResolver resolver, ILocalizationService localization) : base(resolver, localization)
        {
        }

        [HttpGet()]
        [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
        public async Task<object> RikerIpsum([FromQuery]DateTime @default, [FromQuery]DateTime locale, [FromQuery]DateTime iso, [FromQuery]DateTime utc)
        {
            Model.Payload<object> payload = null;
            IEqualityComparer<DateTime> comparer = new AccurateToSeconds();

            var times = new DateTime[]
            {
                TimeZoneInfo.ConvertTimeFromUtc(@default, this.DomainContext.SourceTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(locale, this.DomainContext.SourceTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(iso, this.DomainContext.SourceTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(utc, this.DomainContext.SourceTimeZone)
            };

            var delta = times.Distinct(comparer).ToList();
            string browserName = this.HttpContext.Request.GetFriendlyBrowserName();

            if (delta.Count > 1)
            {
                string expected = times.First().ToString("hh:mm tt");
                string browserCulture = this.HttpContext.Request.Headers.FirstOrDefault(o => o.Key == "Accept-Language").Value.FirstOrDefault();
                string plural = delta.Count == 2 ? "date" : "dates";
                string text = $"{browserName}. Browser sent {delta.Count - 1} {plural} in {browserCulture} format that could not be parsed by the server with '{this.DomainContext.User.CultureName}' culture!";

                payload = new Model.Payload<object>()
                {
                    Data = !this.DomainContext.User.Enabled ? this.Dictionary["home.body.0"].Value : this.Dictionary["home.body.1"].Value,
                    Message = new PayloadMessage()
                    {
                        MessageType = PayloadMessageType.Warning,
                        Text = text
                    }
                };
            }
            else
            {
                DateTime roundTripTime = TimeZoneInfo.ConvertTimeFromUtc(@default, this.DomainContext.SourceTimeZone);

                payload = new Model.Payload<object>()
                {
                    Data = !this.DomainContext.User.Enabled ? this.Dictionary["home.body.0"].Value : this.Dictionary["home.body.1"].Value,
                    Message = new PayloadMessage()
                    {
                        MessageType = PayloadMessageType.Info,
                        Text = browserName + ". " + string.Format(this.Dictionary["home.text"].Value, roundTripTime.ToString("hh:mm tt"))
                    }
                };
            }

            return await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1 * 1000);
                return payload;
            });
        }

        [HttpGet]
        [AuthorizeClaim(SecurityClaimTypes.Example, SecurityClaimValueTypes.Read)]
        public async Task<object> SecureUserContent()
        {
            var payload = new Model.Payload<string>()
            {
                Data = "=] a secret smile for you!",
                Message = new PayloadMessage()
                {
                    MessageType = PayloadMessageType.Success
                }
            };

            return await Task.FromResult(payload);
        }

        private class AccurateToSeconds : IEqualityComparer<DateTime>
        {
            public bool Equals(DateTime x, DateTime y)
            {
                return this.GetHashCode(x) == this.GetHashCode(y);
            }

            public int GetHashCode(DateTime obj)
            {
                var timeOfDay = new TimeSpan(obj.TimeOfDay.Hours, obj.TimeOfDay.Minutes, obj.TimeOfDay.Seconds);
                return obj.Date.ToString().GetHashCode() + timeOfDay.GetHashCode();
            }
        };
    }
}
