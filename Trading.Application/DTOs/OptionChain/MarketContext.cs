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

        public double ATM { get; set; }
        public double Support { get; set; }
        public double Resistance { get; set; }

        public string Trend { get; set; } // BULLISH / BEARISH / SIDEWAYS
        public double AvgCallVolume { get; set; }
        public double AvgPutVolume { get; set; }
        public double AvgCallOI { get; set; }
        public double AvgPutOI { get; set; }
        public decimal AvgIV { get; set; }
    }
}
