using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Domain.Models;

namespace Trading.Domain.Services
{
    public interface ITradeService
    {
        Task<Trade> OpenTradeAsync(Trade trade);
        Task<Trade> CloseTradeAsync(string tradeId, decimal exitPrice);
        Task<Trade> GetTradeByIdAsync(string tradeId);
        Task<IEnumerable<Trade>> GetOpenTradesAsync();
        Task<IEnumerable<Trade>> GetTradesByStrategyAsync(string strategyName);
        Task<IEnumerable<Trade>> GetAllTradesAsync();
        Task<decimal> GetTotalPnLAsync();
        Task<int> GetTotalTradesCountAsync();
    }
}
