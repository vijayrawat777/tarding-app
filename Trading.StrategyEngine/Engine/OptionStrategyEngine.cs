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
        var ctx = AnalyzeMarket(chain, spot);

        foreach (var x in chain.Data)
        {
            decimal strike = (decimal)x.StrikePrice;
            bool nearATM = Math.Abs(strike - (decimal)ctx.ATM) <= 100;

            if (!nearATM) continue;

            // =========================
            // 🚀 BUY CALL (SMART)
            // =========================
            if (ctx.Trend == "BULLISH"
                && x.CallDelta > 0.45m && x.CallDelta < 0.65m
                && x.CallVolume > ctx.AvgCallVolume
                && x.CallOpenInterest > ctx.AvgCallOI)
            {
                decimal confidence = CalculateConfidence(
                    ctx.PCR, x.CallDelta, x.CallIV, x.CallVolume, true);

                signals.Add(new TradeSignal
                {
                    Signal = "BUY_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Entry = x.CallLTP,
                    StopLoss = x.CallLTP * 0.7m,
                    Target = x.CallLTP * 1.6m,
                    Confidence = (double)confidence,
                    Reason = $"Bullish + Delta {x.CallDelta} + Volume Spike + OI Build"
                });
            }

            // =========================
            // 🔻 BUY PUT (SMART)
            // =========================
            if (ctx.Trend == "BEARISH"
                && x.PutDelta < -0.45m && x.PutDelta > -0.65m
                && x.PutVolume > ctx.AvgPutVolume
                && x.PutOpenInterest > ctx.AvgPutOI)
            {
                decimal confidence = CalculateConfidence(
                    ctx.PCR, x.PutDelta, x.PutIV, x.PutVolume, false);

                signals.Add(new TradeSignal
                {
                    Signal = "BUY_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Entry = x.PutLTP,
                    StopLoss = x.PutLTP * 0.7m,
                    Target = x.PutLTP * 1.6m,
                    Confidence = (double)confidence,
                    Reason = $"Bearish + Delta {x.PutDelta} + Volume Spike + OI Build"
                });
            }

            // =========================
            // 🧱 SELL CALL (SMART)
            // =========================
            if (ctx.Trend == "SIDEWAYS"
                && strike > (decimal)ctx.ATM
                && x.CallIV < ctx.AvgIV
                && x.CallDelta < 0.3m)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Entry = x.CallLTP,
                    StopLoss = x.CallLTP * 1.25m,
                    Target = x.CallLTP * 0.5m,
                    Confidence = (double)0.7m,
                    Reason = "Low IV + OTM Call + Range Market"
                });
            }

            // =========================
            // 🧱 SELL PUT (SMART)
            // =========================
            if (ctx.Trend == "SIDEWAYS"
                && strike < (decimal)ctx.ATM
                && x.PutIV < ctx.AvgIV
                && x.PutDelta > -0.3m)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Entry = x.PutLTP,
                    StopLoss = x.PutLTP * 1.25m,
                    Target = x.PutLTP * 0.5m,
                    Confidence = (double)0.7m,
                    Reason = "Low IV + OTM Put + Range Market"
                });
            }
        }

        return signals
            .Where(x => x.Confidence > (double)0.6m)
            .OrderByDescending(x => x.Confidence)
            .Take(3)
            .ToList();
    }
    public MarketContext AnalyzeMarket(OptionChainResponse chain, decimal spot)
    {
        var context = new MarketContext();

        context.Spot = spot;

        context.PCR = chain.TotalCallOI == 0 ? 0 :
            (decimal)chain.TotalPutOI / chain.TotalCallOI;

        var atm = chain.Data
            .OrderBy(x => Math.Abs((decimal)x.StrikePrice - spot))
            .First();

        context.ATM = atm.StrikePrice;

        // Strong Support / Resistance
        context.Support = chain.Data
            .OrderByDescending(x => x.PutOpenInterest)
            .First().StrikePrice;

        context.Resistance = chain.Data
            .OrderByDescending(x => x.CallOpenInterest)
            .First().StrikePrice;

        // Averages (IMPORTANT)
        context.AvgCallVolume = chain.Data.Average(x => x.CallVolume);
        context.AvgPutVolume = chain.Data.Average(x => x.PutVolume);

        context.AvgCallOI = chain.Data.Average(x => x.CallOpenInterest);
        context.AvgPutOI = chain.Data.Average(x => x.PutOpenInterest);

        context.AvgIV = chain.Data
            .Where(x => x.CallIV > 0)
            .Average(x => x.CallIV);

        // 🔥 Smart Trend Logic
        if (context.PCR > 1.2m && spot > (decimal)context.Support)
            context.Trend = "BULLISH";
        else if (context.PCR < 0.8m && spot < (decimal)context.Resistance)
            context.Trend = "BEARISH";
        else
            context.Trend = "SIDEWAYS";

        return context;
    }



    //    📊 Confidence Score Interpretation(IMPORTANT)
    //Confidence        Meaning                  Action
    //-------------------------------------------------------------
    //0.80 – 1.00	    🔥 Very Strong Setup     Take trade(priority)
    //0.70 – 0.79	    ✅ Good Setup            Trade allowed
    //0.60 – 0.69	    ⚠️ Medium Trade          only with confirmation
    //0.50 – 0.59	    ❌ Weak                  Avoid
    //< 0.50	        🚫 Bad Setup             Ignore completely
    private decimal CalculateConfidence(decimal pcr, decimal delta, decimal iv, long volume, bool isCall)
    {
        decimal score = 0;

        // PCR weight
        if (isCall && pcr > 1.1m) score += 0.25m;
        if (!isCall && pcr < 0.9m) score += 0.25m;

        // Delta weight
        if (Math.Abs(delta) > 0.5m) score += 0.25m;

        // IV weight (avoid low IV traps)
        if (iv > 15) score += 0.2m;

        // Volume weight
        if (volume > 1000000) score += 0.3m;

        return Math.Min(score, 1.0m);
    }
}