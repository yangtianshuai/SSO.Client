using System;

namespace SSO.Client
{
    /// <summary>
    /// Cookie信息
    /// </summary>
    public class SsoCookie
    {
        public SsoCookie()
        {
            Time = DateTime.Now;
        }

        public static string Token = "SsoToken";
        public DateTime Time { get; set; }
        /// <summary>
        /// Cookie
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; internal set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmployeeId { get; internal set; }

        public string GetCookie()
        {
            return Token + "=" + ID;
        }
    }
}