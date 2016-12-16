using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using StructureMap;
using Toucan.Data;

namespace Toucan.UI
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            string webRoot = WebApp.Configuration.GetSection(Toucan.UI.Config.WebrootKey).Value;
            StaticFileOptions staticFileOptions = new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(new DirectoryInfo(webRoot).FullName)
            };
            
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseStaticFiles(staticFileOptions);

            app.Run(async (context) =>
            {
                if (context.Request.Path.Value.Contains("foo"))
                {
                    var foo = context.RequestServices.GetService<Toucan.Contract.IFoo>();
                    await context.Response.WriteAsync(foo.Action1());
                }
                else
                {
                    if (context.Request.Path.Value.Contains("blog"))
                    {
                        var dbContext = context.RequestServices.GetService<ToucanContext>();
                        using (dbContext)
                        {
                            var blog = dbContext.Blogs.First();
                            await context.Response.WriteAsync($"#{blog.BlogId} by {blog.Name}");
                        }
                    }
                    else if (context.Request.Path.Value.Contains("posts"))
                    {
                        var dbContext = context.RequestServices.GetService<ToucanContext>();
                        using (dbContext)
                        {
                            var blog = dbContext.Blogs.Include(o => o.Posts).First();
                            var post = blog.Posts.First();
                            await context.Response.WriteAsync($"#{post.PostId} : \"{post.Title}\" by {post.Blog.Name}");
                        }
                    }
                    else
                    {
                        var cfg = context.RequestServices.GetService<IOptions<Config>>();
                        await context.Response.WriteAsync(cfg.Value.Webroot);
                    }
                }
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<ToucanContext>())
                {
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeedData();
                }
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string connectionString = WebApp.Configuration.GetSection(Toucan.Data.Config.DbConnectionKey).Value;

            services.AddOptions();
            services.Configure<Config>(WebApp.Configuration);

            services.AddDbContext<ToucanContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

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