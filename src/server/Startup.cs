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
using Toucan.Contract;
using Toucan.Data;
using Toucan.Service;

namespace Toucan.Server
{
    public partial class Startup
    {

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Config cfg = app.ApplicationServices.GetRequiredService<IOptions<Config>>().Value;
            string webRoot = new DirectoryInfo(cfg.Webroot).FullName;

            loggerFactory.AddConsole(LogLevel.Debug);
            loggerFactory.AddDebug();

            StaticFileOptions staticFileOptions = new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(webRoot)
            };

            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseTokenBasedAuthentication(cfg.Service.TokenProvider);
            app.UseStaticFiles(staticFileOptions);
            app.UseMvc();
            app.UseHtml5HistoryMode(webRoot, cfg.Areas);
            
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<ToucanContext>())
                {
                    ICryptoService crypto = app.ApplicationServices.GetRequiredService<ICryptoService>();
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeedData(crypto);
                }
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string connectionString = WebApp.Configuration.GetSection(Toucan.Data.Config.DbConnectionKey).Value;

            services.AddOptions();
            services.Configure<Config>(WebApp.Configuration); // root web configuration
            var svcCFG = WebApp.Configuration.GetSection("service");
            services.Configure<Toucan.Service.Config>(WebApp.Configuration.GetSection("service")); // services configuration

            var tokenCfg = WebApp.Configuration.GetSection("service:tokenProvider");
            services.Configure<Toucan.Service.TokenProviderConfig>(WebApp.Configuration.GetSection("service:tokenProvider")); // token provider configuration
            services.Configure<Toucan.Data.Config>(WebApp.Configuration.GetSection("data")); // configuration
            services.ConfigureMvc();

            services.AddDbContext<ToucanContext>(options =>
            {
                options.UseSqlServer(connectionString, s => s.MigrationsAssembly("Toucan.Data"));
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