using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace Oauth_1a_Demo.Services
{
    public class OAuthValidator
    {
        private readonly IConfiguration _config;

        public OAuthValidator(IConfiguration config)
        {
            _config = config;
        }

        private string DecryptSecret(string encryptedSecret)
        {
            var decryptionKey = _config["OAuth:DecryptionKey"];
            if (string.IsNullOrEmpty(decryptionKey))
                throw new InvalidOperationException("Decryption key is missing from configuration.");

            return EncryptionHelper.Decrypt(encryptedSecret, decryptionKey);
        }

        public bool Validate(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("OAuth "))
                return false;

            var oauthParams = ParseOAuthHeader(authHeader);

            var timestamp = long.Parse(oauthParams["oauth_timestamp"]);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var allowedWindow = 300;
            if (Math.Abs(now - timestamp) > allowedWindow)
                return false;

            var consumerKey = oauthParams["oauth_consumer_key"];
            var accessToken = oauthParams["oauth_token"];

            var consumerCredentials = GetConsumerCredentials(consumerKey,accessToken);

            if (consumerCredentials == null || !consumerCredentials.AccessTokens.ContainsKey(accessToken))
                return false;


            var consumerSecret = DecryptSecret(consumerCredentials.ConsumerSecret); 
            var tokenSecret = DecryptSecret(consumerCredentials.AccessTokens[accessToken]);
            //var tokenSecret = consumerCredentials.AccessTokens[accessToken];

            var signatureBaseString = BuildSignatureBaseString(request, oauthParams);
            var signingKey = $"{UrlEncode(consumerSecret)}&{UrlEncode(tokenSecret)}";

            using var hasher = new System.Security.Cryptography.HMACSHA1(System.Text.Encoding.ASCII.GetBytes(signingKey));
            var hash = hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes(signatureBaseString));
            var computedSignature = Convert.ToBase64String(hash);

            return computedSignature == oauthParams["oauth_signature"];
        }

        private Dictionary<string, string> ParseOAuthHeader(string header)
        {
            var result = new Dictionary<string, string>();

            var parts = header.Replace("OAuth ", "").Split(',');

            foreach (var part in parts)
            {
                var kv = part.Trim().Split('=');
                var key = kv[0];
                var value = kv[1].Trim('"');
                result[key] = Uri.UnescapeDataString(value);
            }

            return result;
        }

        private ConsumerCredentials GetConsumerCredentials(string consumerKey,string accessToken)
        {
            var consumerSection = _config.GetSection($"OAuth:Consumers:{consumerKey}");
            if (consumerSection == null)
                return null;

            var consumerSecret = consumerSection.GetValue<string>("ConsumerSecret");
            var accessTokensSection = consumerSection.GetSection("AccessTokens");
            var tokenSecret = accessTokensSection.GetValue<string>(accessToken);
            if (string.IsNullOrEmpty(tokenSecret))
                return null; // If the provided access token is not found, return null

            return new ConsumerCredentials
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                AccessTokens = new Dictionary<string, string> { { accessToken, tokenSecret } }
            };
        }

        private string BuildSignatureBaseString(HttpRequest request, Dictionary<string, string> oauthParams)
        {
            var method = request.Method.ToUpper();
            var url = $"{request.Scheme}://{request.Host}{request.Path}";

            var allParams = new SortedDictionary<string, string>();

            foreach (var param in request.Query)
                allParams[param.Key] = param.Value;

            foreach (var param in oauthParams)
            {
                if (param.Key != "oauth_signature")
                    allParams[param.Key] = param.Value;
            }

            var parameterString = string.Join("&",
                allParams.Select(p => $"{UrlEncode(p.Key)}={UrlEncode(p.Value)}"));

            return $"{method}&{UrlEncode(url)}&{UrlEncode(parameterString)}";
        }

        private string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value);
        }

    }
}
