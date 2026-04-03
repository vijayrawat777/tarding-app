using System;

namespace Trading.Domain.Models
{
    public class Indicator
    {
        public string Symbol { get; private set; }
        public DateTime Timestamp { get; private set; }
        
        // RSI (Relative Strength Index)
        public decimal? RSI { get; private set; }
        public int RSIPeriod { get; private set; }
        
        // EMA (Exponential Moving Average)
        public decimal? EMA5 { get; private set; }
        public decimal? EMA10 { get; private set; }
        public decimal? EMA20 { get; private set; }
        public decimal? EMA50 { get; private set; }
        public decimal? EMA200 { get; private set; }
        
        // SMA (Simple Moving Average)
        public decimal? SMA20 { get; private set; }
        public decimal? SMA50 { get; private set; }
        
        // MACD (Moving Average Convergence Divergence)
        public decimal? MACD { get; private set; }
        public decimal? MACDSignal { get; private set; }
        public decimal? MACDHistogram { get; private set; }
        
        // Bollinger Bands
        public decimal? BollingerUpper { get; private set; }
        public decimal? BollingerMiddle { get; private set; }
        public decimal? BollingerLower { get; private set; }
        
        // Volume indicators
        public decimal? VolumeSMA { get; private set; }
        public long? Volume { get; private set; }
        
        // Stochastic Oscillator
        public decimal? StochasticK { get; private set; }
        public decimal? StochasticD { get; private set; }
        
        // Williams %R
        public decimal? WilliamsR { get; private set; }
        
        // Commodity Channel Index
        public decimal? CCI { get; private set; }
        
        // Average True Range
        public decimal? ATR { get; private set; }
        
        public DateTime CalculatedAt { get; private set; }

        private Indicator() { } // EF Core constructor

        public Indicator(string symbol, DateTime timestamp)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Timestamp = timestamp;
            RSIPeriod = 14;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateRSI(decimal rsi, int period = 14)
        {
            RSI = rsi;
            RSIPeriod = period;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateEMAs(decimal? ema5, decimal? ema10, decimal? ema20, decimal? ema50, decimal? ema200)
        {
            EMA5 = ema5;
            EMA10 = ema10;
            EMA20 = ema20;
            EMA50 = ema50;
            EMA200 = ema200;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateSMAs(decimal? sma20, decimal? sma50)
        {
            SMA20 = sma20;
            SMA50 = sma50;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateMACD(decimal macd, decimal signal, decimal histogram)
        {
            MACD = macd;
            MACDSignal = signal;
            MACDHistogram = histogram;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateBollingerBands(decimal upper, decimal middle, decimal lower)
        {
            BollingerUpper = upper;
            BollingerMiddle = middle;
            BollingerLower = lower;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateVolumeIndicators(decimal? volumeSMA, long? volume)
        {
            VolumeSMA = volumeSMA;
            Volume = volume;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateStochastic(decimal k, decimal d)
        {
            StochasticK = k;
            StochasticD = d;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateWilliamsR(decimal williamsR)
        {
            WilliamsR = williamsR;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateCCI(decimal cci)
        {
            CCI = cci;
            CalculatedAt = DateTime.UtcNow;
        }

        public void UpdateATR(decimal atr)
        {
            ATR = atr;
            CalculatedAt = DateTime.UtcNow;
        }
    }
}
