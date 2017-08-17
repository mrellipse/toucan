using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
        public static void ConfigureDataProtection(this IServiceCollection services, Toucan.Server.Config config)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/var/lib/toucan/web/keys"))
                .SetApplicationName(config.ApplicationName);
        }
    }
}
