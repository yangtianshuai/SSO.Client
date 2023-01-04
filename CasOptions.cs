using System.Collections.Generic;

namespace SSO.Client
{
    /// <summary>
    /// Cas参数
    /// </summary>
    public class CasOptions
    {   
        /// <summary>
        /// Cas服务API根路径
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
        /// Cas模式
        /// </summary>
        public CasMode Mode { get; set; } = CasMode.Service;
        /// <summary>
        /// 是否强制使用HTTPS协议
        /// </summary>
        public bool ForceHTTPS { get; set; } = false;
        /// <summary>
        /// Cookie内部管理器
        /// </summary>
        internal CookieHost Cookie { get; set; } = new CookieHost();
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }
    }
}