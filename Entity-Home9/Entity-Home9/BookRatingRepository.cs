using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;

namespace Entity_Home9
{
    public class BookRatingRepository
    {

        // Add a rating
        public void AddRating(int customerId, int bookId, int score)
        {
            using var connection = new SqlConnection("\"Server=(localdb)\\\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;");
            var sql = "INSERT INTO Ratings (CustomerId, BookId, Score) VALUES (@CustomerId, @BookId, @Score)";
            connection.Execute(sql, new { CustomerId = customerId, BookId = bookId, Score = score });
        }


        // Average rating of a book
        public double GetAverageRating(int bookId)
        {
            using var connection = new SqlConnection("Server=(localdb)\\\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;");
            var sql = "SELECT AVG(CAST(Score AS FLOAT)) FROM Ratings WHERE BookId = @BookId";
            return connection.ExecuteScalar<double>(sql, new { BookId = bookId });
        }


        // Top 5 books by rating

        public IEnumerable<(int BookId, string Title, double AvgRating)> GetTop5RatedBooks()
        {
            using var connection = new SqlConnection("Server=(localdb)\\\\\\\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;");
            var sql = @"
        SELECT TOP 5 b.Id, b.Title, AVG(CAST(r.Score AS FLOAT)) AS AvgRating
        FROM Ratings r
        JOIN Books b ON r.BookId = b.Id
        GROUP BY b.Id, b.Title
        ORDER BY AvgRating DESC";

            return connection.Query<(int, string, double)>(sql);
        }


    }
}
