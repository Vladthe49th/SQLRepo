using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;

namespace BookStore.Interfaces
{
    public interface IOrder
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetAllOrdersByNameAsync(string Name);
        Task<Order> GetOrderAsync(int id);
        Task<Order> GetOrderByOrderLinesAndBooksAsync(int id);

        Task AddOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
    }
}
