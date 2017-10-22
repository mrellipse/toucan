using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toucan.Contract;
using Toucan.Contract.Model;
using Toucan.Data;
using Toucan.Data.Model;
using Toucan.Service.Model;

namespace Toucan.Service.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILocalizationResolver resolver;

        public LocalizationService(ILocalizationResolver resolver)
        {
            this.resolver = resolver;
        }

        public ILocalizationResolver Resolver => this.resolver;

        public ILocalizationDictionary CreateDictionary(IDomainContext context)
        {
            return LocalizationDictionary.Create(this.resolver, context.Culture, context.SourceTimeZone);
        }

        public ILocalizationDictionary CreateDictionary(string cultureName, string timeZoneId)
        {
            return LocalizationDictionary.Create(this.resolver, cultureName, timeZoneId);
        }

        public Task<IEnumerable<IKeyValue>> GetSupportedCultures()
        {
            return Task.FromResult(this.resolver.ResolveSupportedCultures());
        }

        public Task<IEnumerable<IKeyValue>> GetSupportedTimeZones()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(o => o.DisplayName)
                .Select(o => new KeyValue() { Key = o.Id, Value = $"{o.DisplayName}" })
                .Cast<IKeyValue>();

            return Task.FromResult(timeZones);
        }

        public bool IsSupportedCulture(string cultureName)
        {
            return this.resolver.ResolveSupportedCultures().Any(o => o.Key == cultureName);
        }

        public bool IsSupportedTimeZone(string timeZoneId)
        {
            return TimeZoneInfo.GetSystemTimeZones().Any(o => o.Id == timeZoneId);
        }

        public async Task<object> ResolveCulture(string cultureName)
        {
            var data = this.Resolver.ResolveCulture(cultureName);

            return await Task.FromResult(data);
        }
    }
}