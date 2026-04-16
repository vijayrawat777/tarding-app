using Trading.Application.DTOs.OptionChain;

public interface IOptionStrategyEngine
{
    public List<TradeSignal> GenerateSignals(OptionChainResponse chain, decimal spotPrice);
}

public class OptionStrategyEngine : IOptionStrategyEngine
{
    public List<TradeSignal> GenerateSignals(OptionChainResponse chain, decimal spotPrice)
    {
        var signals = new List<TradeSignal>();

        decimal pcr = (decimal)chain.TotalPutOI / (decimal)chain.TotalCallOI;

        foreach (var x in chain.Data)
        {
            decimal distance = (decimal)x.StrikePrice - spotPrice;

            bool isATM = Math.Abs(distance) <= 50;
            bool isNearATM = Math.Abs(distance) <= 100;
            bool isOTMCall = (decimal)x.StrikePrice > spotPrice;
            bool isOTMPut = (decimal)x.StrikePrice < spotPrice;

            // =========================
            // 🔥 BUY CALL (Bullish)
            // =========================
            if (pcr > 1.1m && isNearATM && x.CallDelta >= 0.5m && x.CallTheta > -15)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "BUY_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Confidence = CalculateConfidence(pcr, x.CallDelta ?? 0, true)
                });
            }

            // =========================
            // 🔥 BUY PUT (Bearish)
            // =========================
            if (pcr < 0.9m && isNearATM && x.PutDelta <= -0.5m && x.PutTheta > -15)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "BUY_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Confidence = CalculateConfidence(pcr, Math.Abs(x.PutDelta ?? 0), true)
                });
            }

            // =========================
            // 🔥 SELL CALL (OTM)
            // =========================
            if (pcr < 1m && isOTMCall && x.CallDelta <= 0.3m && x.CallTheta < -10)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Confidence = CalculateConfidence(pcr, x.CallDelta ?? 0, false)
                });
            }

            // =========================
            // 🔥 SELL PUT (OTM)
            // =========================
            if (pcr > 1m && isOTMPut && x.PutDelta >= -0.3m && x.PutTheta < -10)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Confidence = CalculateConfidence(pcr, Math.Abs(x.PutDelta ?? 0), false)
                });
            }
        }

        // 🔥 Return only best 3 signals
        return signals
            .OrderByDescending(x => x.Confidence)
            //.Take(3)
            .ToList();
    }

    //    📊 How to Interpret Confidence
    //Confidence Meaning
    //0.8 – 1.0	🔥 Strong signal(good trade)
    //0.6 – 0.8	👍 Decent
    //0.4 – 0.6	⚠️ Weak
    //< 0.4	❌ Ignore
    private double CalculateConfidence(decimal pcr, decimal delta, bool isBuy)
    {
        decimal score = 0;

        // PCR strength
        if (pcr > 1.2m || pcr < 0.8m)
            score += 0.4m;
        else if (pcr > 1.1m || pcr < 0.9m)
            score += 0.3m;
        else
            score += 0.2m;

        // Delta strength
        if (delta > 0.6m)
            score += 0.4m;
        else if (delta > 0.4m)
            score += 0.3m;
        else
            score += 0.2m;

        // Buy vs Sell adjustment
        if (isBuy)
            score += 0.1m;

        return (double)Math.Min(score, 1.0m);
    }
}