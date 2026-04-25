using Trading.Application.DTOs.OptionChain;

public interface IOptionStrategyEngine
{
    public List<TradeSignal> GenerateSignals(OptionChainResponse chain, decimal spotPrice);
    public List<TradeSignal> GenerateTrades(OptionChainResponse chain, decimal spot);
}

public class OptionStrategyEngine : IOptionStrategyEngine
{
    public List<TradeSignal> GenerateSignals(OptionChainResponse chain, decimal spotPrice)
    {
        var signals = new List<TradeSignal>();

        if (chain.Data == null || !chain.Data.Any())
            return signals;

        decimal pcr = chain.TotalCallOI == 0 ? 0 :
                      (decimal)chain.TotalPutOI / chain.TotalCallOI;

        // 🔥 Find ATM dynamically
        var atm = chain.Data
            .OrderBy(x => Math.Abs((decimal)x.StrikePrice - spotPrice))
            .First();

        foreach (var x in chain.Data)
        {
            decimal strike = (decimal)x.StrikePrice;
            decimal distance = strike - spotPrice;

            bool isATM = x.StrikePrice == atm.StrikePrice;
            bool isNearATM = Math.Abs(strike - (decimal)atm.StrikePrice) <= 100;

            bool isOTMCall = strike > spotPrice;
            bool isOTMPut = strike < spotPrice;

            // Safe values
            decimal callDelta = x.CallDelta ?? 0;
            decimal putDelta = x.PutDelta ?? 0;

            decimal callTheta = x.CallTheta ?? -20; // assume worst
            decimal putTheta = x.PutTheta ?? -20;

            long callOI = x.CallOpenInterest;
            long putOI = x.PutOpenInterest;

            long callVol = x.CallVolume;
            long putVol = x.PutVolume;

            // =========================
            // 🔥 BUY CALL (Bullish)
            // =========================
            if (pcr > 1.1m &&
                isNearATM &&
                putOI > callOI &&                // support strength
                putVol > callVol)                // demand
            {
                signals.Add(new TradeSignal
                {
                    Signal = "BUY_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Confidence = CalculateConfidence(pcr, 0.6m, true)
                });
            }

            // =========================
            // 🔥 BUY PUT (Bearish)
            // =========================
            if (pcr < 0.9m &&
                isNearATM &&
                callOI > putOI &&               // resistance
                callVol > putVol)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "BUY_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Confidence = CalculateConfidence(pcr, 0.6m, true)
                });
            }

            // =========================
            // 🔥 SELL CALL (OTM writing)
            // =========================
            if (pcr < 1m &&
                isOTMCall &&
                callOI > putOI &&              // heavy call writing
                callTheta < -5)                // optional if available
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Confidence = CalculateConfidence(pcr, 0.3m, false)
                });
            }

            // =========================
            // 🔥 SELL PUT (OTM writing)
            // =========================
            if (pcr > 1m &&
                isOTMPut &&
                putOI > callOI &&              // heavy put writing
                putTheta < -5)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Confidence = CalculateConfidence(pcr, 0.3m, false)
                });
            }
        }

        return signals
            .OrderByDescending(x => x.Confidence)
            .Take(3) // keep best
            .ToList();
    }
    private double CalculateConfidence(decimal pcr, decimal strength, bool isBuy)
    {
        decimal score = 0;

        // PCR strength
        if (pcr > 1.2m || pcr < 0.8m)
            score += 0.4m;
        else if (pcr > 1.1m || pcr < 0.9m)
            score += 0.3m;
        else
            score += 0.2m;

        // OI/Volume strength (proxy instead of delta)
        if (strength > 0.6m)
            score += 0.4m;
        else if (strength > 0.4m)
            score += 0.3m;
        else
            score += 0.2m;

        if (isBuy)
            score += 0.1m;

        return (double)Math.Min(score, 1.0m);
    }

    public List<TradeSignal> GenerateTrades(OptionChainResponse chain, decimal spot)
    {
        var signals = new List<TradeSignal>();
        var ctx = AnalyzeMarket(chain, spot);

        foreach (var x in chain.Data)
        {
            decimal strike = (decimal)x.StrikePrice;
            bool isATM = strike == (decimal)ctx.ATM;
            bool nearATM = Math.Abs(strike - (decimal)ctx.ATM) <= 100;

            // =========================
            // 🚀 BREAKOUT BUY CALL
            // =========================
            if (ctx.Trend == "BULLISH" && nearATM)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "BUY_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Entry = x.CallLTP,
                    StopLoss = x.CallLTP * 0.75m,
                    Target = x.CallLTP * 1.5m,
                    Confidence = 0.8,
                    Reason = $"Bullish PCR ({ctx.PCR}) + Support at {ctx.Support}"
                });
            }

            // =========================
            // 🔻 BREAKOUT BUY PUT
            // =========================
            if (ctx.Trend == "BEARISH" && nearATM)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "BUY_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Entry = x.PutLTP,
                    StopLoss = x.PutLTP * 0.75m,
                    Target = x.PutLTP * 1.5m,
                    Confidence = 0.8,
                    Reason = $"Bearish PCR ({ctx.PCR}) + Resistance at {ctx.Resistance}"
                });
            }

            // =========================
            // 🧱 RANGE SELL CALL
            // =========================
            if (ctx.Trend == "SIDEWAYS" && strike > (decimal)ctx.ATM)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_CALL",
                    Strike = x.StrikePrice,
                    Symbol = x.CallSymbol,
                    Entry = x.CallLTP,
                    StopLoss = x.CallLTP * 1.3m,
                    Target = x.CallLTP * 0.5m,
                    Confidence = 0.7,
                    Reason = "Range-bound market, resistance holding"
                });
            }

            // =========================
            // 🧱 RANGE SELL PUT
            // =========================
            if (ctx.Trend == "SIDEWAYS" && strike < (decimal)ctx.ATM)
            {
                signals.Add(new TradeSignal
                {
                    Signal = "SELL_PUT",
                    Strike = x.StrikePrice,
                    Symbol = x.PutSymbol,
                    Entry = x.PutLTP,
                    StopLoss = x.PutLTP * 1.3m,
                    Target = x.PutLTP * 0.5m,
                    Confidence = 0.7,
                    Reason = "Range-bound market, support holding"
                });
            }
        }

        return signals
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

        var atm = chain.Data.OrderBy(x => Math.Abs((decimal)x.StrikePrice - spot)).First();
        context.ATM = atm.StrikePrice;

        var support = chain.Data.OrderByDescending(x => x.PutOpenInterest).First();
        var resistance = chain.Data.OrderByDescending(x => x.CallOpenInterest).First();

        context.Support = support.StrikePrice;
        context.Resistance = resistance.StrikePrice;

        // 🔥 Trend Logic
        if (context.PCR > 1.1m)
            context.Trend = "BULLISH";
        else if (context.PCR < 0.9m)
            context.Trend = "BEARISH";
        else
            context.Trend = "SIDEWAYS";

        return context;
    }
}