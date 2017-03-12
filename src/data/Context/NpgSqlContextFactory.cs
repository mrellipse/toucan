using System;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using Toucan.Common.Extensions;

namespace Toucan.Data
{
    public class NpgSqlContextFactory : IDbContextFactory<NpgSqlContext>
    {
        public NpgSqlContext Create(DbContextFactoryOptions options)
        {
            DirectoryInfo info = new DirectoryInfo(options.ApplicationBasePath);
            DirectoryInfo dataProjectRoot = info.Parent.Parent.Parent.Parent;
            string basePath = Path.Combine(dataProjectRoot.FullName, "data");

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("npgsql.json")
                .Build();

            string connectionString = config.GetSection(Toucan.Data.Config.DbConnectionKey).Value;

            var optionsBuilder = new DbContextOptionsBuilder<NpgSqlContext>();

            optionsBuilder.UseNpgsql(connectionString, o =>
            {
                string assemblyName = typeof(NpgSqlContext).GetAssemblyName();
                o.MigrationsAssembly(assemblyName);
            });

            return new NpgSqlContext(optionsBuilder.Options);
        }
    }
}