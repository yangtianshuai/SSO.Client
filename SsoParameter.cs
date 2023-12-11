namespace SSO.Client
{
    public class SsoParameter
    {
        /// <summary>
        /// AppID参数名
        /// </summary>
        public static string AppID { get; internal set; } = "app_id";
        /// <summary>
        /// 票据参数名
        /// </summary>
        public static string TICKET { get; internal set; } = "ticket";
        /// <summary>
        /// 退出API
        /// </summary>
        public static string LogoutPath { get; internal set; } = "path";        
        /// <summary>
        /// AccessToken
        /// </summary>
        public static string AccessToken { get; internal set; } = "access_token";
        /// <summary>
        /// redirect_uri
        /// </summary>
        public static string RedirectUri { get; internal set; } = "redirect_uri";
        /// <summary>
        /// redirect
        /// </summary>
        public static string Redirect { get; internal set; } = "redirect";
    }
}