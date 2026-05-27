using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace EgorkaCoins.Api.Services.Auth
{
    public class OAuthService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IDataProtector _stateProtector;

        public OAuthService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IDataProtectionProvider dataProtectionProvider)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _stateProtector = dataProtectionProvider.CreateProtector("EgorkaCoins.Api.OAuthState");
        }

        public string BuildAuthorizationUrl(string provider, string redirectUri)
        {
            var normalizedProvider = NormalizeProvider(provider);
            var state = CreateState(normalizedProvider);

            return normalizedProvider switch
            {
                "google" => QueryHelpers.AddQueryString(
                    "https://accounts.google.com/o/oauth2/v2/auth",
                    new Dictionary<string, string?>
                    {
                        ["client_id"] = GetClientId("Google"),
                        ["redirect_uri"] = redirectUri,
                        ["response_type"] = "code",
                        ["scope"] = "openid email profile",
                        ["prompt"] = "select_account",
                        ["state"] = state
                    }),
                "discord" => QueryHelpers.AddQueryString(
                    "https://discord.com/oauth2/authorize",
                    new Dictionary<string, string?>
                    {
                        ["client_id"] = GetClientId("Discord"),
                        ["redirect_uri"] = redirectUri,
                        ["response_type"] = "code",
                        ["scope"] = "identify email",
                        ["prompt"] = "consent",
                        ["state"] = state
                    }),
                _ => throw new InvalidOperationException("Unsupported OAuth provider")
            };
        }

        public async Task<OAuthUserInfo> GetUserInfoAsync(
            string provider,
            string code,
            string state,
            string redirectUri,
            CancellationToken cancellationToken = default)
        {
            var normalizedProvider = NormalizeProvider(provider);
            ValidateState(normalizedProvider, state);

            return normalizedProvider switch
            {
                "google" => await GetGoogleUserInfoAsync(code, redirectUri, cancellationToken),
                "discord" => await GetDiscordUserInfoAsync(code, redirectUri, cancellationToken),
                _ => throw new InvalidOperationException("Unsupported OAuth provider")
            };
        }

        public string BuildFrontendCallbackUrl(string? token, string? error)
        {
            var callbackUrl =
                _configuration["Authentication:FrontendCallbackUrl"] ??
                "http://localhost:5173/auth/callback";

            var builder = new StringBuilder(callbackUrl);
            builder.Append('#');

            if (!string.IsNullOrWhiteSpace(token))
            {
                builder.Append("token=");
                builder.Append(Uri.EscapeDataString(token));
            }
            else
            {
                builder.Append("error=");
                builder.Append(Uri.EscapeDataString(error ?? "OAuth login failed"));
            }

            return builder.ToString();
        }

        private async Task<OAuthUserInfo> GetGoogleUserInfoAsync(
            string code,
            string redirectUri,
            CancellationToken cancellationToken)
        {
            var tokenResponse = await ExchangeCodeAsync(
                "https://oauth2.googleapis.com/token",
                new Dictionary<string, string>
                {
                    ["client_id"] = GetClientId("Google"),
                    ["client_secret"] = GetClientSecret("Google"),
                    ["code"] = code,
                    ["redirect_uri"] = redirectUri,
                    ["grant_type"] = "authorization_code"
                },
                cancellationToken);

            using var userInfoRequest = new HttpRequestMessage(
                HttpMethod.Get,
                "https://openidconnect.googleapis.com/v1/userinfo");
            userInfoRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            using var userInfoResponse = await _httpClient.SendAsync(userInfoRequest, cancellationToken);
            if (!userInfoResponse.IsSuccessStatusCode)
                throw new InvalidOperationException("Google userinfo request failed");

            var payload = await userInfoResponse.Content.ReadAsStringAsync(cancellationToken);
            var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(payload, JsonOptions)
                ?? throw new InvalidOperationException("Google userinfo payload is invalid");

            if (string.IsNullOrWhiteSpace(userInfo.Sub) || string.IsNullOrWhiteSpace(userInfo.Email))
                throw new InvalidOperationException("Google account did not return required fields");

            var username = !string.IsNullOrWhiteSpace(userInfo.Name)
                ? userInfo.Name
                : userInfo.Email.Split('@')[0];

            return new OAuthUserInfo(
                "google",
                userInfo.Sub,
                userInfo.Email,
                username,
                userInfo.Picture,
                userInfo.EmailVerified);
        }

        private async Task<OAuthUserInfo> GetDiscordUserInfoAsync(
            string code,
            string redirectUri,
            CancellationToken cancellationToken)
        {
            var tokenResponse = await ExchangeCodeAsync(
                "https://discord.com/api/oauth2/token",
                new Dictionary<string, string>
                {
                    ["client_id"] = GetClientId("Discord"),
                    ["client_secret"] = GetClientSecret("Discord"),
                    ["code"] = code,
                    ["redirect_uri"] = redirectUri,
                    ["grant_type"] = "authorization_code"
                },
                cancellationToken);

            using var userInfoRequest = new HttpRequestMessage(
                HttpMethod.Get,
                "https://discord.com/api/users/@me");
            userInfoRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            using var userInfoResponse = await _httpClient.SendAsync(userInfoRequest, cancellationToken);
            if (!userInfoResponse.IsSuccessStatusCode)
                throw new InvalidOperationException("Discord userinfo request failed");

            var payload = await userInfoResponse.Content.ReadAsStringAsync(cancellationToken);
            var userInfo = JsonSerializer.Deserialize<DiscordUserInfo>(payload, JsonOptions)
                ?? throw new InvalidOperationException("Discord userinfo payload is invalid");

            if (string.IsNullOrWhiteSpace(userInfo.Id) || string.IsNullOrWhiteSpace(userInfo.Email))
                throw new InvalidOperationException("Discord account did not return required fields");

            return new OAuthUserInfo(
                "discord",
                userInfo.Id,
                userInfo.Email,
                userInfo.GlobalName ?? userInfo.Username ?? userInfo.Email.Split('@')[0],
                BuildDiscordAvatarUrl(userInfo),
                userInfo.Verified);
        }

        private async Task<OAuthTokenResponse> ExchangeCodeAsync(
            string url,
            Dictionary<string, string> formValues,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(formValues)
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("OAuth token exchange failed");

            var payload = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(payload, JsonOptions)
                ?? throw new InvalidOperationException("OAuth token payload is invalid");

            if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                throw new InvalidOperationException("OAuth provider did not return an access token");

            return tokenResponse;
        }

        private string CreateState(string provider)
        {
            var state = new OAuthState(provider, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), RandomNumberGenerator.GetHexString(16));
            var json = JsonSerializer.Serialize(state);
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_stateProtector.Protect(json)));
        }

        private void ValidateState(string provider, string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new InvalidOperationException("Missing OAuth state");

            string protectedPayload;

            try
            {
                protectedPayload = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(state));
            }
            catch
            {
                throw new InvalidOperationException("Invalid OAuth state");
            }

            OAuthState? payload;

            try
            {
                payload = JsonSerializer.Deserialize<OAuthState>(_stateProtector.Unprotect(protectedPayload), JsonOptions);
            }
            catch
            {
                throw new InvalidOperationException("OAuth state validation failed");
            }

            if (payload == null || payload.Provider != provider)
                throw new InvalidOperationException("OAuth state provider mismatch");

            var issuedAt = DateTimeOffset.FromUnixTimeSeconds(payload.IssuedAtUnixSeconds);
            if (DateTimeOffset.UtcNow - issuedAt > TimeSpan.FromMinutes(10))
                throw new InvalidOperationException("OAuth state expired");
        }

        private string GetClientId(string providerName)
        {
            return _configuration[$"Authentication:{providerName}:ClientId"]
                ?? throw new InvalidOperationException($"{providerName} ClientId is not configured");
        }

        private string GetClientSecret(string providerName)
        {
            return _configuration[$"Authentication:{providerName}:ClientSecret"]
                ?? throw new InvalidOperationException($"{providerName} ClientSecret is not configured");
        }

        private static string NormalizeProvider(string provider)
        {
            var normalized = provider.Trim().ToLowerInvariant();
            if (normalized != "google" && normalized != "discord")
                throw new InvalidOperationException("Unsupported OAuth provider");

            return normalized;
        }

        private static string? BuildDiscordAvatarUrl(DiscordUserInfo userInfo)
        {
            if (string.IsNullOrWhiteSpace(userInfo.Avatar))
                return null;

            return $"https://cdn.discordapp.com/avatars/{userInfo.Id}/{userInfo.Avatar}.png";
        }

        private sealed record OAuthState(string Provider, long IssuedAtUnixSeconds, string Nonce);

        private sealed class OAuthTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;
        }

        private sealed class GoogleUserInfo
        {
            [JsonPropertyName("sub")]
            public string Sub { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("picture")]
            public string? Picture { get; set; }

            [JsonPropertyName("email_verified")]
            public bool EmailVerified { get; set; }
        }

        private sealed class DiscordUserInfo
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;

            [JsonPropertyName("username")]
            public string? Username { get; set; }

            [JsonPropertyName("global_name")]
            public string? GlobalName { get; set; }

            [JsonPropertyName("avatar")]
            public string? Avatar { get; set; }

            [JsonPropertyName("verified")]
            public bool Verified { get; set; }
        }
    }

    public sealed record OAuthUserInfo(
        string Provider,
        string ProviderUserId,
        string Email,
        string Username,
        string? AvatarUrl,
        bool Verified);
}
