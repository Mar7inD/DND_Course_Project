﻿// <auto-generated />
using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241119190914_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Shared.Models.PersonBase", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("EmployeeId");

                    b.ToTable("People", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Shared.Models.WasteReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double?>("Co2Emission")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<double>("WasteAmount")
                        .HasColumnType("REAL");

                    b.Property<int?>("WasteCollectorId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("WasteDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("WasteProcessingFacility")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WasteType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("WasteReports");
                });

            modelBuilder.Entity("Shared.Models.Employee", b =>
                {
                    b.HasBaseType("Shared.Models.PersonBase");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("Employees", (string)null);
                });

            modelBuilder.Entity("Shared.Models.Manager", b =>
                {
                    b.HasBaseType("Shared.Models.PersonBase");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("Managers", (string)null);
                });

            modelBuilder.Entity("Shared.Models.Employee", b =>
                {
                    b.HasOne("Shared.Models.PersonBase", null)
                        .WithOne()
                        .HasForeignKey("Shared.Models.Employee", "EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Shared.Models.Manager", b =>
                {
                    b.HasOne("Shared.Models.PersonBase", null)
                        .WithOne()
                        .HasForeignKey("Shared.Models.Manager", "EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}