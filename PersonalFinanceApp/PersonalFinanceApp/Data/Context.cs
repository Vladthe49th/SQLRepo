using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using PersonalFinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceApp.Data
{
    public class FinanceDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserSettings> Settings => Set<UserSettings>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<TransactionCategory> Categories => Set<TransactionCategory>();

        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User - Settings
            modelBuilder.Entity<User>()
                .HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Transaction
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category - Transaction
            modelBuilder.Entity<TransactionCategory>()
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enum for keeping transactions as strings
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Type)
                .HasConversion<string>();
        }
    }
}
