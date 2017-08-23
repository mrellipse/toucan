using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using StructureMap;
using Toucan.Common.Extensions;
using Toucan.Contract;
using Toucan.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Toucan.Server
{
    public partial class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var cfg = app.ApplicationServices.GetRequiredService<IOptions<AppConfig>>().Value;
            string webRoot = WebApp.ResolvePath(cfg.Server.Webroot);

            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(LogLevel.Debug);

                app.UseDeveloperExceptionPage();

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions()
                {
                    HotModuleReplacement = true,
                    ProjectPath = WebApp.ResolvePath(@"..\ui"),
                    EnvironmentVariables = new Dictionary<string, string>(){
                        { "api", WebApp.GetUrlsFromEnv().FirstOrDefault() }
                    } // TBC: needs actioning bitches
                });
            }
            else
            {
                loggerFactory.AddConsole(LogLevel.Warning);
            }

            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseAntiforgeryMiddleware(cfg.Server.AntiForgery.ClientName);

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(webRoot)
            });

            app.UseMvc();
            app.UseHistoryModeMiddleware(webRoot, cfg.Server.Areas);

            bool migrate = false;
            bool.TryParse(Environment.GetEnvironmentVariable("TOUCAN_DBMIGRATE"), out migrate);

            Console.WriteLine($"Migrations Enabled : { migrate.ToString()}");

            if (migrate)
            {
                app.ApplyMigrations();
                Console.WriteLine($"Success: Executed migration code against database ");
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var dataConfig = WebApp.Configuration.GetTypedSection<Toucan.Data.Config>("data");
            var serverConfig = WebApp.Configuration.GetTypedSection<Toucan.Server.Config>("server");
            var tokenProvider = WebApp.Configuration.GetTypedSection<Toucan.Service.TokenProviderConfig>("service:tokenProvider");

            services.AddOptions();
            services.Configure<AppConfig>(WebApp.Configuration); // root web configuration
            services.Configure<Toucan.Service.Config>(WebApp.Configuration.GetSection("service")); // services configuration
            services.Configure<Toucan.Service.TokenProviderConfig>(WebApp.Configuration.GetSection("service:tokenProvider")); // token provider configuration
            services.Configure<Toucan.Data.Config>(WebApp.Configuration.GetSection("data")); // configuration
            services.Configure<Toucan.Server.Config>(WebApp.Configuration.GetSection("server"));

            services.ConfigureMvc(WebApp.Configuration.GetTypedSection<Config.AntiForgeryConfig>("server:antiForgery"));
            services.AddMemoryCache();
            services.ConfigureDataProtection(serverConfig);

            Func<Toucan.Data.Config, string> resolveConnection = (config) =>
            {
                string value = config.ConnectionString;

                if (!string.IsNullOrWhiteSpace(config.HostKey) && !string.IsNullOrWhiteSpace(config.HostKey))
                {
                    string host = Environment.GetEnvironmentVariable(config.HostKey);

                    if (!string.IsNullOrWhiteSpace(host))
                        value = new Regex(config.HostPattern).Replace(value, host);
                }

                return value;
            };

            // services.AddDbContext<MsSqlContext>(options =>
            // {
            //     string assemblyName = typeof(Toucan.Data.Config).GetAssemblyName();
            //     string connectionString = resolveConnection(dataConfig);
            //     options.UseSqlServer(connectionString, s => s.MigrationsAssembly(assemblyName));
            // });

            services.AddDbContext<NpgSqlContext>(options =>
            {
                string assemblyName = typeof(Toucan.Data.Config).GetAssemblyName();
                string connectionString = resolveConnection(dataConfig);
                options.UseNpgsql(connectionString, s => s.MigrationsAssembly(assemblyName));
            });

            var container = new Container(c =>
            {
                var registry = new Registry();

                registry.IncludeRegistry<Toucan.Common.ContainerRegistry>();
                registry.IncludeRegistry<Toucan.Data.ContainerRegistry>();
                registry.IncludeRegistry<Toucan.Service.ContainerRegistry>();
                registry.IncludeRegistry<Toucan.Server.ContainerRegistry>();

                c.AddRegistry(registry);
                c.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }
    }
}