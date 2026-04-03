using System;

namespace Trading.Application.DTOs
{
    public class IndicatorDto
    {
        public string Symbol { get; set; }
        public DateTime Timestamp { get; set; }
        
        // RSI (Relative Strength Index)
        public decimal? RSI { get; set; }
        public int RSIPeriod { get; set; } = 14;
        
        // EMA (Exponential Moving Average)
        public decimal? EMA5 { get; set; }
        public decimal? EMA10 { get; set; }
        public decimal? EMA20 { get; set; }
        public decimal? EMA50 { get; set; }
        public decimal? EMA200 { get; set; }
        
        // SMA (Simple Moving Average)
        public decimal? SMA20 { get; set; }
        public decimal? SMA50 { get; set; }
        
        // MACD (Moving Average Convergence Divergence)
        public decimal? MACD { get; set; }
        public decimal? MACDSignal { get; set; }
        public decimal? MACDHistogram { get; set; }
        
        // Bollinger Bands
        public decimal? BollingerUpper { get; set; }
        public decimal? BollingerMiddle { get; set; }
        public decimal? BollingerLower { get; set; }
        
        // Volume indicators
        public decimal? VolumeSMA { get; set; }
        public long? Volume { get; set; }
        
        // Stochastic Oscillator
        public decimal? StochasticK { get; set; }
        public decimal? StochasticD { get; set; }
        
        // Williams %R
        public decimal? WilliamsR { get; set; }
        
        // Commodity Channel Index
        public decimal? CCI { get; set; }
        
        // Average True Range
        public decimal? ATR { get; set; }
        
        public DateTime CalculatedAt { get; set; }
    }
}
