using Microsoft.Extensions.Configuration;
using Oauth_1a_Demo;
using OauthApiCaller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OauthApiCaller
{
    public class ApiCaller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ApiCaller(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private string DecryptSecret(string encryptedSecret)
        {
            var decryptionKey = _config["DecryptionKey"];
            if (string.IsNullOrEmpty(decryptionKey))
                throw new InvalidOperationException("Decryption key is missing from configuration.");

            return EncryptionHelper.Decrypt(encryptedSecret, decryptionKey);
        }

        private OAuthConfig GetOAuthConfig(string consumerKey, string accessToken)
        {
            var section = _config.GetSection(consumerKey);

            var encryptedConsumerSecret = section["ConsumerSecret"];
            var encryptedTokenSecret = section.GetSection("AccessTokenData")[accessToken];

            return new OAuthConfig
            {
                ConsumerKey = consumerKey,
                AccessToken = accessToken,
                ConsumerSecret = DecryptSecret(encryptedConsumerSecret),
                TokenSecret = DecryptSecret(encryptedTokenSecret)
            };
        }

        public async Task<string> SendAsync(RequestOptions options, string consumerKey, string accessToken)
        {
            var oauthConfig = GetOAuthConfig(consumerKey, accessToken);

            // Build Query String
            if (options.QueryParams?.Any() == true)
            {
                var query = string.Join("&",
                    options.QueryParams.Select(q =>
                        $"{Uri.EscapeDataString(q.Key)}={Uri.EscapeDataString(q.Value)}"));

                options.Url += "?" + query;
            }

            var request = new HttpRequestMessage(options.Method, options.Url);

            // Add OAuth Header
            var authHeader = OAuth1Helper.GenerateAuthorizationHeader(
                options.Url.Split('?')[0],
                options.Method,
                oauthConfig,
                options.QueryParams);

            request.Headers.Add("Authorization", authHeader);

            if (options.Headers != null)
            {
                foreach (var header in options.Headers)
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(options.Body))
            {
                request.Content = new StringContent(
                    options.Body,
                    Encoding.UTF8,
                    options.ContentType);
            }

            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

    }
}
