using System.Collections.Generic;

namespace SSO.Client
{
    public class SsoRequest
    {
        public SsoRequest()
        {
            OriginQuery = new Dictionary<string, List<string>>();
            Query = new Dictionary<string, List<string>>();
            Cookie = new Dictionary<string, string>();
            CallBack = new SsoCallback();
        }

        public string Ticket { get; set; }

        /// <summary>
        /// 原始Scheme
        /// </summary>
        public string OriginScheme { get; set; }
        /// <summary>
        /// 原始Request
        /// </summary>
        public string OriginHost { get; set; }
        /// <summary>
        /// 原始Path
        /// </summary>
        public string OriginPath { get; set; }
        public string Scheme { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string ClientIP { get; set; }

        public Dictionary<string, List<string>> OriginQuery { get; set; }

        public Dictionary<string, List<string>> Query { get; set; }
        public Dictionary<string, string> Cookie { get; set; }
        /// <summary>
        /// Body
        /// </summary>
        public byte[] Body { get; set; }

        public SsoCallback CallBack { get; set; }

        public string GetURL()
        {
            var url = Scheme + "://" + Host + ":" + Port + Path;
            return url;
        }
    }
}