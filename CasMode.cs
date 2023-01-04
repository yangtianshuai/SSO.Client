using System.ComponentModel;

namespace SSO.Client
{
    public enum CasMode
    {
        [Description("服务模式")]
        Service = 1,
        [Description("代理模式")]
        Proxy = 2
    }
}