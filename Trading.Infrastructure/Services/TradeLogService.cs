using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.Domain.Models;
using Trading.Domain.Services;

namespace Trading.Infrastructure.Services
{
    public class TradeLogService : ITradeLogService
    {
        private readonly List<TradeLog> _logs = new();
        private readonly int _maxLogs = 1000; // Keep last 1000 logs

        public Task<TradeLog> LogAsync(TradeLog log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            
            _logs.Add(log);
            
            // Keep rotation
            if (_logs.Count > _maxLogs)
            {
                _logs.RemoveRange(0, _logs.Count - _maxLogs);
            }
            
            return Task.FromResult(log);
        }

        public Task<IEnumerable<TradeLog>> GetLogsAsync(int limit = 100)
        {
            var logs = _logs
                .OrderByDescending(l => l.CreatedAt)
                .Take(limit)
                .ToList();
            
            return Task.FromResult<IEnumerable<TradeLog>>(logs);
        }

        public Task<IEnumerable<TradeLog>> GetLogsByTradeAsync(string tradeId)
        {
            var logs = _logs
                .Where(l => l.TradeId == tradeId)
                .OrderByDescending(l => l.CreatedAt)
                .ToList();
            
            return Task.FromResult<IEnumerable<TradeLog>>(logs);
        }

        public Task<IEnumerable<TradeLog>> GetLogsByOrderAsync(string orderId)
        {
            var logs = _logs
                .Where(l => l.OrderId == orderId)
                .OrderByDescending(l => l.CreatedAt)
                .ToList();
            
            return Task.FromResult<IEnumerable<TradeLog>>(logs);
        }

        public Task<IEnumerable<TradeLog>> GetLogsByStrategyAsync(string strategyName)
        {
            var logs = _logs
                .Where(l => l.StrategyName == strategyName)
                .OrderByDescending(l => l.CreatedAt)
                .ToList();
            
            return Task.FromResult<IEnumerable<TradeLog>>(logs);
        }

        public Task ClearOldLogsAsync(int daysToKeep = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var logsToRemove = _logs.Where(l => l.CreatedAt < cutoffDate).ToList();
            
            foreach (var log in logsToRemove)
            {
                _logs.Remove(log);
            }
            
            return Task.CompletedTask;
        }
    }
}
