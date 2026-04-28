using Trading.Domain.Models.Swing;

namespace Trading.StrategyEngine.Engine.Swing
{
    public class SwingStrategy
    {
        public SwingTradeSignal Generate(string symbol, List<Candle> candles)
        {
            if (candles.Count < 200) return null;

            var score = new SwingTradeSignal();
            var closes = candles.Select(x => x.Close).ToList();
            var volumes = candles.Select(x => x.Volume).ToList();

            decimal ema50 = EMA(closes, 50);
            decimal ema200 = EMA(closes, 200);
            decimal lastPrice = closes.Last();

            decimal rsi = RSI(closes, 14);
            decimal avgVolume = (decimal)volumes.TakeLast(20).Average();
            decimal lastVolume = volumes.Last();

            decimal recentHigh = candles.TakeLast(20).Max(x => x.High);

            // 🔥 Trend Check
            if (lastPrice > ema50 && ema50 > ema200)
            {
                score.Score += 25;
                score.Reasons.Add("Strong Uptrend");
            }

            // 🔥 RSI Momentum
            if (rsi > 55 && rsi < 70)
            {
                score.Score += 20;
                score.Reasons.Add("Healthy RSI");
            }

            // 🔥 Volume Breakout
            if (lastVolume > avgVolume * 1.5m)
            {
                score.Score += 20;
                score.Reasons.Add("Volume Spike");
            }

            // 🔥 Breakout
            if (lastPrice > recentHigh)
            {
                score.Score += 25;
                score.Reasons.Add("Breakout");
            }

            // 🔥 Pullback Entry Bonus
            if (lastPrice > ema50 && lastPrice < ema50 * 1.02m)
            {
                score.Score += 10;
                score.Reasons.Add("Pullback Entry");
            }

            // 🎯 Final Decision
            if (score.Score >= 60)
            {
                return new SwingTradeSignal
                {
                    Symbol = symbol,
                    SignalType = "BUY",
                    EntryPrice = lastPrice,
                    StopLoss = ema50, // dynamic SL
                    Target = lastPrice * 1.08m,
                    Confidence = score.Score / 100.0,
                    CreatedAt = DateTime.UtcNow
                };
            }

            return new SwingTradeSignal
            {
                Symbol = symbol,
                SignalType = "No Trade",
                EntryPrice = lastPrice,
                StopLoss = ema50, // dynamic SL
                Target = lastPrice * 1.08m,
                Confidence = score.Score / 100.0,
                CreatedAt = DateTime.UtcNow
            };

        }

        public decimal EMA(List<decimal> prices, int period)
        {
            decimal k = 2m / (period + 1);
            decimal ema = prices.Take(period).Average();

            foreach (var price in prices.Skip(period))
                ema = price * k + ema * (1 - k);

            return ema;
        }

        public decimal RSI(List<decimal> prices, int period)
        {
            decimal gain = 0, loss = 0;

            for (int i = prices.Count - period; i < prices.Count; i++)
            {
                var diff = prices[i] - prices[i - 1];
                if (diff > 0) gain += diff;
                else loss -= diff;
            }

            if (loss == 0) return 100;

            decimal rs = gain / loss;
            return 100 - (100 / (1 + rs));
        }
        public int CalculateQuantity(decimal capital, decimal riskPercent, decimal entry, decimal stopLoss)
        {
            decimal riskAmount = capital * riskPercent;
            decimal perShareRisk = Math.Abs(entry - stopLoss);

            return (int)(riskAmount / perShareRisk);
        }
    }
}
