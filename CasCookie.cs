using System;

namespace SSO.Client
{
    /// <summary>
    /// Cookie信息
    /// </summary>
    public class CasCookie
    {
        public static string Token = "CasToken";
        /// <summary>
        /// Cookie
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        public string GetCookie()
        {
            return Token + "=" + ID;
        }
    }
}