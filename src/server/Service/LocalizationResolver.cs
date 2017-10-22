using System;
using Microsoft.Extensions.Options;
using Toucan.Contract;
using Toucan.Server.Model;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toucan.Contract.Model;
using Toucan.Service.Model;
using System.Threading;

namespace Toucan.Server
{
    public class LocalizationResolver : ILocalizationResolver, ILocalizationCache
    {
        private readonly LocalizationOptions config;
        private readonly ConcurrentDictionary<string, string> cache;
        private List<string> cultures = null;
        private static object cacheLock = new Object();
        private static bool cacheLoading = false;

        public LocalizationResolver(IOptions<LocalizationOptions> options)
        {
            this.cache = new ConcurrentDictionary<string, string>();
            this.config = options.Value;
        }

        public void Clear()
        {
            this.cache.Clear();
            this.cultures.Clear();
        }

        public IEnumerable<IKeyValue> ResolveSupportedCultures()
        {
            while (cacheLoading)
            {
                Thread.Sleep(10);
            }

            if (this.cultures == null)
            {
                try
                {
                    cacheLoading = true;

                    lock (cacheLock)
                    {
                        string searchPattern = string.Format(this.config.Pattern, "*");

                        var cultures = from file in this.config.Directory.GetFiles(searchPattern)
                                       orderby file.Name
                                       select file.Name.Substring(0, file.Name.Length - file.Extension.Length);

                        this.cultures = cultures.ToList();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    cacheLoading = false;
                }
            }

            var result = from c in this.cultures
                         orderby c
                         select new KeyValue()
                         {
                             Key = c,
                             Value = c
                         };

            return result.Cast<IKeyValue>();
        }

        public object ResolveCulture(string cultureName)
        {
            if (!this.cache.ContainsKey(cultureName))
            {
                string searchPattern = string.Format(this.config.Pattern, cultureName);

                var cultureFile = this.config.Directory.GetFiles(searchPattern).FirstOrDefault();

                if (cultureFile != null)
                {
                    string value = File.ReadAllText(cultureFile.FullName);

                    if (!string.IsNullOrWhiteSpace(value))
                        this.cache.TryAdd(cultureName, value);
                }
            }

            string result = null;

            cache.TryGetValue(cultureName, out result);

            return result;
        }
    }
}