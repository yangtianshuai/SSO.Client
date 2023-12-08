using System;
using System.Collections.Generic;
using System.Text;

namespace SSO.Client.OAuth2
{
    public class Oauth2Request
    {
        public string app_id { get; set; }
        /// <summary>
        /// 返回信息类型，支持code、token两种模式
        /// </summary>
        public string response_type { get; set; }
        /// <summary>
        /// 回调URL
        /// </summary>
        public string redirect_uri { get; set; }
        /// <summary>
        /// 请求标识
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 希望获取的资源
        /// </summary>
        public string scope { get; set; }
    }
}
