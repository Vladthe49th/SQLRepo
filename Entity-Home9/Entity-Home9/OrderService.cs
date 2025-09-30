using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;

namespace Entity_Home9
{
    public class OrderService
    {
        private readonly string _connectionString = "Server=(localdb)\\\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;";

        public int CreateOrder(List<CartItem> cartItems)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                decimal total = cartItems.Sum(c => c.Product.Price * c.Quantity);

                int orderId = connection.ExecuteScalar<int>(
                    "INSERT INTO Orders (TotalAmount) OUTPUT INSERTED.Id VALUES (@TotalAmount)",
                    new { TotalAmount = total },
                    transaction
                );

                foreach (var item in cartItems)
                {
                    connection.Execute(
                        @"INSERT INTO OrderDetails (OrderId, ProductId, Quantity, Price)
                      VALUES (@OrderId, @ProductId, @Quantity, @Price)",
                        new
                        {
                            OrderId = orderId,
                            ProductId = item.Product.Id,
                            Quantity = item.Quantity,
                            Price = item.Product.Price
                        },
                        transaction
                    );

                    // Обновляем остаток на складе
                    connection.Execute(
                        "UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @ProductId",
                        new { Quantity = item.Quantity, ProductId = item.Product.Id },
                        transaction
                    );
                }

                transaction.Commit();
                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public decimal GetTotalRevenue()
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.ExecuteScalar<decimal>("SELECT SUM(TotalAmount) FROM Orders");
        }

        public IEnumerable<(string Name, int Quantity)> GetTopSellingProducts()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
            SELECT p.Name, SUM(od.Quantity) AS TotalSold
            FROM OrderDetails od
            JOIN Products p ON od.ProductId = p.Id
            GROUP BY p.Name
            ORDER BY TotalSold DESC";
            return connection.Query<(string, int)>(sql);
        }
    }

}
