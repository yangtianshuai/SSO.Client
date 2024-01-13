using System.Collections.Concurrent;

namespace SSO.Client.OAuth2
{
    public class OAuth2Options
    {        
        public OAuth2Mode Mode { get; set; } = OAuth2Mode.Standard;
        /// <summary>
        /// 回调URL
        /// </summary>
        public string RedictUri { get; set; }

        private static ConcurrentDictionary<string, RefreshToken> refresh_tokens { get; set; }

        public OAuth2Options()
        {
            refresh_tokens = new ConcurrentDictionary<string, RefreshToken>();
        }

        internal void SetRefresh(string key, RefreshToken token = null)
        {
            if(!refresh_tokens.ContainsKey(key))
            {
                refresh_tokens.TryAdd(key, token);
            }
            else
            {                
                if(token != null)
                {
                    refresh_tokens.TryUpdate(key, token, refresh_tokens[key]);
                }
                else
                {
                    refresh_tokens.TryRemove(key, out token);
                }                
            }
        }

        internal RefreshToken GetRefresh(string key)
        {
            if (refresh_tokens.ContainsKey(key))
            {
                return refresh_tokens[key];
            }
            return null;
        }

        internal string ResponseType()
        {
            return Mode == OAuth2Mode.Standard ? "code" : "token";
        }
    }
}
