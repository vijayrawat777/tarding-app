using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Application.DTOs.OptionChain
{
    public class TradeSignal
    {
        public string Signal { get; set; } // BUY_CALL, BUY_PUT, SELL_CALL, SELL_PUT
        public double Strike { get; set; }
        public string Symbol { get; set; }

        public decimal Entry { get; set; }
        public decimal StopLoss { get; set; }
        public decimal Target { get; set; }

        public double Confidence { get; set; }

        public string Reason { get; set; }
    }
}
