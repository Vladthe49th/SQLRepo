using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;

namespace BookStore.Data
{
    public class DbInit
    {
        public void Init(ApplicationContext context) 
        { 
        
          if (!context.Authors.Any())
            {
                context.Authors.AddRange(new Author[]
                    {
                    new Author {Name = "Jess Kidd"},
                    new Author {Name = "Hanzo Hasashi"},
                    new Author {Name = "Sex piston"},
                    new Author {Name = "Kuai Liang"},

                    });
                context.SaveChanges();
            }

            if (!context.Authors.Any())
            {
                context.Categories.AddRange
                    (
                    new Category
                    {
                        Name = "Artistic",
                        Description = "Fiction is compared to art forms that use " + "other material instead of verbal"
                    }
                    );
                context.SaveChanges();


            }

            if (!context.Authors.Any())
            {
                Book theNightShip = new Book
                {
                    Name = "The Night Ship",
                    Description = "Based on a real life story, historic epic from the award winning author of Man and Jar!",
                    Price = 70,
                    PublishedOn = new DateTime(2022, 9, 12),
                    Publisher = "Jess Kid",
                    Category = context.Categories.FirstOrDefault(e => e.Name.Equals("Artistic")),
                 Authors = new List<Author>()
                 {
                     context.Authors.FirstOrDefault(e =>  e.Name.Equals("Jess Kid"))
                 }


                };

                context.Books.AddRange(new Book[] { theNightShip });

                context.SaveChanges();


            }

            if (context.Authors.Any())
            {
                context.Reviews.AddRange
                    (
                    new Review
                    {
                        UserName = "Alex",
                        UserEmail = "alex@gmail.com",
                        Comment = "Good one!",
                        Stars = 5,
                        Book = context.Books.FirstOrDefault(e => e.Name.Equals("The Night Ship"))

                    }

                   );
                context.SaveChanges();
            }


            if(context.Authors.Any())
            {
                context.Promotions.AddRange(

                    new Promotion
                    {
                        Name = "Christmas Eve Discount!",
                        Percent = 15,
                        Book = context.Books.FirstOrDefault(e => e.Name.Equals("The Night Ship"))
                    }
                );
                context.SaveChanges();
            }



            if (!context.Orders.Any())
            {
                context.Orders.AddRange
                    (
                    new Order
                    {
                        CustomerName = "Alex",
                        City = "Kiev",
                        Address = "Shevchenko 17, kv 10",
                        Shipped = false,
                        Lines = new List<OrderLine>()
                        {
                         new OrderLine { Quantity = 1, Book= context.Books.FirstOrDefault(e => e.Title.Equals("The Night Ship"))},
                        }
                    },
                    new Order
                    {
                        CustomerName = "Marry",
                        City = "Dnepr",
                        Address = "Polya Avenu 121, kv 37",
                        Shipped = true,
                        Lines = new List<OrderLine>()
                        {
                         new OrderLine { Quantity = 2, Book= context.Books.FirstOrDefault(e => e.Title.Equals("The Night Ship"))},

                        }
                    }
                    );
                context.SaveChanges();
            }

        }
    }
}
