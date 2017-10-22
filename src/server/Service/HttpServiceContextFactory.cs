
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Toucan.Contract;
using Toucan.Server.Model;

namespace Toucan.Server
{
    public class HttpServiceContextFactory
    {
        private readonly ILocalizationService localization;
        private readonly string defaultCultureName;
        private readonly string defaultTimeZoneId;

        public HttpServiceContextFactory(ILocalizationService localization, IOptions<Server.Config> config)
        {
            if (localization == null)
                throw new ArgumentNullException($"{nameof(localization)}");

            this.localization = localization;

            this.defaultCultureName = config.Value.DefaultCulture;
            this.defaultTimeZoneId = config.Value.DefaultTimeZone;
        }

        public string DefaultCultureName => defaultCultureName;

        public string DefaultTimeZoneId => defaultTimeZoneId;

        public HttpServiceContext Create(IUser user, CultureInfo culture = null, TimeZoneInfo sourceTimeZone = null)
        {
            if (user == null)
                throw new ArgumentNullException($"{nameof(user)}");

            culture = culture ?? ResolveCulture(user.CultureName);
            sourceTimeZone = sourceTimeZone ?? ResolveTimeZoneInfo(user.TimeZoneId);

            return new HttpServiceContext(user, culture, sourceTimeZone);
        }

        private CultureInfo ResolveCulture(string cultureName)
        {
            CultureInfo culture = null;

            if (this.localization.IsSupportedCulture(cultureName))
                culture = new CultureInfo(cultureName);

            if (culture == null)
            {
                if (cultureName.Contains("-"))
                {
                    var fallbackCultureName = cultureName.Split('-')[0];

                    if (this.localization.IsSupportedCulture(fallbackCultureName))
                        culture = new CultureInfo(fallbackCultureName);
                }
            }

            if (culture == null)
                culture = new CultureInfo(this.defaultCultureName);

            return culture;
        }

        private TimeZoneInfo ResolveTimeZoneInfo(string timeZoneId)
        {
            TimeZoneInfo timeZoneInfo = null;

            if (localization.IsSupportedTimeZone(timeZoneId))
                timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(o => o.Id == timeZoneId);

            if (timeZoneInfo == null)
                timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(o => o.Id == this.defaultTimeZoneId);

            return timeZoneInfo;
        }
    }
}