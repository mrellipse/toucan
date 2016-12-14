using Microsoft.EntityFrameworkCore;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public class ToucanContext : DbContext
    {
        public ToucanContext()
        {
            
        }
        public ToucanContext(DbContextOptions<ToucanContext> options) : base(options)
        {

        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}