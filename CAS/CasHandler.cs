using System;
using System.Threading.Tasks;
using System.Web;

namespace SSO.Client.CAS
{
    public class CasHandler : SsoHandler
    {       
        public CasHandler(SsoOptions options)
        {
            _options = options;
        }
        
        public override async Task ValidateAsync(bool cache_flag)
        {
            if (_request.Query.ContainsKey(CasParameter.AccessToken))
            {
                string url = _options.GetBaseURL(_request.RequestHost);
                url += "/" + CasApi.LOGIN;
                url = $"{url}?{CasParameter.SERVICE}={HttpUtility.UrlEncode(_request.GetURL())}";
                if (!String.IsNullOrEmpty(_options.AppID))
                {
                    url = $"{url}&{CasParameter.AppID}={_options.AppID}";
                }
                if (_request.Query.ContainsKey(CasParameter.AccessToken))
                {
                    url = $"{url}&{CasParameter.AccessToken}={_request.Query[CasParameter.AccessToken]}";
                }

                _request.CallBack.Redirect?.Invoke(url);
            }
            else if (!_request.Query.ContainsKey(CasParameter.TICKET))
            {
                string url = _options.GetBaseURL(_request.RequestHost);
                if (_options.LoginURL != null && _options.LoginURL.Length > 0)
                {
                    url = _options.LoginURL;
                }
                else
                {
                    url += "/" + CasApi.LOGIN;
                }
                url = $"{url}?{CasParameter.SERVICE}={HttpUtility.UrlEncode(_request.GetURL())}";
                if (!string.IsNullOrEmpty(_options.AppID))
                {
                    url = $"{url}&{CasParameter.AppID}={_options.AppID}";
                }
                _request.CallBack.Redirect?.Invoke(url);
            }
            else
            {
                string ticket = _request.Query[CasParameter.TICKET];
                var service = HttpUtility.UrlEncode(_request.GetURL());
                string url = "";
                if (cache_flag && Exist(ticket))
                {
                    _request.CallBack.Validate?.Invoke(_options.Cookie.GetCookie(ticket));
                }
                else
                {
                    var logoutUrl = _request.RequestHost;
                    if (_options.LogoutPath.Length > 0 && _options.LogoutPath[0] != '/')
                    {
                        logoutUrl += '/';
                    }
                    logoutUrl += _options.LogoutPath;
                    var param = CasParameter.AppID + "=" + _options.AppID
                        + "&" + CasParameter.TICKET + "=" + ticket
                        + "&" + CasParameter.LogoutPath + "=" + HttpUtility.UrlEncode(logoutUrl);

                    url = $"{_options.GetBaseURL(_request.RequestHost, true)}/{CasApi.VALIDATE}?" + param;

                    await HttpRequestAsync(url, ticket);
                }

                url = _request.GetURL();
                if (_request.Query.ContainsKey(CasParameter.TICKET))
                {
                    url = url.Replace(CasParameter.TICKET + "=" + _request.Query[CasParameter.TICKET], "").TrimEnd('&').TrimEnd('?');
                }
                _request.CallBack.Redirect?.Invoke(url);
            }
        }

    }
}