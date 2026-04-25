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
    }
}
