using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Common.Extensions;
using Toucan.Contract;
using Toucan.Data;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                DbContextBase dbContext = serviceScope.ServiceProvider.GetService<DbContextBase>();
                ICryptoService crypto = app.ApplicationServices.GetRequiredService<ICryptoService>();
                int attempt = 0;
                int maxAttempts = 5;

                Action up = () =>
                {
                    attempt++;

                    Console.WriteLine($"Attempt #{attempt}/{maxAttempts} to apply ef migrations ");
                    
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeedData(crypto);
                };

                up.Retry(maxAttempts, 1000 * 3);
            }
        }
    }
}
