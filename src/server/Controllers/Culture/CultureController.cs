using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Service;
using Toucan.Server.Model;

namespace Toucan.Server.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    public class CultureController : Controller
    {
        private readonly ILocalizationService localization;

        public CultureController(ILocalizationService localization)
        {
            this.localization = localization;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<object> SupportedTimeZones()
        {
            return await localization.GetSupportedTimeZones();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<object> SupportedCultures()
        {
            return await localization.GetSupportedCultures();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<object> ResolveCulture(string id)
        {
            string timeZoneId = null;

            return new
            {
                CultureName = id,
                Resources = await localization.ResolveCulture(id),
                TimeZoneId = timeZoneId
            };
        }
    }
}