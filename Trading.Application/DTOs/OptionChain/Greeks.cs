namespace Trading.Application.DTOs.OptionChain
{
    public class Greeks
    {
        public decimal Delta { get; set; }
        public decimal Gamma { get; set; }
        public decimal Theta { get; set; }
        public decimal Vega { get; set; }
        public decimal Rho { get; set; }
        public decimal IV { get; set; }
    }
}