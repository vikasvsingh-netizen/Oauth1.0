using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OauthApiCaller
{
    public class RequestOptions
    {
        public string Url { get; set; } = string.Empty;
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public Dictionary<string, string>? QueryParams { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? Body { get; set; }
        public string? ContentType { get; set; } = "application/json";
    }
}
