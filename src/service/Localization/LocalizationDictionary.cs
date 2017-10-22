
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using Toucan.Contract;
using Toucan.Contract.Model;
using Toucan.Service.Model;

namespace Toucan.Service.Localization
{
    public class LocalizationDictionary : ReadOnlyDictionary<string, ILocalizedString>, ILocalizationDictionary
    {
        public static LocalizationDictionary Create(ILocalizationResolver resolver, string cultureName, string timeZoneId)
        {
            return Create(resolver, CultureInfo.GetCultureInfo(cultureName), TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }

        public static LocalizationDictionary Create(ILocalizationResolver resolver, CultureInfo culture, TimeZoneInfo timeZone)
        {
            string cultureName = culture?.Name;

            string data = resolver.ResolveCulture(cultureName).ToString();

            var strings = JsonHelper.DeserializeAndFlatten(data).Select(o => new LocalizedString(cultureName, o.Key, o.Value.ToString()));

            return new LocalizationDictionary(strings.ToDictionary(o => o.Name, o => o as ILocalizedString))
            {
                Culture = culture,
                TimeZone = timeZone
            };
        }
        
        internal LocalizationDictionary(IDictionary<string, ILocalizedString> dictionary) : base(dictionary)
        {
        }

        public CultureInfo Culture { get; private set; }

        public TimeZoneInfo TimeZone { get; private set; }

        IEnumerable<string> ILocalizationDictionary.Keys => base.Keys;

        IEnumerable<ILocalizedString> ILocalizationDictionary.Values => base.Values;
    }
}