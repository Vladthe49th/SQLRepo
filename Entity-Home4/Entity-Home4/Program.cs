using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

namespace Entity_Home4
{
    // Company class
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Store> Stores { get; set; } = new List<Store>();
    }

    // Store class

    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<CustomerStore> CustomerStores { get; set; } = new List<CustomerStore>();
    }

    // Customer class
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public ICollection<CustomerStore> CustomerStores { get; set; } = new List<CustomerStore>();
    }

    // Table for many-to-many connection 

    public class CustomerStore
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }
    }


    // Context
    public class AppDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerStore> CustomerStores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerStore>()
                .HasKey(cs => new { cs.CustomerId, cs.StoreId });

            modelBuilder.Entity<CustomerStore>()
                .HasOne(cs => cs.Customer)
                .WithMany(c => c.CustomerStores)
                .HasForeignKey(cs => cs.CustomerId);

            modelBuilder.Entity<CustomerStore>()
                .HasOne(cs => cs.Store)
                .WithMany(s => s.CustomerStores)
                .HasForeignKey(cs => cs.StoreId);
        }
    }




    internal class Program
    {
        static void Main(string[] args)
        {
            // 1) Companies with stores and customers
            using (var context = new AppDbContext())
            {
                var companies = context.Companies
                    .Include(c => c.Stores)
                        .ThenInclude(s => s.CustomerStores)
                            .ThenInclude(cs => cs.Customer)
                    .ToList();

                foreach (var company in companies)
                {
                    Console.WriteLine($"\tCompany: {company.Name}");
                    foreach (var store in company.Stores)
                    {
                        Console.WriteLine($"Store: {store.Name}");
                        foreach (var cs in store.CustomerStores)
                        {
                            Console.WriteLine($"\t\tCustomer: {cs.Customer.FullName}");
                        }
                    }
                }
            }


            // 2) Customer info

            using (var context = new AppDbContext())
            {
                var customers = context.Customers
                    .Include(c => c.CustomerStores)
                        .ThenInclude(cs => cs.Store)
                          .ThenInclude(s => s.Company)
                     .ToList();

                foreach (var customer in customers)
                {
                    Console.WriteLine($"Customer: {customer.FullName}");
                    foreach (var cs in customer.CustomerStores)
                    {
                        Console.WriteLine($"\t Store: {cs.Store.Name}, company: {cs.Store.Company.Name}");
                    }
                }

            }


            // 3) Companies with more than 5 stores

            using (var context = new AppDbContext())
            {
                var popularCompanies = context.Companies
                   .Where(c => c.Stores.Count > 5)
                    .Select(c => new
                    {
                         CompanyName = c.Name,
                        StoreCount = c.Stores.Count
                    })
                   .ToList();

                foreach (var company in popularCompanies)
                {
                    Console.WriteLine($"Company: {company.CompanyName}, has: {company.StoreCount} stores");
                }

            }


            // 4) Customers registered in more than one store

            using (var context = new AppDbContext())
            {
                var multiStoreCustomers = context.Customers
                   .Where(c => c.CustomerStores.Count > 1)
                   .Select(c => new
                 {
                    FullName = c.FullName,
                    StoreCount = c.CustomerStores.Count
                 })
               .ToList();

                foreach (var customer in multiStoreCustomers)
                {
                    Console.WriteLine($"Customer: {customer.FullName}, registered in {customer.StoreCount} stores");
                }

            }


        }
    }
}
