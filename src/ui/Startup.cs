using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StructureMap;

namespace Toucan.UI
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.Run(async (context) =>
            {
                if (context.Request.Path.Value.Contains("foo"))
                {
                    var foo = context.RequestServices.GetService<Toucan.Contract.IFoo>();
                    await context.Response.WriteAsync(foo.Action1());
                }
                else
                {
                    var cfg = context.RequestServices.GetService<IOptions<ToucanOptions>>();
                    await context.Response.WriteAsync(cfg.Value.Author);
                }
            });
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ToucanOptions>(WebApp.ConfigurationRoot);

            var container = new Container(c =>
            {
                var registry = new Registry();

                registry.IncludeRegistry<Toucan.Common.ContainerRegistry>();
                registry.IncludeRegistry<Toucan.Data.ContainerRegistry>();
                registry.IncludeRegistry<Toucan.Service.ContainerRegistry>();
                registry.IncludeRegistry<Toucan.UI.ContainerRegistry>();

                c.AddRegistry(registry);
                c.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        public void ConfigureProduction(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseExceptionHandler("/error.html");
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            this.ConfigureServices(services);
        }

        public void ConfigureStaging(IApplicationBuilder app)
        {
            this.Configure(app);
        }

        public void ConfigureStagingServices(IServiceCollection services)
        {
            this.ConfigureServices(services);
        }

    }
}