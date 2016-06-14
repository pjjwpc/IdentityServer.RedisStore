namespace Host.UI.OAuth {
    public class ChangePasswordModel {
        /// <summary>
        /// 用户名.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 原密码.
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
