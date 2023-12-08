namespace SSO.Client.OAuth2
{
    public class OAuth2Api : SsoApi
    {
        /// <summary>
        /// 请求授权
        /// </summary>
        public static string AUTHORIZE = "oauth2/authorize";
        /// <summary>
        /// 获取授权码
        /// </summary>
        public static string TOKEN = "oauth2/token";
    }
}
