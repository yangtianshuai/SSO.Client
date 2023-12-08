using System.ComponentModel;

namespace SSO.Client.OAuth2
{
    public enum OAuth2Mode
    {
        [Description("简单模式")]
        Simple = 1,
        [Description("标准模式")]
        Standard = 2
    }
}
