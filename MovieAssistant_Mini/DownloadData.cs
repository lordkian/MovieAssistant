using System;
using System.Collections.Specialized;
using System.Net;
namespace MovieAssistant_Mini
{
    internal class DownloadData
    {
        public Uri Uri { get; set; }
        public string Path { get; set; }
        public NameValueCollection PostData { get; set; }
        public CookieCollection PostCookies { get; set; }
    }
}
