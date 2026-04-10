using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Domain.Models;

namespace Trading.Domain.Services
{
    public interface ITradeLogService
    {
        Task<TradeLog> LogAsync(TradeLog log);
        Task<IEnumerable<TradeLog>> GetLogsAsync(int limit = 100);
        Task<IEnumerable<TradeLog>> GetLogsByTradeAsync(string tradeId);
        Task<IEnumerable<TradeLog>> GetLogsByOrderAsync(string orderId);
        Task<IEnumerable<TradeLog>> GetLogsByStrategyAsync(string strategyName);
        Task ClearOldLogsAsync(int daysToKeep = 30);
    }
}
