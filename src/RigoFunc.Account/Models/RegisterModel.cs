namespace RigoFunc.Account.Models {
    public class RegisterModel {
        /// <summary>
        /// 用户名.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 电话号码.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 验证码.
        /// </summary>
        public string Code { get; set; }
    }
}
