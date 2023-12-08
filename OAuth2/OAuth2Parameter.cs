namespace SSO.Client.OAuth2
{
    internal class OAuth2Parameter : SsoParameter
    {
        public static string ResponseType { get; internal set; } = "response_type";
        public static string State { get; internal set; } = "state";
        public static string Scope { get; internal set; } = "scope";

        public static string Code { get; internal set; } = "code";
        public static string Secret { get; internal set; } = "secret";
        //authorization_code、password、refresh_token
        public static string GrantType { get; internal set; } = "grant_type";
        
    }
}
