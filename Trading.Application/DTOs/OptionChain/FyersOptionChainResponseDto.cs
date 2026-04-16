namespace Trading.Application.DTOs.OptionChain;

public class FyersOptionChainResponseDto
{
    public string? CallOi { get; set; }
    public string? PutOi { get; set; }
    public List<ExpiryDataDto> ExpiryData { get; set; } = new();
    public List<FyersOptionDto> IndiavixData { get; set; } = new();
    public List<FyersOptionDto> OptionsChainData { get; set; } = new();
}

public class ExpiryDataDto
{
    public string Date { get; set; } = string.Empty;
    public string Expiry { get; set; } = string.Empty;
}

public class FyersOptionDto
{
    public decimal Ask { get; set; }
    public decimal Bid { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Ex_symbol { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string FyToken { get; set; } = string.Empty;
    public decimal Fp { get; set; }
    public decimal Fpch { get; set; }
    public decimal Fpchp { get; set; }
    public decimal Ltp { get; set; }
    public decimal Ltpch { get; set; }
    public decimal Ltpchp { get; set; }
    public string Option_type { get; set; } = string.Empty;
    public decimal Strike_price { get; set; }
    public string Symbol { get; set; } = string.Empty;

    // Optional Greeks (if returned by API with greeks=1)
    public decimal? Delta { get; set; }
    public decimal? Gamma { get; set; }
    public decimal? Theta { get; set; }
    public decimal? Vega { get; set; }
    public decimal? ImpliedVolatility { get; set; }
    public long? Volume { get; set; }
    public long? OpenInterest { get; set; }
}
