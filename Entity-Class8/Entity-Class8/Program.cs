using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Entity_Class8
{

    //Employee class
    public abstract class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
    }
    

    // Manager class
    public class Manager : Employee
    {
        public string Department { get; set; }
    }

    // Developer
    public class Developer : Employee
    {
        public string ProgrammingLanguage { get; set; }
    }

    // Salesperson

    public class SalesPerson : Employee
    {
        public decimal SalesTarget { get; set; }
    }



    // Context
    public class CompanyDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка TPH (Discriminator автоматически добавится)
            modelBuilder.Entity<Employee>()
                .HasDiscriminator<string>("EmployeeType")
                .HasValue<Manager>("Manager")
                .HasValue<Developer>("Developer")
                .HasValue<SalesPerson>("SalesPerson");
        }
    }


    
    public class Program
    {
        public static async Task Main()

        {
            
            using (var context = new CompanyDbContext())
            {
                //Test Data

                context.Database.EnsureCreated();

                var employees = new List<Employee>
    {
        new Manager { Name = "Vanya", Position = "Dept.Manager", Salary = 100000, Department = "IT" },
        new Developer { Name = "Nafanya", Position = "Senior Developer", Salary = 90000, ProgrammingLanguage = "C#" },
        new SalesPerson { Name = "Dolores Umbridge", Position = "Salesperson", Salary = 80000, SalesTarget = 500000 }
    };

                context.Employees.AddRange(employees);
                context.SaveChanges();



                // All employees
                var allEmployees = await context.Employees.ToListAsync();


                // List of managers and developers
                var managersAndDevelopers = await context.Employees
                .Where(e => e is Manager || e is Developer)
                 .ToListAsync();

                //Salesmen and their targets

                var salesPeople = await context.Employees
                .OfType<SalesPerson>()
                .Select(sp => new { sp.Name, sp.SalesTarget })
                .ToListAsync();


                // Use Migrations
                await context.Database.MigrateAsync();

                // Update employees

                var employee = await context.Employees.FindAsync(1);
                if (employee is Manager manager)
                {
                    manager.Department = "Finances";
                    manager.Salary = 110000;
                }
                else if (employee is Developer dev)
                {
                    dev.ProgrammingLanguage = "Python";
                }
                await context.SaveChangesAsync();
            }


         



        }
    }
}
