using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SSO.Client
{
    public abstract class SsoHandler : ISsoHandler
    {
        protected SsoRequest _request;
        protected SsoOptions _options;        

        public void SetRequest(SsoRequest request)
        {
            this._request = request;
        }

        public SsoOptions GetOptions()
        {
            return this._options;
        }

        public bool IsLogout(string path = null)
        {
            if (path == null)
            {
                path = _request.Path;
            }
            return path.ToLower() == _options.LogoutPath.ToLower();
        }

        public void Logout(string token,bool redirect_flag)
        {
            SsoCookie cookie = null;
            if (string.IsNullOrEmpty(token) && _request.Query.ContainsKey(SsoParameter.TICKET))
            {
                token = _request.Query[SsoParameter.TICKET][0];
                cookie = _options.Cookie.GetCookie(token);
            }
            else
            {
                cookie = _options.Cookie[token];
            }

            _options.Cookie.Remove(token);
            //子系统退出
            _request.CallBack.Logout?.Invoke(cookie);            

            //认证服务通知时，不需要跳转
            if (!redirect_flag)
            {
                return;
            }

            var url = $"{_options.GetBaseURL(_request.OriginHost)}/{SsoApi.LOGOUT}" +
                 $"?{SsoParameter.AppID}={_options.AppID}" +
                 $"&{SsoParameter.TICKET}={token}" +
                 $"&{SsoParameter.RedirectUri}={HttpUtility.UrlEncode(_request.GetURL())}";
            _request.CallBack.Redirect?.Invoke(url, false, _options.Mode);
        }

        public void Validate(bool cache_flag)
        {
            this.ValidateAsync(cache_flag).GetAwaiter().GetResult();
        }

        protected async Task<bool> HttpRequestAsync(string url, string ticket = null)
        {
            try
            {
                var uri = new Uri(url);
            }
            catch (Exception ex)
            {
                throw new Exception("无效URL");
            }

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
                        var cookie = new SsoCookie();
                        if (!string.IsNullOrEmpty(ticket))
                        {
                            cookie.ID = ticket;                            
                        }
                        else
                        {
                            cookie.ID = res["ticket"]?.ToString();
                        }
                        cookie.UserID = res["id"]?.ToString();
                        cookie.Name = res["name"]?.ToString();
                        cookie.EmployeeId = res["employee_id"]?.ToString();

                        //添加Cookie
                        _options.Cookie.Set(cookie.ID, cookie);
                        _request.CallBack.Validate?.Invoke(cookie);
                        return true;
                    }
                }
            }
            return false;
        }

        protected async Task ValidateSSOAsync(bool cache_flag)
        {
            string ticket = _request.Ticket;
            if (_request.Query.ContainsKey(SsoParameter.TICKET))
            {
                ticket = _request.Query[SsoParameter.TICKET][0];
            }
            
            var service = HttpUtility.UrlEncode(_request.GetURL());
            string url = "";
            if (cache_flag && Exist(ticket))
            {
                _request.CallBack.Validate?.Invoke(_options.Cookie.GetCookie(ticket));
            }
            else
            {
                var logoutUrl = _request.OriginHost;
                if (_options.LogoutPath.Length > 0 && _options.LogoutPath[0] != '/')
                {
                    logoutUrl += '/';
                }
                logoutUrl += _options.LogoutPath;
                var param = SsoParameter.AppID + "=" + _options.AppID
                    + "&" + SsoParameter.TICKET + "=" + ticket
                    + "&" + SsoParameter.LogoutPath + "=" + HttpUtility.UrlEncode(logoutUrl);

                url = $"{_options.GetBaseURL(_request.OriginHost, true)}/{SsoApi.VALIDATE}?" + param;

                if (!await HttpRequestAsync(url, ticket))
                {

                }
            }

            url = _request.GetURL();
            if (_request.Query.ContainsKey(SsoParameter.TICKET))
            {
                var param = SsoParameter.TICKET + "=" + _request.Query[SsoParameter.TICKET][0];
                url = url.Replace(param, "");
                url = url.TrimEnd('&').TrimEnd('?');
            }
            _request.CallBack.Redirect?.Invoke(url, false, _options.Mode);
        }

        public abstract Task ValidateAsync(bool cache_flag);

        public bool Exist(string token)
        {
            if (string.IsNullOrEmpty(token) && _request.Query.ContainsKey(SsoParameter.TICKET))
            {
                token = _request.Query[SsoParameter.TICKET][0];
            }
            //需要定时检测token有效性
            return _options.Cookie.Contain(token);
        }
    }
}