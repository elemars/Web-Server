﻿// <auto-generated />
using System;
using Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(FenstermonitoringContext))]
    partial class FenstermonitoringContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");

            modelBuilder.Entity("Server.Models.Device", b =>
                {
                    b.Property<int>("Deviceid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("deviceid");

                    b.Property<decimal?>("Batterylevel")
                        .HasPrecision(3, 2)
                        .HasColumnType("decimal(3,2)")
                        .HasColumnName("batterylevel");

                    b.Property<int>("Laststate")
                        .HasColumnType("int")
                        .HasColumnName("laststate");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("location");

                    b.Property<string>("Macadress")
                        .IsRequired()
                        .HasMaxLength(17)
                        .HasColumnType("varchar(17)")
                        .HasColumnName("macadress");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)")
                        .HasColumnName("name");

                    b.HasKey("Deviceid")
                        .HasName("PRIMARY");

                    b.ToTable("devices", (string)null);
                });

            modelBuilder.Entity("Server.Models.State", b =>
                {
                    b.Property<int>("Stateid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("stateid");

                    b.Property<int>("Deviceid")
                        .HasColumnType("int")
                        .HasColumnName("deviceid");

                    b.Property<int?>("Statevalue")
                        .HasColumnType("int")
                        .HasColumnName("statevalue");

                    b.Property<DateTime?>("Timestamp")
                        .HasColumnType("datetime")
                        .HasColumnName("timestamp");

                    b.HasKey("Stateid")
                        .HasName("PRIMARY");

                    b.ToTable("states", (string)null);
                });

            //modelBuilder.Entity("Server.Models.User", b =>
            //{
            //    b.Property<string>("Username")
            //        .IsRequired()
            //        .HasMaxLength(16)
            //        .HasColumnType("varchar(16)")
            //        .HasColumnName("username");

            //    b.Property<string>("Role")
            //        .IsRequired()
            //        .HasMaxLength(5)
            //        .HasColumnType("varchar(5)")
            //        .HasColumnName("role");

            //    b.Property<string>("Password")
            //        .IsRequired()
            //        .HasMaxLength(32)
            //        .HasColumnType("varchar(32)")
            //        .HasColumnName("password");

            //    b.HasKey("Username")
            //        .HasName("PRIMARY");

            //    b.ToTable("users", (string)null);
            //});
#pragma warning restore 612, 618
        }
    }
}