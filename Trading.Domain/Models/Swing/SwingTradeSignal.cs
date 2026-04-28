using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Domain.Models.Swing
{
    public class SwingTradeSignal
    {
        public string Symbol { get; set; }
        public string SignalType { get; set; } // BUY / SELL
        public decimal EntryPrice { get; set; }
        public decimal Target { get; set; }
        public decimal StopLoss { get; set; }
        public double Confidence { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Score { get; set; }
        public List<string> Reasons { get; set; } = new List<string>();
    }
}
