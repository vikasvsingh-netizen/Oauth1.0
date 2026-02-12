using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OauthApiCaller.Model
{
    public class OAuthConfig
    {
        public string ConsumerKey { get; set; } = string.Empty;
        public string ConsumerSecret { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string TokenSecret { get; set; } = string.Empty;
    }

}
