using System.Collections.Generic;

namespace SSO.Client
{
    /// <summary>
    /// SSO参数
    /// </summary>
    public class SsoOptions
    {   
        /// <summary>
        /// 服务API根路径
        /// </summary>
        public string BaseURL { get; set; }
        public string GetBaseURL(string host, bool validate = false)
        {
            if (IPMappings != null && IPMappings.Count > 0)
            {
                var ip_mapping = IPMappings.Find(t => host.Contains(t.server_ip));
                if (ip_mapping != null)
                {
                    if (validate && !string.IsNullOrEmpty(ip_mapping.validate_url))
                    {
                        return ip_mapping.validate_url;
                    }
                    return ip_mapping.base_url;
                }
            }
            return BaseURL;
        }
        public List<IPMapping> IPMappings { get; set; }
        /// <summary>
        /// 登录页面
        /// </summary>
        public string LoginURL { get; set; }
        /// <summary>
        /// 退出路径
        /// </summary>
        public string LogoutPath { get; set; }
        /// <summary>
        /// 模式
        /// </summary>
        public SsoMode Mode { get; set; } = SsoMode.Service;
        /// <summary>
        /// 是否强制使用HTTPS协议
        /// </summary>
        public bool ForceHTTPS { get; set; } = false;
        /// <summary>
        /// Cookie内部管理器
        /// </summary>
        internal CookieHost Cookie { get; set; } = new CookieHost();
        /// <summary>
        /// 票据有效期
        /// </summary>
        public int Expires
        {
            get
            {
                return Cookie.expires;
            }
            set
            {                
                Cookie.expires = value;
            }
        }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// 应用密钥
        /// </summary>
        public string Secret { get; set; }

    }
}