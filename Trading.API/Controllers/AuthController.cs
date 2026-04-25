using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Trading.Application.DTOs;
using Trading.Infrastructure.Services;

namespace Trading.API.Controllers
{
    /// <summary>
    /// Authentication Controller - Handle Fyers OAuth token validation and refresh
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IFyersAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IFyersAuthenticationService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Get the Fyers authorization code
        /// Directly hits the Fyers OAuth endpoint and extracts the auth code
        /// </summary>
        /// <returns>Authorization code from Fyers</returns>
        [HttpGet("auth-code-url")]
        public async Task<IActionResult> GetAuthCodeUrl()
        {
            try
            {
                var authCode = await _authService.GetAuthCodeUrlAsync();

                // Check if we got a code or just a URL
                if (authCode.StartsWith("http"))
                {
                    return Ok(new
                    {
                        success = true,
                        authCodeUrl = authCode,
                        message = "Visit this URL to get your auth code"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        authCode = authCode,
                        message = "Authorization code obtained successfully",
                        nextStep = "Use this code with POST /api/auth/get-access-token endpoint"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting auth code URL: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get access token using authorization code
        /// This is called after user visits the auth code URL and gets the code
        /// </summary>
        /// <param name="request">Request with authorization code</param>
        /// <returns>Access token response with expiration</returns>
        [HttpPost("get-access-token")]
        public async Task<IActionResult> GetAccessToken([FromBody] GetAccessTokenRequest request)
        {
            if (string.IsNullOrEmpty(request?.AuthCode))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Authorization code is required"
                });
            }

            try
            {
                var result = await _authService.GetAccessTokenFromAuthCodeAsync(request.AuthCode);

                if (result.RESPONSE_MESSAGE == "SUCCESS")
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(400, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting access token: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// OAuth callback endpoint - handles redirect from Fyers
        /// Fyers redirects here with the authorization code after user approves
        /// </summary>
        /// <param name="code">Authorization code from Fyers</param>
        /// <param name="state">State parameter for CSRF protection</param>
        /// <returns>HTML page showing the auth code or error</returns>
        [HttpGet("callback")]
        public IActionResult OAuthCallback([FromQuery] string auth_code)
        {
            try
            {
                 // Check if code is present
                if (string.IsNullOrEmpty(auth_code))
                {
                    _logger.LogError("Authorization code not provided in callback");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Authorization code not provided"
                    });
                }       

                return Ok(auth_code);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in OAuth callback: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }
    }
}
