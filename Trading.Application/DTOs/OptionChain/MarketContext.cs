using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Application.DTOs.OptionChain
{
    public class MarketContext
    {
        public decimal Spot { get; set; }
        public decimal PCR { get; set; }

        public decimal ATM { get; set; }
        public decimal Support { get; set; }
        public decimal Resistance { get; set; }

        public string Trend { get; set; } // BULLISH / BEARISH / SIDEWAYS
        public decimal AvgCallVolume { get; set; }
        public decimal AvgPutVolume { get; set; }
        public decimal AvgCallOI { get; set; }
        public decimal AvgPutOI { get; set; }
        public decimal AvgIV { get; set; }
        public decimal VIX { get; set; }
    }
}
