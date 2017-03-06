using System;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;

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
            string assemblyName = typeof(NpgSqlContext).AssemblyQualifiedName.Split(',')[1].Trim();

            var optionsBuilder = new DbContextOptionsBuilder<NpgSqlContext>();

            optionsBuilder.UseSqlServer(connectionString, o =>
            {
                o.MigrationsAssembly(assemblyName);
            });

            return new NpgSqlContext(optionsBuilder.Options);
        }
    }
}