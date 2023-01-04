using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SSO.Client
{
    internal class CookieHost
    {
        private static ConcurrentDictionary<string, CasCookie> _cookies { get; set; }

        public CookieHost()
        {
            _cookies = new ConcurrentDictionary<string, CasCookie>();
        }
      
        public CasCookie this[string st]
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

        public CasCookie GetCookie(string ticket)
        {
            var list = new List<string>(_cookies.Keys);
            foreach (var item in list)
            {
                var cookie = _cookies[item];
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
                CasCookie value = null;
                _cookies.TryRemove(st,out value);
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="st"></param>
        /// <param name="user"></param>
        public void Set(string st, CasCookie cookie)
        {
            if (!Contain(st))
            {
                _cookies.TryAdd(st, cookie);
            }
        }
    }
}