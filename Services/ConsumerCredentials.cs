
namespace Oauth_1a_Demo.Services
{
    public class ConsumerCredentials
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public Dictionary<string, string> AccessTokens { get; set; }
    }
}