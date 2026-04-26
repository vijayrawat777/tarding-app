using System.Diagnostics;
using Trading.Application.DTOs.OptionChain;

public interface IOptionStrategyEngine
{
    public List<TradeSignal> GenerateTrades(OptionChainResponse chain, decimal spot);
}

public class OptionStrategyEngine : IOptionStrategyEngine
{
    public List<TradeSignal> GenerateTrades(OptionChainResponse chain, decimal spot)
    {
        var signals = new List<TradeSignal>();

        if (chain?.Data == null || !chain.Data.Any())
            return signals;

        var ctx = AnalyzeMarket(chain, spot);

        // 🔥 Market filter (avoid bad conditions)
        if (ctx.VIX > 25) return signals; // too volatile
        if (ctx.VIX < 10 && ctx.Trend == "SIDEWAYS") return signals;

        // 🎯 Select ONLY near ATM strikes
        var nearStrikes = chain.Data
            .Where(x => Math.Abs((decimal)x.StrikePrice - spot) <= 150)
            .ToList();

        foreach (var x in nearStrikes)
        {
            decimal strike = (decimal)x.StrikePrice;

            // =========================
            // 🚀 CALL BUY LOGIC
            // =========================
            if (ctx.Trend == "BULLISH" && x.CallLTP > 0)
            {
                decimal confidence = 0;

                if (ctx.PCR > 1.1m) confidence += 0.25m;
                if (x.PutOpenInterest > x.CallOpenInterest) confidence += 0.20m;
                if (x.PutVolume > x.CallVolume) confidence += 0.15m;
                if (x.CallGreeks?.Delta > 0.5m) confidence += 0.15m;
                if (Math.Abs(strike - (decimal)ctx.ATM) <= 50) confidence += 0.10m;

                // Volume breakout confirmation
                if (x.CallVolume > x.CallOpenInterest * 0.1m)
                    confidence += 0.15m;

                if (confidence >= 0.6m)
                {
                    signals.Add(new TradeSignal
                    {
                        Signal = "BUY_CALL",
                        Strike = x.StrikePrice,
                        Symbol = x.CallSymbol,
                        Entry = x.CallLTP,
                        StopLoss = x.CallLTP * 0.70m,
                        Target = x.CallLTP * 1.6m,
                        Confidence = Math.Min(confidence, 1),
                        Reason = $"Bullish | PCR:{ctx.PCR:F2} | Support:{ctx.Support}"
                    });
                }
            }

            // =========================
            // 🔻 PUT BUY LOGIC
            // =========================
            if (ctx.Trend == "BEARISH" && x.PutLTP > 0)
            {
                decimal confidence = 0;

                if (ctx.PCR < 0.9m) confidence += 0.25m;
                if (x.CallOpenInterest > x.PutOpenInterest) confidence += 0.20m;
                if (x.CallVolume > x.PutVolume) confidence += 0.15m;
                if (x.PutGreeks?.Delta < -0.5m) confidence += 0.15m;
                if (Math.Abs(strike - (decimal)ctx.ATM) <= 50) confidence += 0.10m;

                if (x.PutVolume > x.PutOpenInterest * 0.1m)
                    confidence += 0.15m;

                if (confidence >= 0.6m)
                {
                    signals.Add(new TradeSignal
                    {
                        Signal = "BUY_PUT",
                        Strike = x.StrikePrice,
                        Symbol = x.PutSymbol,
                        Entry = x.PutLTP,
                        StopLoss = x.PutLTP * 0.70m,
                        Target = x.PutLTP * 1.6m,
                        Confidence = Math.Min(confidence, 1),
                        Reason = $"Bearish | PCR:{ctx.PCR:F2} | Resistance:{ctx.Resistance}"
                    });
                }
            }

            // =========================
            // 🧱 SIDEWAYS SELL CALL
            // =========================
            if (ctx.Trend == "SIDEWAYS" && strike > (decimal)ctx.ATM && x.CallLTP > 0)
            {
                decimal confidence = 0;

                if (ctx.PCR >= 0.9m && ctx.PCR <= 1.1m) confidence += 0.3m;
                if (x.CallOpenInterest > x.PutOpenInterest) confidence += 0.25m;
                if (x.CallVolume < x.PutVolume) confidence += 0.15m;

                if (confidence >= 0.6m)
                {
                    signals.Add(new TradeSignal
                    {
                        Signal = "SELL_CALL",
                        Strike = x.StrikePrice,
                        Symbol = x.CallSymbol,
                        Entry = x.CallLTP,
                        StopLoss = x.CallLTP * 1.30m,
                        Target = x.CallLTP * 0.50m,
                        Confidence = Math.Min(confidence, 1),
                        Reason = "Range-bound resistance"
                    });
                }
            }

            // =========================
            // 🧱 SIDEWAYS SELL PUT
            // =========================
            if (ctx.Trend == "SIDEWAYS" && strike < (decimal)ctx.ATM && x.PutLTP > 0)
            {
                decimal confidence = 0;

                if (ctx.PCR >= 0.9m && ctx.PCR <= 1.1m) confidence += 0.3m;
                if (x.PutOpenInterest > x.CallOpenInterest) confidence += 0.25m;
                if (x.PutVolume < x.CallVolume) confidence += 0.15m;

                if (confidence >= 0.6m)
                {
                    signals.Add(new TradeSignal
                    {
                        Signal = "SELL_PUT",
                        Strike = x.StrikePrice,
                        Symbol = x.PutSymbol,
                        Entry = x.PutLTP,
                        StopLoss = x.PutLTP * 1.30m,
                        Target = x.PutLTP * 0.50m,
                        Confidence = Math.Min(confidence, 1),
                        Reason = "Range-bound support"
                    });
                }
            }
        }

        // 🔥 FINAL FILTER (best trades only)
        return signals
            .Where(x => x.Confidence >= 0.6m)
            .OrderByDescending(x => x.Confidence)
            .ThenByDescending(x => x.Entry)
            .Take(2)
            .ToList();
    }
    public MarketContext AnalyzeMarket(OptionChainResponse chain, decimal spot)
    {
        var context = new MarketContext();

        context.Spot = spot;
        context.PCR = chain.TotalCallOI == 0 ? 0 :
                      (decimal)chain.TotalPutOI / chain.TotalCallOI;

        var atm = chain.Data.OrderBy(x => Math.Abs((decimal)x.StrikePrice - spot)).First();
        context.ATM = atm.StrikePrice;

        var support = chain.Data.OrderByDescending(x => x.PutOpenInterest).First();
        var resistance = chain.Data.OrderByDescending(x => x.CallOpenInterest).First();

        context.Support = support.StrikePrice;
        context.Resistance = resistance.StrikePrice;

        context.VIX = chain.VIXData?.Ltp ?? 15;

        // 🔥 Trend Logic (Improved)
        if (context.PCR > 1.15m)
            context.Trend = "BULLISH";
        else if (context.PCR < 0.85m)
            context.Trend = "BEARISH";
        else
            context.Trend = "SIDEWAYS";

        return context;
    }
}