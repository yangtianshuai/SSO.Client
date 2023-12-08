namespace SSO.Client.OAuth2
{
    public class OAuth2Options
    {        
        public OAuth2Mode Mode { get; set; } = OAuth2Mode.Standard;
        /// <summary>
        /// 回调URL
        /// </summary>
        public string RedictUri { get; set; }

        internal string ResponseType()
        {
            return Mode == OAuth2Mode.Standard ? "code" : "token";
        }
    }
}
