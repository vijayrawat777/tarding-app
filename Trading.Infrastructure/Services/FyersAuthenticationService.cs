using FyersCSharpSDK;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using Trading.Application.DTOs;
using Trading.Infrastructure.Configuration;

namespace Trading.Infrastructure.Services
{
    /// <summary>
    /// Fyers authentication service for token validation and refresh
    /// </summary>
    public interface IFyersAuthenticationService
    {
        Task<ValidateTokenResponse> ValidateTokenAsync(string accessToken);
        Task<AuthTokenResponse> GetAccessTokenFromAuthCodeAsync(string authCode);
        Task<string> GetAuthCodeUrlAsync();
    }

    public class FyersAuthenticationService : IFyersAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly FyersConfig _fyersConfig;
        private readonly ILogger<FyersAuthenticationService> _logger;

        private const string FyersAuthUrl = "https://api-t1.fyers.in/api/v3";

        public FyersAuthenticationService(
            HttpClient httpClient,
            FyersConfig fyersConfig,
            ILogger<FyersAuthenticationService> logger)
        {
            _httpClient = httpClient;
            _fyersConfig = fyersConfig;
            _logger = logger;
        }

        /// <summary>
        /// Get authorization code by hitting the Fyers auth URL
        /// Makes an HTTP request to Fyers OAuth endpoint and extracts the auth code
        /// </summary>
        public Task<string> GetAuthCodeUrlAsync()
        {
            try
            {
                // Construct the authorization URL according to Fyers OAuth 2.0 flow
                var authCodeUrl = $"{FyersAuthUrl}/generate-authcode?" +
                    $"client_id={Uri.EscapeDataString(_fyersConfig.AppId)}&" +
                    $"redirect_uri={Uri.EscapeDataString(_fyersConfig.RedirectUrl)}&" +
                    $"response_type=code&" +
                    $"state=sample_state";

                _logger.LogInformation($"Authorization URL: {authCodeUrl}");

                FyersClass fyersModel = FyersClass.Instance;
                fyersModel.GetGenerateCode(_fyersConfig.AppId, _fyersConfig.AppSecret, _fyersConfig.RedirectUrl);

                return Task.FromResult(authCodeUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating auth code URL: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extract auth code from Fyers response or redirect URL
        /// </summary>
        private string ExtractAuthCodeFromResponse(string responseContent, string responseUrl)
        {
            try
            {
                // Try to extract from URL query parameters first (most reliable for OAuth redirect)
                if (responseUrl.Contains("auth_code="))
                {
                    var uri = new Uri(responseUrl);
                    var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    var code = queryParams.Get("auth_code");
                    if (!string.IsNullOrEmpty(code))
                    {
                        _logger.LogInformation("Auth code extracted from URL query parameters");
                        return code;
                    }
                }

                // Try to extract from JSON response (if API returns JSON)
                if (responseContent.Contains("code") && responseContent.TrimStart().StartsWith("{"))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(responseContent);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("code", out var codeElement))
                        {
                            var code = codeElement.GetString();
                            if (!string.IsNullOrEmpty(code))
                            {
                                _logger.LogInformation("Auth code extracted from JSON 'code' property");
                                return code;
                            }
                        }

                        if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Object && 
                            dataElement.TryGetProperty("code", out var dataCode))
                        {
                            var code = dataCode.GetString();
                            if (!string.IsNullOrEmpty(code))
                            {
                                _logger.LogInformation("Auth code extracted from JSON 'data.code' property");
                                return code;
                            }
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogWarning($"Failed to parse JSON response: {jsonEx.Message}");
                    }
                }

                // Try to extract from HTML response (if embedded in page)
                if (responseContent.Contains("code=") || responseContent.Contains("\"code\"") || responseContent.Contains("auth_code="))
                {
                    // Try to find auth_code parameter first (Fyers uses this)
                    var match = System.Text.RegularExpressions.Regex.Match(responseContent, @"auth_code=([a-zA-Z0-9_\-\.]+)");
                    if (match.Success && match.Groups.Count > 1)
                    {
                        var code = match.Groups[1].Value;
                        if (!string.IsNullOrEmpty(code))
                        {
                            _logger.LogInformation("Auth code extracted from HTML 'auth_code' parameter");
                            return code;
                        }
                    }

                    // Fallback to generic code extraction
                    match = System.Text.RegularExpressions.Regex.Match(responseContent, @"code[""=:]*([a-zA-Z0-9_\-\.]+)");
                    if (match.Success && match.Groups.Count > 1)
                    {
                        var code = match.Groups[1].Value;
                        if (!string.IsNullOrEmpty(code))
                        {
                            _logger.LogInformation("Auth code extracted from HTML 'code' parameter");
                            return code;
                        }
                    }
                }

                _logger.LogWarning("Auth code not found in response content or URL");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error extracting auth code: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validate if access token is valid
        /// Real validation would require calling Fyers API
        /// For now, we check token structure and expiration
        /// </summary>
        public async Task<ValidateTokenResponse> ValidateTokenAsync(string accessToken)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Access token is empty"
                    };
                }

