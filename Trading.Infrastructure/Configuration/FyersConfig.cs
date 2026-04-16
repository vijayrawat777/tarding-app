namespace Trading.Infrastructure.Configuration;

/// <summary>
/// Fyers API Configuration
/// </summary>
public class FyersConfig
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AuthCode { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = "https://localhost:7047/";
    public string BaseUrl { get; set; } = "https://api.fyers.in";
    public bool IsProduction { get; set; } = false;
    public string AppIdHash { get; set; } = string.Empty;
}
