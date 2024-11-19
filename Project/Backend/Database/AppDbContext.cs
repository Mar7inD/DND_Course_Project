using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<WasteReport> WasteReports { get; set; }
        public DbSet<PersonBase> People { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Manager> Managers { get; set; }

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
                entity.HasKey(p => p.EmployeeId); // Primary Key on Root Type
                entity.Property(p => p.Name).IsRequired(); // Name is required
                entity.Property(p => p.Role).IsRequired(); // Role is required
                entity.Property(p => p.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for CreatedOn
                entity.Property(p => p.ModifiedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for ModifiedOn
            });

            // Table Per Hierarchy (TPH) Configuration for Inheritance
            modelBuilder.Entity<PersonBase>().ToTable("People");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Manager>().ToTable("Managers");

            // Unique constraints to prevent duplicates
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId).IsUnique();
            });
            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasIndex(m => m.EmployeeId).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
