using System;

namespace SSO.Client
{
    public class SsoCallback
    {
        /// <summary>
        /// 跳转回调
        /// </summary>
        public Action<string,bool> Redirect { get; set; }        
        /// <summary>
        /// 验证回调
        /// </summary>
        public Action<SsoCookie> Validate { get; set; }
        /// <summary>
        /// 跳转
        /// </summary>
        public Action<SsoCookie> Logout { get; set; }
    }
}