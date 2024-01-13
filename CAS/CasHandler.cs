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
                string url = _options.GetBaseURL(_request.OriginHost);
                url += "/" + CasApi.LOGIN;
                url = $"{url}?{CasParameter.SERVICE}={HttpUtility.UrlEncode(_request.GetURL())}";
                if (!String.IsNullOrEmpty(_options.AppID))
                {
                    url = $"{url}&{CasParameter.AppID}={_options.AppID}";
                }
                if (_request.Query.ContainsKey(CasParameter.AccessToken))
                {
                    url = $"{url}&{CasParameter.AccessToken}={_request.Query[CasParameter.AccessToken][0]}";
                }

                _request.CallBack.Redirect?.Invoke(url, true, _options.Mode);
            }
            else if (!_request.Query.ContainsKey(CasParameter.TICKET))
            {               
                string url = _options.GetBaseURL(_request.OriginHost);
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
                _request.CallBack.Redirect?.Invoke(url, true, _options.Mode);
            }
            else
            {
                await ValidateSSOAsync(cache_flag);
            }
        }

    }
}