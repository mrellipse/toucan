using System;
using System.Linq;
using Toucan.Data.Model;
using Toucan.Contract;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Toucan.Data
{
    public static partial class Extensions
    {
        // Creates a new sequence (with optional schema name, and returns the qualified pgsql object name)
        public static string CreateNpgSequence(this ModelBuilder modelBuilder, string name, string schemaName = null, int startsAt = 1, int incrementsBy = 1)
        {
            string value = "";

            if (string.IsNullOrWhiteSpace(schemaName))
                value = $"\"{name}\"";
            else
                value = $"\"{schemaName}\".\"{name}\"";

            if (schemaName == null || string.IsNullOrWhiteSpace(schemaName))
                modelBuilder.HasSequence<long>(name).StartsAt(startsAt).IncrementsBy(incrementsBy);
            else
                modelBuilder.HasSequence<long>(name, schemaName).StartsAt(startsAt).IncrementsBy(incrementsBy);

            return $"nextval('{value}')";
        }

        /*
        Helper method to configure audit columns for an entity
        */
        public static void AddAuditColumns<T>(this EntityTypeBuilder<T> entity) where T : class, IAuditable
        {
            entity.Property(e => e.CreatedOn)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.CreatedBy);

            entity.Property(e => e.LastUpdatedOn)
                .IsRequired(false)
                .HasColumnType("DATETIME2(7)")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.LastUpdatedBy).IsRequired(false);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(o => o.CreatedBy);

            entity.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(o => o.LastUpdatedBy);
        }

    }
}