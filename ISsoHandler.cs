namespace SSO.Client
{
    public interface ISsoHandler
    {
        /// <summary>
        /// 获取选项
        /// </summary>
        /// <returns></returns>
        SsoOptions GetOptions();

        void SetRequest(SsoRequest request);
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="cache_flag">缓存过滤</param>       
        void Validate(bool cache_flag = false);
        /// <summary>
        /// 是否为退出请求
        /// </summary>        
        /// <returns></returns>
        bool IsLogout(string path = null);
        /// <summary>
        /// 退出登录
        /// </summary>        
        void Logout(string token = null);
        /// <summary>
        /// 是否已存在Cookie
        /// </summary>
        /// <returns></returns>
        bool Exist(string token);        
    }
}