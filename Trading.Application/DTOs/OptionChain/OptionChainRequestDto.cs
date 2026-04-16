using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Application.DTOs.OptionChain
{
    public class OptionChainRequestDto
    {
        public string AccessToken { get; set; }
        public string ExpiryDate { get; set; }
        public string Symbol { get; set; }
        public int StrikeCount { get; set; }
        public string Timestamp { get; set; }

        public string Greek { get; set; } = "1";
    }
}
