using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public abstract class DbContextBase : DbContext
    {
        public virtual DbSet<Provider> Provider { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserProvider> UserProvider { get; set; }
        public virtual DbSet<UserProviderLocal> LocalProvider { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Verification> Verification { get; set; }

        public DbContextBase() : base()
        {

        }

        public DbContextBase(DbContextOptions options) : base(options)
        {

        }
    }
}