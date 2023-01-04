using System.Collections.Generic;

namespace SSO.Client
{
    public class CasRequest
    {
        public CasRequest()
        {
            Query = new Dictionary<string, string>();
            Cookie = new Dictionary<string, string>();
            CallBack = new CasCallback();
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

        public Dictionary<string, string> Query { get; set; }
        public Dictionary<string, string> Cookie { get; set; }
        /// <summary>
        /// Body
        /// </summary>
        public byte[] Body { get; set; }

        public CasCallback CallBack { get; set; }

        public string GetURL()
        {
            var url = Scheme + "://" + Host + ":" + Port + Path;
            return url;
        }
    }
}