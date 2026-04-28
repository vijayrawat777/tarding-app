using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Domain.Models.Swing;

namespace Trading.StrategyEngine.Engine.Swing
{
    public class BacktestEngine
    {
        private readonly SwingStrategy _strategy;

        public BacktestEngine()
        {
            _strategy = new SwingStrategy();
        }

        public BacktestResult Run(string symbol, List<Candle> candles)
        {
            var trades = new List<BacktestTrade>();

            decimal capital = 100000;
            decimal peak = capital;
            decimal maxDrawdown = 0;

            bool inTrade = false;

            decimal riskPercent = 0.02m;     // 2% risk per trade
            decimal brokerage = 20;          // per trade
            decimal slippagePercent = 0.001m; // 0.1%
            int maxHoldDays = 7;

            for (int i = 60; i < candles.Count - 1; i++)
            {
                if (inTrade) continue;

                var slice = candles.Take(i).ToList();
                var signal = _strategy.Generate(symbol, slice);

                if (signal == null) continue;

                decimal entryPrice = signal.EntryPrice;
                decimal stopLoss = signal.StopLoss;
                decimal target = signal.Target;

                decimal riskPerShare = entryPrice - stopLoss;
                if (riskPerShare <= 0) continue;

                // ✅ Position sizing (risk-based)
                decimal riskAmount = capital * riskPercent;
                int quantity = (int)(riskAmount / riskPerShare);

                if (quantity <= 0) continue;

                // Apply slippage on entry
                entryPrice += entryPrice * slippagePercent;

                inTrade = true;
                DateTime entryTime = candles[i].Time;

                for (int j = i + 1; j < candles.Count; j++)
                {
                    var c = candles[j];

                    decimal exitPrice = 0;
                    bool isWin = false;
                    bool exit = false;

                    // ✅ Stop Loss hit
                    if (c.Low <= stopLoss)
                    {
                        exitPrice = stopLoss - (stopLoss * slippagePercent);
                        isWin = false;
                        exit = true;
                    }
                    // ✅ Target hit
                    else if (c.High >= target)
                    {
                        exitPrice = target - (target * slippagePercent);
                        isWin = true;
                        exit = true;
                    }
                    // ✅ Time-based exit
                    else if ((j - i) >= maxHoldDays)
                    {
                        exitPrice = c.Close - (c.Close * slippagePercent);
                        isWin = exitPrice > entryPrice;
                        exit = true;
                    }

                    if (exit)
                    {
                        decimal pnlPerShare = exitPrice - entryPrice;
                        decimal pnl = pnlPerShare * quantity;

                        // subtract brokerage (entry + exit)
                        pnl -= (brokerage * 2);

                        capital += pnl;

                        trades.Add(new BacktestTrade
                        {
                            Symbol = symbol,
                            Entry = entryPrice,
                            Exit = exitPrice,
                            StopLoss = stopLoss,
                            Target = target,
                            EntryTime = entryTime,
                            ExitTime = c.Time,
                            IsWin = pnl > 0
                        });

                        // ✅ Drawdown tracking
                        if (capital > peak) peak = capital;
                        var dd = peak - capital;
                        if (dd > maxDrawdown) maxDrawdown = dd;

                        inTrade = false;
                        i = j; // skip to exit candle
                        break;
                    }
                }
            }

            return new BacktestResult
            {
                TotalTrades = trades.Count,
                Wins = trades.Count(x => x.IsWin),
                Losses = trades.Count(x => !x.IsWin),
                NetProfit = capital - 100000,
                MaxDrawdown = maxDrawdown
            };
        }

        private BacktestTrade CreateTrade(string symbol, decimal entry, decimal exit, bool win, DateTime entryTime, DateTime exitTime)
        {
            return new BacktestTrade
            {
                Symbol = symbol,
                Entry = entry,
                Exit = exit,
                IsWin = win,
                EntryTime = entryTime,
                ExitTime = exitTime
            };
        }
    }
}
