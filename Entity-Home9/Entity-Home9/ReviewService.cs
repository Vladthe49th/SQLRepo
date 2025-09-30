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
    public class ReviewService
    {
        private readonly string _connectionString = "Server=(localdb)\\\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;";

        public void AddReview(int productId, string customerName, string comment)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Reviews (ProductId, CustomerName, Comment) 
                       VALUES (@ProductId, @CustomerName, @Comment)";
            connection.Execute(sql, new { ProductId = productId, CustomerName = customerName, Comment = comment });
        }

        public IEnumerable<(string CustomerName, string Comment, DateTime CreatedAt)> GetReviews(int productId)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT CustomerName, Comment, CreatedAt 
                       FROM Reviews 
                       WHERE ProductId = @ProductId
                       ORDER BY CreatedAt DESC";
            return connection.Query<(string, string, DateTime)>(sql, new { ProductId = productId });
        }
    }

}
