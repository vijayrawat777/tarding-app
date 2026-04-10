using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.Domain.Models;
using Trading.Domain.Services;

namespace Trading.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly List<Order> _orders = new();

        public Task<Order> PlaceOrderAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            
            order.Status = OrderStatus.Accepted;
            _orders.Add(order);
            
            return Task.FromResult(order);
        }

        public Task<Order> GetOrderByIdAsync(string orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            return Task.FromResult(order);
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync()
        {
            var openOrders = _orders
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Accepted)
                .ToList();
            
            return Task.FromResult<IEnumerable<Order>>(openOrders);
        }

        public Task<IEnumerable<Order>> GetOrdersByStrategyAsync(string strategyName)
        {
            var orders = _orders
                .Where(o => o.StrategyName == strategyName)
                .ToList();
            
            return Task.FromResult<IEnumerable<Order>>(orders);
        }

        public Task<bool> CancelOrderAsync(string orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return Task.FromResult(false);
            
            if (order.Status == OrderStatus.Executed || order.Status == OrderStatus.Cancelled)
                return Task.FromResult(false);
            
            order.Cancel();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return Task.FromResult<IEnumerable<Order>>(_orders.ToList());
        }

        public void ExecuteOrder(string orderId, decimal executedPrice, decimal executedQty)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null && order.Status != OrderStatus.Executed)
            {
                order.Execute(executedPrice, executedQty);
            }
        }
    }
}
