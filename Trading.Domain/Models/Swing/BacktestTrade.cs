using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Domain.Models.Swing
{
    public class BacktestTrade
    {
        public string Symbol { get; set; }
        public decimal Entry { get; set; }
        public decimal Exit { get; set; }
        public decimal StopLoss { get; set; }
        public decimal Target { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public bool IsWin { get; set; }
    }
}
