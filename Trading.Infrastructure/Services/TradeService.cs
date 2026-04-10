using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.Domain.Models;
using Trading.Domain.Services;

namespace Trading.Infrastructure.Services
{
    public class TradeService : ITradeService
    {
        private readonly List<Trade> _trades = new();

        public Task<Trade> OpenTradeAsync(Trade trade)
        {
            if (trade == null) throw new ArgumentNullException(nameof(trade));
            
            _trades.Add(trade);
            return Task.FromResult(trade);
        }

        public Task<Trade> CloseTradeAsync(string tradeId, decimal exitPrice)
        {
            var trade = _trades.FirstOrDefault(t => t.Id == tradeId);
            if (trade == null) return Task.FromResult<Trade>(null);
            
            trade.Close(exitPrice);
            return Task.FromResult(trade);
        }

        public Task<Trade> GetTradeByIdAsync(string tradeId)
        {
            var trade = _trades.FirstOrDefault(t => t.Id == tradeId);
            return Task.FromResult(trade);
        }

        public Task<IEnumerable<Trade>> GetOpenTradesAsync()
        {
            var trades = _trades
                .Where(t => t.Status == TradeStatus.Open)
                .ToList();
            
            return Task.FromResult<IEnumerable<Trade>>(trades);
        }

        public Task<IEnumerable<Trade>> GetTradesByStrategyAsync(string strategyName)
        {
            var trades = _trades
                .Where(t => t.StrategyName == strategyName)
                .ToList();
            
            return Task.FromResult<IEnumerable<Trade>>(trades);
        }

        public Task<IEnumerable<Trade>> GetAllTradesAsync()
        {
            return Task.FromResult<IEnumerable<Trade>>(_trades.ToList());
        }

        public Task<decimal> GetTotalPnLAsync()
        {
            var totalPnL = _trades.Sum(t => t.GrossProfit);
            return Task.FromResult(totalPnL);
        }

        public Task<int> GetTotalTradesCountAsync()
        {
            return Task.FromResult(_trades.Count);
        }
    }
}
