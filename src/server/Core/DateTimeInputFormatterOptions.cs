
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
using Toucan.Server.Formatters;

namespace Toucan.Server.Formatters
{
    public class DateTimeInputFormatterOptions : IConfigureOptions<MvcOptions>
    {
        private readonly ArrayPool<char> charPool;
        private readonly ILoggerFactory loggerFactory;
        private readonly ObjectPoolProvider objectPoolProvider;

        public DateTimeInputFormatterOptions(ILoggerFactory loggerFactory, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider)
        {
            this.charPool = charPool;
            this.loggerFactory = loggerFactory;
            this.objectPoolProvider = objectPoolProvider;
        }

        public void Configure(MvcOptions options)
        {
            options.InputFormatters.RemoveType<JsonInputFormatter>();

            DateTimeInputFormatterJson formatter = CreateCustomFormatter();

            options.InputFormatters.Add(formatter);
        }

        private DateTimeInputFormatterJson CreateCustomFormatter()
        {
            var inputSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();

            var jsonInputLogger = loggerFactory.CreateLogger<JsonInputFormatter>();

            return new DateTimeInputFormatterJson(jsonInputLogger, inputSettings, charPool, objectPoolProvider);
        }
    }
}