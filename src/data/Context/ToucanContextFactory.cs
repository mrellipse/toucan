using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Toucan.Data
{
    public class ToucanContextFactory : IDbContextFactory<ToucanContext>
    {
        public ToucanContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ToucanContext>();

            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ToucanTest;Trusted_Connection=True;");

            return new ToucanContext(optionsBuilder.Options);
        }
    }
}