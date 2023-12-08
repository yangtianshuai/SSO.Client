namespace SSO.Client
{
    public class SsoApi
    {
        /// <summary>
        /// 退出登录
        /// </summary>
        public static string LOGOUT = "sso/logout";
        /// <summary>
        /// 验证票据/令牌，成功返回用户信息
        /// </summary>
        public static string VALIDATE = "sso/validate";
    }
}
