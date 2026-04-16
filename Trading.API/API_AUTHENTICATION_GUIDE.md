# Trading API - Authentication Guide

## Overview

The Trading API uses **Fyers OAuth 2.0** flow for authentication. This guide explains the two-step authentication process and how to use the API endpoints.

## Authentication Flow

### Step 1: Get Authorization Code URL
- **Endpoint:** `GET /api/auth/auth-code-url`
- **Purpose:** Get the URL that user needs to visit to authorize the application

**Response:**
```json
{
  "success": true,
  "authCodeUrl": "https://api-t1.fyers.in/api/v3/generate-authcode?client_id=26ZQB99CSZ-200&redirect_uri=https://localhost:7047/&response_type=code&state=sample_state",
  "message": "Visit this URL to get your auth code"
}
```

**What to do:**
1. Copy the `authCodeUrl`
2. Open it in a browser
3. Authorize the application
4. You'll be redirected and get an authorization `code` as a URL parameter

### Step 2: Exchange Code for Access Token
- **Endpoint:** `POST /api/auth/get-access-token`
- **Purpose:** Exchange the authorization code for an access token

**Request:**
```json
{
  "authCode": "YOUR_AUTH_CODE_HERE"
}
```

**Response:**
```json
{
  "success": true,
  "accessToken": "eyJ0eXAiOiJKV1QiLCJhbGc...",
  "message": "Access token obtained successfully",
  "expiresIn": 86400,
  "expiresAt": "2026-04-16T08:00:00Z"
}
```

### Step 3: Validate Token (Optional)
- **Endpoint:** `POST /api/auth/validate-token`
- **Purpose:** Check if access token is still valid before making API calls

**Request:**
```json
{
  "accessToken": "YOUR_ACCESS_TOKEN"
}
```

**Response (Valid Token):**
```json
{
  "isValid": true,
  "message": "Token is valid",
  "expiresAt": "2026-04-16T08:00:00Z"
}
```

**Response (Expired Token):**
```json
{
  "isValid": false,
  "message": "Token has expired"
}
```

## Complete Workflow Example

### Using Swagger UI (Recommended for Testing)

1. **Start the API:**
   ```powershell
   cd Trading.API
   dotnet run
   ```

2. **Open Swagger UI:**
   - Navigate to: `https://localhost:7047`
   - You'll see the Swagger documentation

3. **Get Auth Code URL:**
   - Click on `/api/auth/auth-code-url` endpoint
   - Click "Try it out"
   - Click "Execute"
   - Copy the `authCodeUrl` from the response

4. **Get Authorization Code:**
   - Open the `authCodeUrl` in a new browser tab
   - Authorize the application
   - Look for the `code` parameter in the redirect URL
   - Example: `https://localhost:7047/?code=ABC123&state=sample_state`
   - Copy the code value (e.g., `ABC123`)

5. **Get Access Token:**
   - Go back to Swagger UI
   - Click on `/api/auth/get-access-token` endpoint
   - Click "Try it out"
   - Paste this in the request body:
     ```json
     {
       "authCode": "ABC123"
     }
     ```
   - Click "Execute"
   - You'll receive your `accessToken` and `expiresAt` time

6. **Validate Token (Optional):**
   - Click on `/api/auth/validate-token` endpoint
   - Click "Try it out"
   - Paste your access token:
     ```json
     {
       "accessToken": "YOUR_ACCESS_TOKEN_HERE"
     }
     ```
   - Click "Execute"

### Using cURL Commands

```bash
# Step 1: Get Auth Code URL
curl -X GET "https://localhost:7047/api/auth/auth-code-url" \
  -H "accept: application/json"

# Step 2: Get Access Token (replace AUTH_CODE with your actual code)
curl -X POST "https://localhost:7047/api/auth/get-access-token" \
  -H "Content-Type: application/json" \
  -d '{"authCode":"YOUR_AUTH_CODE_HERE"}'

# Step 3: Validate Token
curl -X POST "https://localhost:7047/api/auth/validate-token" \
  -H "Content-Type: application/json" \
  -d '{"accessToken":"YOUR_ACCESS_TOKEN_HERE"}'
```

### Using Postman

1. **Import the API:**
   - Open Postman
   - Create new requests:
   
2. **Request 1 - Get Auth Code URL:**
   ```
   Method: GET
   URL: https://localhost:7047/api/auth/auth-code-url
   ```

3. **Request 2 - Get Access Token:**
   ```
   Method: POST
   URL: https://localhost:7047/api/auth/get-access-token
   Body (JSON):
   {
     "authCode": "YOUR_CODE_HERE"
   }
   ```

4. **Request 3 - Validate Token:**
   ```
   Method: POST
   URL: https://localhost:7047/api/auth/validate-token
   Body (JSON):
   {
     "accessToken": "YOUR_TOKEN_HERE"
   }
   ```

## Important Configuration

The API credentials are stored in `appsettings.json`:

```json
{
  "FyersConfig": {
    "ClientId": "26ZQB99CSZ-200",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "RedirectUri": "https://localhost:7047/",
    "AppIdHash": "07e6fefaf039bfad106dbc12af5b49f153ee9d3e5dc45a2dd1f94208d8b6f6f2"
  }
}
```

**Important:** 
- The `ClientId` and `AppIdHash` are public identifiers for YOUR application
- Change `RedirectUri` if hosting on a different URL
- Store `ClientSecret` securely (not in appsettings.json for production)

## Token Lifecycle

1. **Token Expiration:** Tokens expire after `expiresIn` seconds (usually 24 hours)
2. **Before Expiration:** Validate token using the validate endpoint
3. **After Expiration:** Repeat the authentication flow to get a new token

## Error Handling

### Common Errors

| Status Code | Error | Solution |
|------------|-------|----------|
| 400 | `Access token is empty` | Provide a non-empty authorization code or token |
| 400 | `Invalid token format` | Token should be a valid JWT with 3 parts (header.payload.signature) |
| 400 | `Token has expired` | Get a new access token using the auth code |
| 500 | `Failed to get access token` | Check your auth code is correct and not expired |
| 500 | `Error: ...` | Check API logs for detailed error information |

## Security Notes

1. **HTTPS Only:** Always use HTTPS in production
2. **Token Storage:** Store tokens securely (not in plain text or local storage if possible)
3. **Token Reuse:** Reuse tokens for multiple API calls (don't refresh on every call)
4. **Token Validation:** Validate tokens before making expensive operations
5. **OAuth State:** The `state` parameter helps prevent CSRF attacks

## Fyers API Documentation

For more information about Fyers API:
- Visit: https://api-docs.fyers.in/
- Contact: support@fyers.in

