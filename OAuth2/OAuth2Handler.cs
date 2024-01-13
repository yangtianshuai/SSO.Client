using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SSO.Client.OAuth2
{
    public class OAuth2Handler : SsoHandler
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

            if (_request.OriginPath.ToLower() == _options2.RedictUri.ToLower())
            {
                //验证state

                string ticket = string.Empty;

                //回调地址
                if (_options2.Mode == OAuth2Mode.Simple)
                {
                    //简单模式，返回：access_token、state、expires_in、scope
                    ticket = _request.OriginQuery[OAuth2Parameter.AccessToken][0];
                    if (_request.OriginQuery.ContainsKey(OAuth2Parameter.Expire))
                    {
                        _options2.SetRefresh(ticket, new RefreshToken
                        {
                            Expire = int.Parse(_request.OriginQuery[OAuth2Parameter.Expire][0])
                        });
                    }                    
                }
                else
                {
                    //标准模式，返回：code、state、scope
                    //发送：app_id、secret、grant_type、code
                    ticket = await AccessTokenAsync();
                }
                //跳转原页面 
                url = _request.OriginQuery[OAuth2Parameter.Scope][0];
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                else
                {
                    url += "&";
                }
                url += $"{SsoParameter.TICKET}={ticket}";
                _request.CallBack.Redirect?.Invoke(url, false, SsoMode.Service);
            }
            else
            {
                if (!string.IsNullOrEmpty(_request.Ticket))
                {
                    //是否需要刷新令牌
                    var refresh_token = _options2.GetRefresh(_request.Ticket);
                    if(refresh_token != null)
                    {
                        var time_span = DateTime.Now - refresh_token.CreateTime;
                        if (time_span.TotalSeconds >= (refresh_token.Expire - 60 * 5))
                        {
                            //需要刷新令牌
                            await RefreshAccessTokenAsync(_request.Ticket, refresh_token.Token);
                        }
                    }
                }
                if (_request.Query.ContainsKey(SsoParameter.TICKET) || !string.IsNullOrEmpty(_request.Ticket))
                {                    
                    await ValidateSSOAsync(cache_flag);
                    return;
                }

                url = _options.GetBaseURL(_request.OriginHost);
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
                        + "&" + OAuth2Parameter.RedirectUri + "=" + HttpUtility.UrlEncode(_request.OriginHost + _options2.RedictUri)
                        + "&" + OAuth2Parameter.State + "=" + state
                        + "&" + OAuth2Parameter.Scope + "=" + HttpUtility.UrlEncode(_request.GetURL());

                url = $"{url}?{param}";

                _request.CallBack.Redirect?.Invoke(url, true, _options.Mode);

            }            
        }

        private async Task<string> AccessTokenAsync()
        {
            string code = "";
            if (_request.OriginQuery.ContainsKey(OAuth2Parameter.Code))
            {
                code = _request.OriginQuery[OAuth2Parameter.Code][0];
            }
            var grant_type = "authorization_code";
            var param = OAuth2Parameter.AppID + "=" + _options.AppID
                    + "&" + OAuth2Parameter.Secret + "=" + _options.Secret
                    + "&" + OAuth2Parameter.Secret + "=" + _options.Secret
                    + "&" + OAuth2Parameter.GrantType + "=" + grant_type
                    + "&" + OAuth2Parameter.Code + "=" + code;


            var url = $"{_options.GetBaseURL(_request.OriginHost, true)}/{OAuth2Api.TOKEN}?" + param;

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
                        string access_token = res["access_token"]?.ToString();
                        _options2.SetRefresh(access_token, new RefreshToken
                        {
                            Token = res["refresh_token"]?.ToString(),
                            Expire = int.Parse(res["expires_in"]?.ToString())
                        });
                        return access_token;
                    }
                }
            }
            return string.Empty;
        }

        private async Task<string> RefreshAccessTokenAsync(string access_token,string refresh_token)
        {                    
            var grant_type = "refresh_token";
            var param = OAuth2Parameter.RefreshToken + "=" + refresh_token
                    + "&" + OAuth2Parameter.GrantType + "=" + grant_type;


            var url = $"{_options.GetBaseURL(_request.OriginHost, true)}/{OAuth2Api.TOKEN}?" + param;

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
                        access_token = res["access_token"]?.ToString();                                 
                        _options2.SetRefresh(access_token, new RefreshToken
                        {
                            Token = res["refresh_token"]?.ToString(),
                            Expire = int.Parse(res["expires_in"]?.ToString())
                        });
                        return access_token;                        
                    }
                    else
                    {
                        _options2.SetRefresh(access_token, null);
                    }
                }
            }
            return string.Empty;
        }

    }
}