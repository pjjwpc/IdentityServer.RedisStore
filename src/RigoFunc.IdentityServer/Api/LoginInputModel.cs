namespace RigoFunc.IdentityServer.Api {
    public class LoginInputModel {
        /// <summary>
        /// 用户名.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 是否记住登陆.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
