using OauthApiCaller.Model;
using System.Security.Cryptography;
using System.Text;
namespace OauthApiCaller
{
    public class OAuth1Helper
    {
        public static string GenerateAuthorizationHeader(string url,HttpMethod method,OAuthConfig config, Dictionary<string, string>? queryParams = null)
        {
            var oauthParams = new Dictionary<string, string>
            {
                { "oauth_consumer_key", config.ConsumerKey },
                { "oauth_token", config.AccessToken },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", GenerateTimeStamp() },
                { "oauth_nonce", GenerateNonce() },
                { "oauth_version", "1.0" }
            };

            var allParams = new SortedDictionary<string, string>(oauthParams);
            if (queryParams != null)
            {
                foreach (var qp in queryParams)
                    allParams.Add(qp.Key, qp.Value);
            }

            string signatureBaseString = BuildSignatureBaseString(url, method.Method, allParams);

            string signingKey = $"{UrlEncode(config.ConsumerSecret)}&{UrlEncode(config.TokenSecret)}";

            using var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
            var hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
            string signature = Convert.ToBase64String(hash);

            oauthParams.Add("oauth_signature", signature);

            return "OAuth " + string.Join(", ",
                oauthParams.Select(kvp =>
                    $"{kvp.Key}=\"{UrlEncode(kvp.Value)}\""));
        }

        private static string BuildSignatureBaseString(string url, string method, SortedDictionary<string, string> parameters)
        {
            var parameterString = string.Join("&",
                parameters.Select(kvp =>
                    $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));

            return $"{method.ToUpper()}&{UrlEncode(url)}&{UrlEncode(parameterString)}";
        }

        private static string GenerateTimeStamp()
            => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        private static string GenerateNonce()
            => Guid.NewGuid().ToString("N");

        private static string UrlEncode(string value)
            => Uri.EscapeDataString(value ?? string.Empty);
    }
}
