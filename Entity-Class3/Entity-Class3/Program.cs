



using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;



public enum Rank
{
    Constable,
    Sergeant,
    Inspector,
    Chief
}

public class Officer
{
    [Key]
    [Column("OfficerId")] // Another name for our table key
    public int Id { get; set; }

    [Required] 
    [MaxLength(50)] 
    public string FullName { get; set; }

    public int PrecinctNumber { get; set; } 

    // Age check 
    [Required]
    [Range(21, 65)] 
    public int Age { get; set; }

    [Required]
    public Rank Rank { get; set; } // Check through enum

    public string? PhoneNumber { get; set; } 

}


public class PoliceContext : DbContext
{
    public DbSet<Officer> Officers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PoliceDB;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Officer>()
            .HasKey(o => new { o.Id, o.PrecinctNumber }); // Composite key

        modelBuilder.Entity<Officer>()
            .Property(o => o.FullName)
            .HasMaxLength(100);

        modelBuilder.Entity<Officer>()
            .HasCheckConstraint("CK_Officer_Age", "[Age] >= 21 AND [Age] <= 65"); // Age check

        modelBuilder.Entity<Officer>()
            .HasCheckConstraint("CK_Officer_Rank", "[Rank] IN (0, 1, 2, 3)"); // Enum check for rank
    }
}