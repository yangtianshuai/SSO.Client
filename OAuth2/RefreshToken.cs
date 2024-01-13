using System;

namespace SSO.Client.OAuth2
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public int Expire { get; set; }
        public DateTime CreateTime { get; set; }

        public RefreshToken()
        {
            CreateTime = DateTime.Now;
        }
    }
}
