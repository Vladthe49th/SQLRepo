using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entity_Home8
{
    class Program
    {
        static void Main()
        {

            // Calling for procedures

            using (var db = new ApplicationContext())
            {
                // 1. GetUsersWithCompanies
                var usersWithCompanies = db.Users
                    .FromSqlRaw("EXEC GetUsersWithCompanies")
                    .ToList();

                foreach (var user in usersWithCompanies)
                {
                    Console.WriteLine($"{user.Email} works in {user.Company?.Name}");
                }

                // 2. GetUsersByFirstName
                var nameParam = new SqlParameter("@Name", "Tom");
                var tomUsers = db.Users
                    .FromSqlRaw("EXEC GetUsersByFirstName @Name", nameParam)
                    .ToList();

                Console.WriteLine("Users with the name Tom:");
                foreach (var user in tomUsers)
                {
                    Console.WriteLine(user.Email);
                }

                // 3. GetAverageAge
                var avgAgeParam = new SqlParameter
                {
                    ParameterName = "@AvgAge",
                    SqlDbType = SqlDbType.Float,
                    Direction = ParameterDirection.Output
                };

                db.Database.ExecuteSqlRaw("EXEC GetAverageAge @AvgAge OUTPUT", avgAgeParam);
                Console.WriteLine($"Average user age: {avgAgeParam.Value}");
            }



            using (ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                List<Country> countries = new List<Country>
            {
                new Country { Name = "United States" },
                new Country { Name = "Canada" },
                new Country { Name = "United Kingdom" },
                new Country { Name = "Australia" }
            };

                List<Company> companies = new List<Company>
            {
                new Company { Name = "Acme Corporation", Address = "123 Main St, Cityville, USA", RegistrationDate = new DateTime(1999, 10, 12), Country = countries[0] },
                new Company { Name = "Tech Innovators Ltd.", Address = "456 Tech Blvd, Technocity, Canada", RegistrationDate = new DateTime(2005, 07, 19), Country = countries[1] },
                new Company { Name = "Global Logistics Solutions", Address = "789 Logistics Ave, Transportville, USA", RegistrationDate = new DateTime(1987, 05, 24), Country = countries[0] },
                new Company { Name = "Green Energy Solutions Inc.", Address = "101 Eco Street, Greenburg, United Kingdom", RegistrationDate = new DateTime(2003, 10, 14), Country = countries[2] },
                new Company { Name = "Financial Wizards LLC", Address = "202 Money Lane, Cashville, Australia", RegistrationDate = new DateTime(1994, 08, 22), Country = countries[3] }
            };

                List<Role> roles = new List<Role>()
            {
                new Role { Name = "Administrator" },
                new Role { Name = "Manager" },
                new Role { Name = "Editor" },
                new Role { Name = "Guest" }
            };

                List<User> users = new List<User>
            {
                new User { Email = "john.doe@example.com", Age = 21, Salary = 3500, Company = companies[0], Roles = new List<Role>{ roles[0] } },
                new User { Email = "jane.smith@gmail.com", Age = 30, Salary = 2600, Company = companies[2], Roles = new List<Role>{ roles[1] } },
                new User { Email = "bob.jones@yahoo.com", Age = 40, Salary = 8000, Company = companies[3], Roles = new List<Role>{ roles[2], roles[3] } },
                new User { Email = "alice.williams@hotmail.com", Age = 26, Salary = 1800, Company = companies[3], Roles = new List<Role>{ roles[1], roles[2] } },
                new User { Email = "sarahcharlie@example.org", Age = 34, Salary = 2000, Company = companies[0], Roles = new List<Role>{ roles[3] } },
                new User { Email = "davis.green@gmail.com", Age = 16, Salary = 10_000, Company = companies[1], Roles = new List<Role>{ roles[3] } },
                new User { Email = "david.miller@yahoo.com", Age = 22, Salary = 6300, Company = companies[3], Roles = new List<Role>{ roles[3] } },
                new User { Email = "sophia.wilson@hotmail.com", Age = 27, Salary = 3800, Company = companies[2], Roles = new List<Role>{ roles[1], roles[2] } },
                new User { Email = "ethan1taylor@example.net", Age = 53, Salary = 2300, Company = companies[1], Roles = new List<Role>{ roles[0], roles[1], roles[2] } },
                new User { Email = "olivia.martin@gmail.com", Age = 45, Salary = 2900, Company = companies[2], Roles = new List<Role>{ roles[1] } },
                new User { Email = "tomberel23@yahoo.com", Age = 62, Salary = 4950, Company = companies[2], Roles = new List<Role>{ roles[2], roles[3] } },
                new User { Email = "kate.king@hotmail.com", Age = 39, Salary = 1900, Company = companies[0], Roles = new List<Role>{ roles[1], roles[2] } },
                new User { Email = "tomsmith@example.org", Age = 44, Salary = 2450, Company = companies[0], Roles = new List<Role>{ roles[3] } },
                new User { Email = "alan12@gmail.com", Age = 19, Salary = 7400, Company = companies[4], Roles = new List<Role>{ roles[0], roles[1], roles[2], roles[3] } }
             };

                List<UserSettings> userSettings = new List<UserSettings>()
            {
                new UserSettings { FirstName = "John", LastName = "Doe", PhoneNumber = "+44-204-577-0077", Passport = "AB569784", User = users[0] },
                new UserSettings { FirstName = "Jane", LastName = "Smith", PhoneNumber = "+1-800-925-6278", Passport = null, User = users[1] },
                new UserSettings { FirstName = "Bob", LastName = "Johnson", PhoneNumber = "+1-800-689-1234", Passport = "CD569784", User = users[2] },
                new UserSettings { FirstName = "Charlie", LastName = "Brown", PhoneNumber = "+1-800-976-8901", Passport = "GH569784", User = users[3] },
                new UserSettings { FirstName = "Emily", LastName = "Davis", PhoneNumber = "+1-800-632-4567", Passport = "IJ569784", User = users[4] },
                new UserSettings { FirstName = "David", LastName = "Miller", PhoneNumber = "+1-800-519-2345", Passport = "KL569784", User = users[5] },
                new UserSettings { FirstName = "Sophia", LastName = "Wilson", PhoneNumber = "+1-800-768-9012", Passport = "MN569784", User = users[6] },
                new UserSettings { FirstName = "Ethan", LastName = "Taylor", PhoneNumber = "+44-204-577-0077", Passport = null, User = users[7] },
                new UserSettings { FirstName = "Olivia", LastName = "Martin", PhoneNumber = null, Passport = "UV569784", User = users[8] },
                new UserSettings { FirstName = "Tom", LastName = "Berel", PhoneNumber = "+1-800-465-1234", Passport = "ST569784", User = users[9] },
                new UserSettings { FirstName = "Kate", LastName = "Filips", PhoneNumber = "+1-800-543-6789", Passport = "WX569784", User = users[10] },
                new UserSettings { FirstName = "Tom", LastName = "Smith", PhoneNumber = "+44-587-365-4488", Passport = "AA569784", User = users[11] },
                new UserSettings { FirstName = "Alan", LastName = "Dilan", PhoneNumber = null, Passport = "YZ569784", User = users[12] }
            };

                db.Countries.AddRange(countries);
                db.Companies.AddRange(companies);
                db.Roles.AddRange(roles);
                db.Users.AddRange(users);
                db.UserSettings.AddRange(userSettings);
                db.SaveChanges();
            }
        }
    }

    public class Country
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<Company> Companies { get; set; }
    }

    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }

        public int CountryId { get; set; }
        public Country? Country { get; set; }
        public List<User> Users { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
        public decimal Salary { get; set; }

        public UserSettings UserSettings { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public List<Role> Roles { get; set; }
    }

    public class UserSettings
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Passport { get; set; }
        public bool IsPermanent { get; set; } = false;

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserSettings> UserSettings { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=testdb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(e => e.Roles).WithMany(e => e.Users).UsingEntity(e => e.ToTable("UsersRoles"));
        }
    }



}
