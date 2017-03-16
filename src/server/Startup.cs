using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using StructureMap;
using Toucan.Common.Extensions;
using Toucan.Contract;
using Toucan.Data;

namespace Toucan.Server
{
    public partial class Startup
    {

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var cfg = app.ApplicationServices.GetRequiredService<IOptions<AppConfig>>().Value;
            string webRoot = new DirectoryInfo(cfg.Server.Webroot).FullName;

            loggerFactory.AddConsole(LogLevel.Debug);
            loggerFactory.AddDebug();

            StaticFileOptions staticFileOptions = new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(webRoot)
            };

            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseTokenBasedAuthentication(cfg.Service.TokenProvider);
            app.UseAntiforgery(cfg.Server.AntiForgery.ClientName);
            app.UseStaticFiles(staticFileOptions);
            app.UseMvc();
            app.UseHtml5HistoryMode(webRoot, cfg.Server.Areas);

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

            services.AddOptions();
            services.Configure<AppConfig>(WebApp.Configuration); // root web configuration
            services.Configure<Toucan.Service.Config>(WebApp.Configuration.GetSection("service")); // services configuration
            services.Configure<Toucan.Service.TokenProviderConfig>(WebApp.Configuration.GetSection("service:tokenProvider")); // token provider configuration
            services.Configure<Toucan.Data.Config>(WebApp.Configuration.GetSection("data")); // configuration
            services.Configure<Toucan.Server.Config>(WebApp.Configuration.GetSection("server"));

            services.ConfigureMvc(WebApp.Configuration.GetTypedSection<Config.AntiForgeryConfig>("server:antiForgery"));
            services.AddMemoryCache();

            // services.AddDbContext<NpgSqlContext>(options =>
            // {    
            //     string assemblyName = typeof(Toucan.Data.Config).GetAssemblyName();
            //     options.UseNpgsql(dataConfig.ConnectionString, s => s.MigrationsAssembly(assemblyName));
            // });

            services.AddDbContext<MsSqlContext>(options =>
            {
                string assemblyName = typeof(Toucan.Data.Config).GetAssemblyName();
                options.UseSqlServer(dataConfig.ConnectionString, s => s.MigrationsAssembly(assemblyName));
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

        public void ConfigureProduction(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();
            app.UseExceptionHandler("/error.html");
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            this.ConfigureServices(services);
        }

        public void ConfigureStaging(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this.Configure(app, env, loggerFactory);
        }

        public void ConfigureStagingServices(IServiceCollection services)
        {
            this.ConfigureServices(services);
        }
    }
}