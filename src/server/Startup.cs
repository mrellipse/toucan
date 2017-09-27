using System;
using System.IO;
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
using System.Collections.Generic;

namespace Toucan.Server
{
    public partial class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var cfg = app.ApplicationServices.GetRequiredService<IOptions<AppConfig>>().Value;
            string webRoot = new DirectoryInfo(cfg.Server.Webroot).FullName;

            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(LogLevel.Debug);

                app.UseDeveloperExceptionPage();

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions()
                {
                    HotModuleReplacement = true,
                    HotModuleReplacementEndpoint = "/dist", // this value must be the same as 'output.publicPath' in webpack.config.js
                    ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\ui")
                });
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

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<DbContextBase>())
                {
                    ICryptoService crypto = app.ApplicationServices.GetRequiredService<ICryptoService>();
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeedData(crypto);
                }
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

            services.AddMemoryCache();
            services.ConfigureMvc(WebApp.Configuration.GetTypedSection<Config.AntiForgeryConfig>("server:antiForgery"));
            services.ConfigureAuthentication(tokenProvider, new string[] { "admin" });

            // services.AddDbContext<NpgSqlContext>(options =>
            // {   
            //     string assemblyName = typeof(Toucan.Data.Config).GetAssemblyName();
            //     options.UseNpgsql(dataConfig.ConnectionString, s => s.MigrationsAssembly(assemblyName));
            // });

            // services.AddDbContext<MsSqlContext>(options =>
            // {
            //     string assemblyName = typeof(Toucan.Data.Config).GetAssemblyName();
            //     options.UseSqlServer(dataConfig.ConnectionString, s => s.MigrationsAssembly(assemblyName));
            // });

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