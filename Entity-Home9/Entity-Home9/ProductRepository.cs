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
    public class ProductRepository
    {
        private readonly string _connectionString = "Server=(localdb)\\\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;";


        // Add product
        public void AddProduct(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Products (Name, Price, Stock, Description, CategoryId) 
                       VALUES (@Name, @Price, @Stock, @Description, @CategoryId)";
            connection.Execute(sql, product);
        }

        // Update product
        public void UpdateProduct(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Products SET Name = @Name, Price = @Price, Stock = @Stock,
                       Description = @Description, CategoryId = @CategoryId WHERE Id = @Id";
            connection.Execute(sql, product);
        }

        // Delete product
        public void DeleteProduct(int productId)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = "DELETE FROM Products WHERE Id = @Id";
            connection.Execute(sql, new { Id = productId });
        }

        public IEnumerable<Product> GetAllProducts()
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Products";
            return connection.Query<Product>(sql);
        }
    }
}
