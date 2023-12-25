using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SSO.Client.OAuth2
{
    public abstract class OAuth2Handler : SsoHandler
    {
        protected OAuth2Options _options2;

        public OAuth2Handler(SsoOptions options, OAuth2Options options2)
        {
            _options = options;
            _options2 = options2;
        }
        
        public override async Task ValidateAsync(bool cache_flag)
        {
            string url = string.Empty;
            if (_request.Path.ToLower() == _options2.RedictUri.ToLower())
            {
                //验证state

                string ticket = string.Empty;
                
                //回调地址
                if (_options2.Mode == OAuth2Mode.Simple)
                {
                    //简单模式，返回：access_token、state、expires_in、scope
                    ticket = _request.Query[OAuth2Parameter.AccessToken][0];
                }
                else
                {
                    //标准模式，返回：code、state、scope
                    //发送：app_id、secret、grant_type、code
                    ticket =  await AccessTokenAsync();
                }
                         
                if (cache_flag && Exist(ticket))
                {
                    _request.CallBack.Validate?.Invoke(_options.Cookie.GetCookie(ticket));
                }
                else
                {
                    var logoutUrl = _request.RequestHost;
                    if (_options.LogoutPath[0] != '/')
                    {
                        logoutUrl += '/';
                    }
                    logoutUrl += _options.LogoutPath;

                    var param = OAuth2Parameter.AppID + "=" + _options.AppID
                        + "&" + OAuth2Parameter.TICKET + "=" + ticket
                        + "&" + OAuth2Parameter.LogoutPath + "=" + HttpUtility.UrlEncode(logoutUrl);

                    url = $"{_options.GetBaseURL(_request.RequestHost, true)}/{OAuth2Api.VALIDATE}?" + param;

                    await HttpRequestAsync(url, ticket);
                }

                //跳转原页面 
                url = _request.Query[OAuth2Parameter.Scope][0];
                _request.CallBack.Redirect?.Invoke(url, false);
            }
            else
            {
                url = _options.GetBaseURL(_request.RequestHost);
                if (_options.LoginURL != null && _options.LoginURL.Length > 0)
                {
                    url = _options.LoginURL;
                }
                else
                {
                    url += "/" + OAuth2Api.AUTHORIZE;
                }

                var state = Guid.NewGuid().ToString("N");

                var param = OAuth2Parameter.AppID + "=" + _options.AppID
                        + "&" + OAuth2Parameter.ResponseType + "=" + _options2.ResponseType()
                        + "&" + OAuth2Parameter.RedirectUri + "=" + _options2.RedictUri
                        + "&" + OAuth2Parameter.State + "=" + state
                        + "&" + OAuth2Parameter.Scope + "=" + HttpUtility.UrlEncode(_request.GetURL());

                url = $"{url}?{param}";

                _request.CallBack.Redirect?.Invoke(url, true);

            }            
        }

        private async Task<string> AccessTokenAsync()
        {
            var code = _request.Query[OAuth2Parameter.Code];
            var grant_type = "authorization_code";
            var param = OAuth2Parameter.AppID + "=" + _options.AppID
                    + "&" + OAuth2Parameter.Secret + "=" + _options.Secret
                    + "&" + OAuth2Parameter.Secret + "=" + _options.Secret
                    + "&" + OAuth2Parameter.GrantType + "=" + grant_type
                    + "&" + OAuth2Parameter.Code + "=" + code;


            var url = $"{_options.GetBaseURL(_request.RequestHost, true)}/{OAuth2Api.TOKEN}?" + param;

            using (var client = new HttpClient())
            {
                //SSH权限
                if (_request.Scheme.ToLower() == "https")
                {
                    ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; };
                }

                //可以通过Header设置Service的参数？
                var response = await client.GetAsync(url);

                if (response != null)
                {
                    var body = await response.Content.ReadAsStringAsync();

                    var res = JsonConvert.DeserializeObject<JObject>(body);
                    if (res["code"]?.ToString() == "1")
                    {                        
                        //expires_in = temp_ticket.Expire,
                        //refresh_token                        
                        return res["access_token"]?.ToString();                        
                    }
                }
            }
            return string.Empty;
        }

    }
}