namespace Trading.Application.DTOs
{
    /// <summary>
    /// Request to validate access token
    /// </summary>
    public class ValidateTokenRequest
    {
        public string AccessToken { get; set; }
    }

    /// <summary>
    /// Response from token validation
    /// </summary>
    public class ValidateTokenResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// Request to get new access token
    /// </summary>
    public class GetAccessTokenRequest
    {
        public string AuthCode { get; set; }
    }

    /// <summary>
    /// Response with access token
    /// </summary>
    public class GetAccessTokenResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string Message { get; set; }
        public long ExpiresIn { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class AuthTokenResponse
    {
        public string Token { get; set; }
        public string refresh_token { get; set; }
        public string RESPONSE_MESSAGE { get; set; }
    }

    /// <summary>
    /// Step 1: Generate auth code response from Fyers
    /// </summary>
    public class FyersAuthCodeResponse
    {
        public string AuthCode { get; set; }
        public string State { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Step 2: Request to validate auth code and get access token
    /// </summary>
    public class FyersValidateAuthCodeRequest
    {
        public string GrantType { get; set; } = "authorization_code";
        public string AppIdHash { get; set; }
        public string Code { get; set; }
    }

    /// <summary>
    /// Step 2: Response from validating auth code with access token
    /// </summary>
    public class FyersValidateAuthCodeResponse
    {
        public int StatusCode { get; set; }
        public FyersAccessTokenData Data { get; set; }
    }

    public class FyersAccessTokenData
    {
        public string AccessToken { get; set; }
        public long ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string Message { get; set; }
    }
}
