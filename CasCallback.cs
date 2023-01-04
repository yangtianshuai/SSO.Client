using System;

namespace SSO.Client
{
    public class CasCallback
    {
        /// <summary>
        /// 跳转回调
        /// </summary>
        public Action<string> Redirect { get; set; }
        /// <summary>
        /// 验证回调
        /// </summary>
        public Action<CasCookie> Validate { get; set; }
        /// <summary>
        /// 跳转
        /// </summary>
        public Action<CasCookie> Logout { get; set; }
    }
}