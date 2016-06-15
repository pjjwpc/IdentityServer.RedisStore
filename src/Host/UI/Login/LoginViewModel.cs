namespace Host.UI.Login {
    public class LoginViewModel : LoginInputModel {
        public LoginViewModel() {
        }

        public LoginViewModel(LoginInputModel other) {
            UserName = other.UserName;
            Password = other.Password;
            RememberMe = other.RememberMe;
            SignInId = other.SignInId;
        }

        public string ErrorMessage { get; set; }
    }
}