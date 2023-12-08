using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SSO.Client
{
    internal class CookieHost
    {
        private static ConcurrentDictionary<string, SsoCookie> _cookies { get; set; }
        public int expires { get; set; } = 60 * 30;

        public CookieHost()
        {
            _cookies = new ConcurrentDictionary<string, SsoCookie>();
        }

        public CookieHost(int expires):this()
        {
            this.expires = expires;
        }

        public SsoCookie this[string st]
        {
            get
            {
                if (Contain(st))
                {
                    return _cookies[st];
                }
                return null;
            }
        }

        public SsoCookie GetCookie(string ticket)
        {
            var keys = new List<string>(_cookies.Keys);
            foreach (var key in keys)
            {
                var cookie = _cookies[key];
                if (cookie != null)
                {                    
                    if (cookie.ID == ticket)
                    {                        
                        return cookie;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 是否存在Cookie
        /// </summary>
        /// <param name="st">临时票据</param>
        /// <returns></returns>
        public bool Contain(string st)
        {
            if (st == null)
            {
                return false;
            }
            if (_cookies.ContainsKey(st))
            {
                var cookie = _cookies[st];
                if (cookie.Time.AddSeconds(expires) < DateTime.Now)
                {
                    _cookies.TryRemove(st, out cookie);
                    return false;
                }
            }
            return _cookies.ContainsKey(st);
        }
        /// <summary>
        /// 移除Cookie
        /// </summary>
        /// <param name="st"></param>
        public void Remove(string st)
        {
            if (Contain(st))
            {
                SsoCookie value = null;
                _cookies.TryRemove(st,out value);
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="st"></param>
        /// <param name="user"></param>
        public void Set(string st, SsoCookie cookie)
        {
            if (!Contain(st))
            {
                _cookies.TryAdd(st, cookie);
            }
        }


    }
}