                // Try to decode JWT token to check expiration
                var tokenParts = accessToken.Split('.');
                if (tokenParts.Length != 3)
                {
                    return new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Invalid token format"
                    };
                }

                // Decode payload (second part)
                var payload = tokenParts[1];
                // Add padding if needed
                var padding = 4 - (payload.Length % 4);
                if (padding != 4)
                {
                    payload += new string('=', padding);
                }

                var decodedBytes = Convert.FromBase64String(payload);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);

                using var doc = JsonDocument.Parse(decodedString);
                var root = doc.RootElement;

                if (root.TryGetProperty("exp", out var expElement) && expElement.TryGetInt64(out var expUnix))
                {
                    var expiresAt = UnixTimeStampToDateTime(expUnix);
                    var isValid = expiresAt > DateTime.UtcNow;

                    _logger.LogInformation($"Token validation: Valid={isValid}, ExpiresAt={expiresAt}");

                    return new ValidateTokenResponse
                    {
                        IsValid = isValid,
                        Message = isValid ? "Token is valid" : "Token has expired",
                        ExpiresAt = expiresAt
                    };
                }

                return new ValidateTokenResponse
                {
                    IsValid = false,
                    Message = "Could not extract expiration from token"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating token: {ex.Message}");
                return new ValidateTokenResponse
                {
                    IsValid = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Exchange auth code for access token
        /// Step 1: Call GET to get auth code
        /// Step 2: Call POST to validate auth code and get access token
        /// </summary>
        public async Task<AuthTokenResponse> GetAccessTokenFromAuthCodeAsync(string authCode)
        {
            try
            {
                if (string.IsNullOrEmpty(authCode))
                {
                    return new AuthTokenResponse
                    {
                        Token = null,
                        refresh_token = null,
                        RESPONSE_MESSAGE = "Auth code is required"
                    };
                }

                FyersClass fyersModel = FyersClass.Instance;
                //string appHashId = Utility.GenerateAppHashID(clientID, secretKey);

                JObject authTokenJobject = await fyersModel.GenerateToken(_fyersConfig.AppSecret, _fyersConfig.RedirectUrl, authCode, _fyersConfig.AppIdHash);
                var authToken = authTokenJobject.ToObject<AuthTokenResponse>();

                if (authToken?.RESPONSE_MESSAGE == "SUCCESS")
                {


                    return new AuthTokenResponse
                    {
                        Token = authToken.Token,
                        refresh_token = authToken.refresh_token,
                        RESPONSE_MESSAGE = authToken.RESPONSE_MESSAGE
                    };
                   
                }

                return new AuthTokenResponse
                {
                    Token = null,
                    refresh_token = null,
                    RESPONSE_MESSAGE = "No access token in response"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting access token: {ex.Message}");
                return new AuthTokenResponse
                {
                    Token = null,
                    refresh_token = null,
                    RESPONSE_MESSAGE = $"Error: {ex.Message}"
                };
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTime;
        }
    }
}

