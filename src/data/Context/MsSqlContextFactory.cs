using System;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using Toucan.Common.Extensions;

namespace Toucan.Data
{
    public class MsSqlContextFactory : IDbContextFactory<MsSqlContext>
    {
        public MsSqlContext Create(DbContextFactoryOptions options)
        {
            DirectoryInfo info = new DirectoryInfo(options.ApplicationBasePath);
            DirectoryInfo dataProjectRoot = info.Parent.Parent.Parent.Parent;
            string basePath = Path.Combine(dataProjectRoot.FullName, "data");

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("mssql.json")
                .Build();

            string connectionString = config.GetSection(Toucan.Data.Config.DbConnectionKey).Value;

            var optionsBuilder = new DbContextOptionsBuilder<MsSqlContext>();

            optionsBuilder.UseSqlServer(connectionString, o =>
            {
                string assemblyName = typeof(MsSqlContext).GetAssemblyName();
                o.MigrationsAssembly(assemblyName);
            });

            return new MsSqlContext(optionsBuilder.Options);
        }
    }
}