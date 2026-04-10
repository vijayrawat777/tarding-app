using System;
using System.Collections.Generic;
using System.Linq;
using Trading.Domain.Models;

namespace Trading.Infrastructure.Services
{
    public interface IBacktestDataService
    {
        List<BacktestCandle> GetHistoricalData(string symbol, DateTime startDate, DateTime endDate);
        List<BacktestTrade> GetBacktestResults(string symbol, string strategy, DateTime startDate, DateTime endDate);
    }

    public class BacktestCandle
    {
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
    }

    public class BacktestTrade
    {
        public DateTime EntryTime { get; set; }
        public decimal EntryPrice { get; set; }
        public DateTime? ExitTime { get; set; }
        public decimal? ExitPrice { get; set; }
        public decimal PnL { get; set; }
        public decimal PnLPercent { get; set; }
        public string Strategy { get; set; } = "";
    }

    public class BacktestDataService : IBacktestDataService
    {
        public List<BacktestCandle> GetHistoricalData(string symbol, DateTime startDate, DateTime endDate)
        {
            var candles = new List<BacktestCandle>();
            var random = new Random(symbol.GetHashCode());
            var basePrice = GetBasePrice(symbol);
            var currentPrice = basePrice;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var dailyChangePercent = (decimal)(random.NextDouble() - 0.48) * 3m;
                var dailyChange = currentPrice * dailyChangePercent / 100m;
                var open = currentPrice;
                var close = currentPrice + dailyChange;
                var volatility = (decimal)(random.NextDouble() * 2.0);
                var high = Math.Max(open, close) + volatility;
                var low = Math.Min(open, close) - volatility;
                var volume = (long)(5000000 + random.Next(10000000));

                candles.Add(new BacktestCandle
                {
                    DateTime = date,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = volume
                });

                currentPrice = close;
            }

            return candles;
        }

        public List<BacktestTrade> GetBacktestResults(string symbol, string strategy, DateTime startDate, DateTime endDate)
        {
            var trades = new List<BacktestTrade>();
            var random = new Random((symbol + strategy).GetHashCode());
            var basePrice = GetBasePrice(symbol);
            var currentPrice = basePrice;
            var tradeCount = random.Next(5, 20);

            for (int i = 0; i < tradeCount; i++)
            {
                var entryTime = startDate.AddDays(random.Next((int)(endDate - startDate).TotalDays));
                var entryPrice = currentPrice + (decimal)(random.NextDouble() - 0.5) * basePrice * 2m / 100m;
                
                var pnlPercent = strategy switch
                {
                    "EMA" => (decimal)(random.NextDouble() - 0.45) * 8m,
                    "RSI" => (decimal)(random.NextDouble() - 0.40) * 6m,
                    "MACD" => (decimal)(random.NextDouble() - 0.48) * 5m,
                    _ => (decimal)(random.NextDouble() - 0.45) * 7m
                };

                var exitPrice = entryPrice * (1m + pnlPercent / 100m);
                var pnl = (exitPrice - entryPrice) * 100m;

                trades.Add(new BacktestTrade
                {
                    EntryTime = entryTime,
                    EntryPrice = entryPrice,
                    ExitTime = entryTime.AddDays(random.Next(1, 20)),
                    ExitPrice = exitPrice,
                    PnL = pnl,
                    PnLPercent = pnlPercent,
                    Strategy = strategy
                });

                currentPrice = exitPrice;
            }

            return trades.OrderBy(t => t.EntryTime).ToList();
        }

        private decimal GetBasePrice(string symbol) => symbol switch
        {
            "RELIANCE" => 2850m,
            "INFY" => 2350m,
            "TCS" => 3950m,
            "HDFCBANK" => 1850m,
            "HDFC" => 2650m,
            "LT" => 3120m,
            "BAJAJFINSV" => 14200m,
            "ITC" => 420m,
            "MARUTI" => 9850m,
            "SBIN" => 730m,
            "WIPRO" => 480m,
            "TECHM" => 1425m,
            "ASIANPAINT" => 3280m,
            "SUNPHARMA" => 1680m,
            "AXISBANK" => 1125m,
            "ICICIBANK" => 1050m,
            "INDIGO" => 3150m,
            "KOTAKBANK" => 1875m,
            "MINDTREE" => 3580m,
            "DATAENTRY" => 2450m,
            _ => 2500m
        };
    }
}

