using System;

namespace SSO.Client
{
    /// <summary>
    /// Cookie信息
    /// </summary>
    public class CasCookie
    {
        public CasCookie()
        {
            Time = DateTime.Now;
        }

        public static string Token = "CasToken";
        public DateTime Time { get; set; }
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