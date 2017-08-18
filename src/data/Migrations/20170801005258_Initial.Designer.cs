using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Toucan.Data;

namespace data.Migrations
{
    [DbContext(typeof(NpgSqlContext))]
    [Migration("20170801005258_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("Relational:Sequence:.user_seq", "'user_seq', '', '1', '1', '', '', 'Int64', 'False'");

            modelBuilder.Entity("Toucan.Data.Model.Provider", b =>
                {
                    b.Property<string>("ProviderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(512)");

                    b.Property<bool>("Enabled")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("true");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(128)");

                    b.HasKey("ProviderId");

                    b.ToTable("Provider");
                });

            modelBuilder.Entity("Toucan.Data.Model.Role", b =>
                {
                    b.Property<string>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(16)");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<bool>("Enabled")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("false");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.HasKey("RoleId");

                    b.HasIndex("CreatedBy");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Toucan.Data.Model.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "nextval('\"user_seq\"')");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("varchar(128)");

                    b.Property<bool>("Enabled")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("true");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(128)");

                    b.Property<bool>("Verified")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("false");

                    b.HasKey("UserId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Toucan.Data.Model.UserProvider", b =>
                {
                    b.Property<string>("ProviderId")
                        .HasColumnType("varchar(64)");

                    b.Property<long>("UserId");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(64)");

                    b.Property<string>("UserProviderType")
                        .IsRequired();

                    b.HasKey("ProviderId", "UserId")
                        .HasName("PK_UserProvider");

                    b.HasIndex("UserId")
                        .HasName("IX_UserProvider_UserId");

                    b.ToTable("UserProvider");

                    b.HasDiscriminator<string>("UserProviderType").HasValue("External");
                });

            modelBuilder.Entity("Toucan.Data.Model.UserRole", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(16)");

                    b.Property<long>("UserId");

                    b.HasKey("RoleId", "UserId")
                        .HasName("PK_UserRole");

                    b.HasIndex("UserId")
                        .HasName("IX_UserRole_UserId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Toucan.Data.Model.Verification", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("IssuedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<DateTime?>("RedeemedAt")
                        .HasColumnType("timestamp");

                    b.HasKey("UserId")
                        .HasName("PK_Verification");

                    b.ToTable("Verification");
                });

            modelBuilder.Entity("Toucan.Data.Model.UserProviderLocal", b =>
                {
                    b.HasBaseType("Toucan.Data.Model.UserProvider");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordSalt")
                        .HasColumnType("varchar(128)");

                    b.ToTable("UserProviderLocal");

                    b.HasDiscriminator().HasValue("Local");
                });

            modelBuilder.Entity("Toucan.Data.Model.Role", b =>
                {
                    b.HasOne("Toucan.Data.Model.User", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Toucan.Data.Model.UserProvider", b =>
                {
                    b.HasOne("Toucan.Data.Model.Provider", "Provider")
                        .WithMany("Users")
                        .HasForeignKey("ProviderId")
                        .HasConstraintName("FK_UserProvider_Provider");

                    b.HasOne("Toucan.Data.Model.User", "User")
                        .WithMany("Providers")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserProvider_User");
                });

            modelBuilder.Entity("Toucan.Data.Model.UserRole", b =>
                {
                    b.HasOne("Toucan.Data.Model.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRole_Role");

                    b.HasOne("Toucan.Data.Model.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRole_User");
                });

            modelBuilder.Entity("Toucan.Data.Model.Verification", b =>
                {
                    b.HasOne("Toucan.Data.Model.User", "User")
                        .WithMany("Verifications")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Verification_User")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
