using System;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;

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
            string assemblyName = typeof(MsSqlContext).AssemblyQualifiedName.Split(',')[1].Trim();

            var optionsBuilder = new DbContextOptionsBuilder<MsSqlContext>();

            optionsBuilder.UseSqlServer(connectionString, o =>
            {
                o.MigrationsAssembly(assemblyName);
            });

            return new MsSqlContext(optionsBuilder.Options);
        }
    }
}