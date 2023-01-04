namespace SSO.Client
{
    public class CasParameter
    {
        /// <summary>
        /// Service参数名
        /// </summary>
        public static string SERVICE { get; internal set; } = "service";
        /// <summary>
        /// 票据参数名
        /// </summary>
        public static string TICKET { get; internal set; } = "ticket";
        /// <summary>
        /// 退出API
        /// </summary>
        public static string LOGOUT { get; internal set; } = "path";
        /// <summary>
        /// AppID参数名
        /// </summary>
        public static string AppID { get; internal set; } = "app_id";
        /// <summary>
        /// 参数名AccessToken
        /// </summary>
        public static string AccessToken { get; internal set; } = "access_token";
    }
}