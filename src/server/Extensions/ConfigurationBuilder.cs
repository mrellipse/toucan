using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static IConfigurationBuilder AddToucan(this IConfigurationBuilder builder)
        {
            builder.AddEnvironmentVariables("ASPNETCORE_")
                .AddJsonFile("hosting.json");

            var env = builder.Build().GetSection(WebHostDefaults.EnvironmentKey).Value;

            builder.AddJsonFile($"app.{env}.json", optional: false);

            return builder;
        }
    }
}
