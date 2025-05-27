using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


// MAIN TASK //
namespace Entity_Home3
{

    // Product class 

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Description { get; set; }

        // Ignored field
        public string? TemporaryData { get; set; }
    }


    public class ShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ShopDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                // 1) Primary key
                entity.HasKey(p => p.Id);

                // 2) Name length
                entity.Property(p => p.Name)
                    .HasMaxLength(100)
                    // 3) Required field
                    .IsRequired();

                // 4) Price type
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(10,2)");

                // 5)  StockQuantity default value
                entity.Property(p => p.StockQuantity)
                    .HasDefaultValue(0);

                // 6) Description not required
                entity.Property(p => p.Description)
                    .IsRequired(false);

                // 7) Unique name index
                entity.HasIndex(p => p.Name)
                    .IsUnique();

                // 8) Ignored field
                entity.Ignore(p => p.TemporaryData);

                // 9) Table name
                entity.ToTable("StoreProducts");

                // 10)  Price limit >= 0
                entity.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0");
            });
        }
    }



}


// SUB-TASK SOFTWARE DEV MANAGER ///

// Enum for task status

public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed,
    Blocked
}



// Task class

public class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime Deadline { get; set; }

    public TaskStatus Status { get; set; }
}


// Task configuration
public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        // Table name
        builder.ToTable("Tasks");

        // Text length
        builder.Property(t => t.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        // Enum as a string
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();

        // Index for creation data
        builder.HasIndex(t => t.CreatedAt);

        // Unique task names
        builder.HasIndex(t => t.Title)
            .IsUnique();

        // Checking Deadline >= CreatedAt
        builder.HasCheckConstraint("CK_Deadline", "[Deadline] >= [CreatedAt]");

        // Starting data
        builder.HasData(
            new Task
            {
                Id = 1,
                Title = "Creation of architecture",
                Description = "Build new arcitechture for our app",
                CreatedAt = DateTime.UtcNow,
                Deadline = DateTime.UtcNow.AddDays(7),
                Status = TaskStatus.NotStarted
            },
            new Task
            {
                Id = 2,
                Title = "Model realization",
                Description = "Creation of model classes and migrations",
                CreatedAt = DateTime.UtcNow,
                Deadline = DateTime.UtcNow.AddDays(10),
                Status = TaskStatus.InProgress
            }
        );
    }
}

// Appdbcontext

public class AppDbContext : DbContext
{
    public DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TaskManagerDb;Trusted_Connection=True;");
    }
}

