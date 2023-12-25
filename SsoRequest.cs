using System.Collections.Generic;

namespace SSO.Client
{
    public class SsoRequest
    {
        public SsoRequest()
        {
            Query = new Dictionary<string, List<string>>();
            Cookie = new Dictionary<string, string>();
            CallBack = new SsoCallback();
        }
        public string Scheme { get; set; }
        /// <summary>
        /// 内部Request
        /// </summary>
        public string RequestHost { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string ClientIP { get; set; }

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