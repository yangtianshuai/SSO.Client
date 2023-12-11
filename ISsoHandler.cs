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
        /// 是否是退出登录请求
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsLogout(string path = null);
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="redirect_flag">是否跳转</param>        
        void Logout(string token,bool redirect_flag);
        /// <summary>
        /// 是否已存在Cookie
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        bool Exist(string token);        
    }
}