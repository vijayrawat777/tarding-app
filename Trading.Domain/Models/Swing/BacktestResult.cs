using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Domain.Models.Swing
{
    public class BacktestResult
    {
        public int TotalTrades { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal MaxDrawdown { get; set; }
        public double WinRate => TotalTrades == 0 ? 0 : (double)Wins / TotalTrades;
    }
}
