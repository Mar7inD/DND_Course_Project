using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet for WasteReport
        public DbSet<WasteReport> WasteReports { get; set; }

        // DbSet for Person hierarchy
        public DbSet<PersonBase> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // WasteReport Configuration
            modelBuilder.Entity<WasteReport>(entity =>
            {
                entity.HasKey(w => w.Id); // Primary Key
                entity.Property(w => w.WasteType).IsRequired(); // WasteType is required
                entity.Property(w => w.WasteProcessingFacility).IsRequired(); // WasteProcessingFacility is required
                entity.Property(w => w.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for CreatedOn
                entity.Property(w => w.ModifiedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for ModifiedOn
            });

            // PersonBase Configuration
            modelBuilder.Entity<PersonBase>(entity =>
            {
                entity.HasKey(p => p.EmployeeId); // Primary Key
                entity.Property(p => p.Name).IsRequired(); // Name is required
                entity.Property(p => p.Role).IsRequired(); // Role is required
                entity.Property(p => p.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for CreatedOn
                entity.Property(p => p.ModifiedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for ModifiedOn
            });

            // Employee and Manager use Table-Per-Hierarchy (TPH) inheritance
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Manager>().ToTable("Managers");

            base.OnModelCreating(modelBuilder);
        }
    }
}
