using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System;
using System.Linq;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static IConfigurationBuilder AddToucan(this IConfigurationBuilder builder)
        {
            builder.AddEnvironmentVariables("ASPNETCORE_");

            var env = builder.Build().GetSection(WebHostDefaults.EnvironmentKey).Value;

            builder.AddJsonFile($"app.{env}.json", optional: false);

            return builder;
        }

        public static T GetTypedSection<T>(this IConfigurationRoot root, string sectionKey) where T : class, new() {

            T section = new T();

            var configValues = WebApp.Configuration.GetSection(sectionKey).GetChildren();

            if (configValues.Any())
            {
                Type t = section.GetType();
                PropertyInfo[] props = t.GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    var value = configValues.FirstOrDefault(o => string.Equals(o.Key, prop.Name, StringComparison.CurrentCultureIgnoreCase));
                    if (value.Value != null)
                        prop.SetValue(section, Convert.ChangeType(value.Value, prop.PropertyType));
                }
            }

            return section;
        }
    }
}
