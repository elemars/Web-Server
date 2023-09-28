using System;
using System.Collections.Generic;
using FenstermonitoringAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FenstermonitoringAPI.Database;

public partial class FenstermonitoringContext : DbContext
{
    public FenstermonitoringContext()
    {
    }

    public FenstermonitoringContext(DbContextOptions<FenstermonitoringContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<State> States { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("Name=ConnectionStrings:DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Deviceid).HasName("PRIMARY");

            entity.ToTable("devices");

            entity.Property(e => e.Deviceid)
                .ValueGeneratedNever()
                .HasColumnName("deviceid");
            entity.Property(e => e.Batterylevel)
                .HasPrecision(3, 2)
                .HasColumnName("batterylevel");
            entity.Property(e => e.Location)
                .HasMaxLength(30)
                .HasColumnName("location");
            entity.Property(e => e.Macadress)
                .HasMaxLength(17)
                .HasColumnName("macadress");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Stateid).HasName("PRIMARY");

            entity.ToTable("states");

            entity.Property(e => e.Stateid)
                .ValueGeneratedNever()
                .HasColumnName("stateid");
            entity.Property(e => e.Deviceid).HasColumnName("deviceid");
            entity.Property(e => e.Statevalue).HasColumnName("statevalue");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
