using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Domain.Models;

namespace Trading.Domain.Services
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<Order>> GetOpenOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByStrategyAsync(string strategyName);
        Task<bool> CancelOrderAsync(string orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
    }
}
