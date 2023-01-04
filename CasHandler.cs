using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SSO.Client
{
    public class CasHandler : ICasHandler
    {
        private readonly CasOptions _options;
        private CasRequest _request;

        public CasHandler(CasOptions options)
        {
            _options = options;
        }

        public void SetRequest(CasRequest request)
        {
            this._request = request;
        }

        public bool Exist(string token)
        {
            return _options.Cookie.Contain(token);
        }

        public bool IsLogout(string path = null)
        {
            if (path == null)
            {
                path = _request.Path;
            }
            return path.ToLower() == _options.LogoutPath.ToLower();
        }

        public void Logout(string token = null)
        {
            CasCookie cookie = null;
            if (_request.Query.ContainsKey(CasParameter.TICKET))
            {
                token = _request.Query[CasParameter.TICKET];
                cookie = _options.Cookie.GetCookie(token);
            }
            else
            {
                cookie = _options.Cookie[token];
            }
            //子系统退出
            _request.CallBack.Logout?.Invoke(cookie);
            _options.Cookie.Remove(token);
            var temp = _options.GetBaseURL(_request.RequestHost);
            temp = _request.GetURL();
            _request.CallBack.Redirect($"{_options.GetBaseURL(_request.RequestHost)}/{CasAPI.LOGOUT}" +
                    $"?{CasParameter.SERVICE}={HttpUtility.UrlEncode(_request.GetURL())}");
        }
        public async Task ValidateAsync(bool cache_flag)
        {
            if (_request.Query.ContainsKey(CasParameter.AccessToken))
            {
                string url = _options.GetBaseURL(_request.RequestHost);
                url += "/" + CasAPI.LOGIN;
                url = $"{url}?{CasParameter.SERVICE}={HttpUtility.UrlEncode(_request.GetURL())}";
                if (!String.IsNullOrEmpty(_options.AppID))
                {
                    url = $"{url}&{CasParameter.AppID}={_options.AppID}";
                }
                if (_request.Query.ContainsKey(CasParameter.AccessToken))
                {
                    url = $"{url}&{CasParameter.AccessToken}={_request.Query[CasParameter.AccessToken]}";
                }
                using (var client = new HttpClient())
                {
                    //SSH权限
                    if (_request.Scheme.ToLower() == "https")
                    {
                        ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; };
                    }
                    var response = await client.GetAsync(url);

                    if (response != null)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<JObject>(body);
                        if (res["data"] != null)
                        {
                            var cookie = new CasCookie();
                            cookie.ID = res["data"]["ticket"].ToString();
                            cookie.UserID = res["data"]["id"].ToString();
                            //添加Cookie
                            _options.Cookie.Set(cookie.ID, cookie);
                            _request.CallBack.Validate(cookie);
                        }
                    }

                }
                _request.CallBack.Redirect(url);
            }
            else if (!_request.Query.ContainsKey(CasParameter.TICKET))
            {
                if (_request.CallBack.Redirect != null)
                {
                    string url = _options.GetBaseURL(_request.RequestHost);
                    if (_options.LoginURL != null && _options.LoginURL.Length > 0)
                    {
                        url = _options.LoginURL;
                    }
                    else
                    {
                        url += "/" + CasAPI.LOGIN;
                    }
                    url = $"{url}?{CasParameter.SERVICE}={HttpUtility.UrlEncode(_request.GetURL())}";
                    if (!String.IsNullOrEmpty(_options.AppID))
                    {
                        url = $"{url}&{CasParameter.AppID}={_options.AppID}";
                    }
                    _request.CallBack.Redirect(url);
                }
            }
            else
            {
                string ticket = _request.Query[CasParameter.TICKET];
                var service = HttpUtility.UrlEncode(_request.GetURL());
                string url = "";
                if (cache_flag && Exist(ticket))
                {
                    _request.CallBack.Validate(_options.Cookie.GetCookie(ticket));
                    Thread.Sleep(3000);
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        //SSH权限
                        if (_request.Scheme.ToLower() == "https")
                        {
                            ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; };
                        }
                        var logoutUrl = _request.RequestHost;
                        if (_options.LogoutPath.Length > 0 && _options.LogoutPath[0] != '/')
                        {
                            logoutUrl += '/';
                        }
                        logoutUrl += _options.LogoutPath;
                        var param = CasParameter.SERVICE + "=" + service
                            + "&" + CasParameter.TICKET + "=" + ticket + "&" + CasParameter.LOGOUT + "=" + HttpUtility.UrlEncode(logoutUrl);

                        url = $"{_options.GetBaseURL(_request.RequestHost, true)}/{CasAPI.VALIDATE}?" + param;
                        //可以通过Header设置Service的参数？
                        var response = await client.GetAsync(url);

                        if (response != null)
                        {
                            var body = await response.Content.ReadAsStringAsync();
                            if (_request.CallBack.Validate != null)
                            {
                                var res = JsonConvert.DeserializeObject<JObject>(body);
                                if (res["data"] != null)
                                {
                                    var cookie = new CasCookie();
                                    cookie.ID = ticket;
                                    cookie.UserID = res["data"].ToString();
                                    //添加Cookie
                                    _options.Cookie.Set(cookie.ID, cookie);
                                    _request.CallBack.Validate(cookie);
                                }
                            }
                        }

                    }
                }

                url = _request.GetURL();
                if (_request.Query.ContainsKey(CasParameter.TICKET))
                {
                    url = url.Replace(CasParameter.TICKET + "=" + _request.Query[CasParameter.TICKET], "").TrimEnd('&').TrimEnd('?');
                }
                _request.CallBack.Redirect(url);
            }
        }

        public void Validate(bool cache_flag)
        {
            Task.Run(async () =>
            {
                await this.ValidateAsync(cache_flag);
            }).GetAwaiter().GetResult();            
        }

        public CasOptions GetOptions()
        {
            return this._options;
        }
    }
}