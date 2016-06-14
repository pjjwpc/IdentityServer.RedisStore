namespace Host.UI.OAuth {
    public class VerifyCodeInputModel {
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
