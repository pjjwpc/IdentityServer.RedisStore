using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RigoFunc.ApiCore.Services;
using RigoFunc.Account.Models;
using RigoFunc.OAuth;
using RigoFunc.Utils;

namespace RigoFunc.Account.Services {
    /// <summary>
    /// Represents the default implementation of the <see cref="IAccountService"/> interface.
    /// </summary>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    public class DefaultAccountService<TUser> : IAccountService where TUser: class, new() {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly HttpContext _httpContext;
        private readonly ApiOptions _options;
        private const string DefaultSecurityStamp = "022a9e42-9509-4aa6-8a0a-c34a1f405c61";
        private const string WChatLoginProvider = "wxLoginProvider";

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAccountService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="smsSender">The SMS sender.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="options">The options.</param>
        public DefaultAccountService(UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor contextAccessor,
            IOptions<ApiOptions> options) {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger("AccountService"); ;
            _httpContext = contextAccessor.HttpContext;
        }

        /// <summary>
        /// Gets OAuth user by the specified user Id or Phone number asynchronous.
        /// </summary>
        /// <param name="model">The model contains the user Id or Phone number.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the get operation. Task result contains the found user.</returns>
        public async Task<OAuthUser> GetAsync(FindUserModel model) {
            TUser user;
            if (model.Id != null) {
                user = await _userManager.FindByIdAsync(model.Id.ToString());
            }
            else {
                user = await _userManager.FindByNameAsync(model.PhoneNumber);
            }

            if (user == null) {
                throw new Exception("cannot find the user by id or phone number");
            }

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

            if (_userManager.SupportsUserEmail) {
                var email = await _userManager.GetEmailAsync(user);
                if (!string.IsNullOrWhiteSpace(email)) {
                    userPrincipal.Identities.First().AddClaims(new[]
                    {
                        new Claim(JwtClaimTypes.Email, email),
                        new Claim(JwtClaimTypes.EmailVerified,
                            await _userManager.IsEmailConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            if (_userManager.SupportsUserPhoneNumber) {
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (!string.IsNullOrWhiteSpace(phoneNumber)) {
                    userPrincipal.Identities.First().AddClaims(new[]
                    {
                        new Claim(JwtClaimTypes.PhoneNumber, phoneNumber),
                        new Claim(JwtClaimTypes.PhoneNumberVerified,
                            await _userManager.IsPhoneNumberConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            return OAuthUser.FromUser(userPrincipal);
        }

        /// <summary>
        /// Registers a new user asynchronous.
        /// </summary>
        /// <param name="model">The register model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the register operation. Task result contains the register response.</returns>
        public async Task<IResponse> RegisterAsync(RegisterModel model) {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user != null) {
                throw new ArgumentException(string.Format(Resources.PhoneNumberHadBeenRegister, model.PhoneNumber));
            }

            if (!ValidateCode(model.Code, model.PhoneNumber)) {
                throw new ArgumentException(string.Format(Resources.VerifyCodeFailed, model.Code));
            }

            user = new TUser();
            
            // set user name
            var result = await _userManager.SetUserNameAsync(user, model.PhoneNumber);
            if (!result.Succeeded) {
                HandleErrors(result, string.Format(Resources.RegisterNewUserFailed, model.PhoneNumber, model.Code));
            }

            // set phone number.
            if (_userManager.SupportsUserPhoneNumber) {
                result = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!result.Succeeded) {
                    HandleErrors(result, string.Format(Resources.RegisterNewUserFailed, model.PhoneNumber, model.Code));
                }
            }

            result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) {
                HandleErrors(result, string.Format(Resources.RegisterNewUserFailed, model.PhoneNumber, model.Code));
            }

            // sign in
            await _signInManager.SignInAsync(user, isPersistent: false);

            return await RequestTokenAsync(model.PhoneNumber, model.Password);
        }

        /// <summary>
        /// Logins with the specified model asynchronous.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the login operation.</returns>
        public async Task<IResponse> LoginAsync(LoginModel model) {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded) {
                _logger.LogInformation(1, "User logged in.");
                return await RequestTokenAsync(model.UserName, model.Password);
            }

            _logger.LogError(result.ToString());

            throw new ArgumentException(Resources.PhoneNumberOrPasswordError);
        }

        /// <summary>
        /// Sends the specified code asynchronous.
        /// </summary>
        /// <param name="model">The send code model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the send operation.</returns>
        public async Task<bool> SendCodeAsync(SendCodeModel model) {
            string code;
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) {
                code = GenerateCode(model.PhoneNumber);
            }
            else {
                code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            }

            SendSmsResult result;
            if (string.IsNullOrWhiteSpace(_options.CodeSmsTemplate)) {
                result = await _smsSender.SendSmsAsync(model.PhoneNumber, code);
            }
            else {
                result = await _smsSender.SendSmsAsync(_options.CodeSmsTemplate, model.PhoneNumber, Tuple.Create("code", code));
            }

            if (!result.IsSuccessSend) {
                _logger.LogError(string.Format(Resources.SendCodeFailed, model.PhoneNumber, result.ErrorMessage));

                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the specified code asynchronous.
        /// </summary>
        /// <param name="model">The verify code model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the verify operation.</returns>
        public async Task<IResponse> VerifyCodeAsync(VerifyCodeModel model) {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) {
                if (!ValidateCode(model.Code, model.PhoneNumber)) {
                    throw new ArgumentException(string.Format(Resources.VerifyCodeFailed, model.Code));
                }

                user = new TUser();

                // set user name
                var result = await _userManager.SetUserNameAsync(user, model.PhoneNumber);
                if (!result.Succeeded) {
                    HandleErrors(result, string.Format(Resources.RegisterNewUserFailed, model.PhoneNumber, model.Code));
                }

                // set phone number.
                if (_userManager.SupportsUserPhoneNumber) {
                    result = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                    if (!result.Succeeded) {
                        HandleErrors(result, string.Format(Resources.RegisterNewUserFailed, model.PhoneNumber, model.Code));
                    }
                }

                var password = $"{GenericUtil.UniqueKey()}@520";
                result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded) {
                    HandleErrors(result, string.Format(Resources.RegisterNewUserFailed, model.PhoneNumber, model.Code));
                }

                SendSmsResult smsResult;
                if (string.IsNullOrWhiteSpace(_options.PasswordSmsTemplate)) {
                    smsResult = await _smsSender.SendSmsAsync(model.PhoneNumber, password);
                }
                else {
                    smsResult = await _smsSender.SendSmsAsync(_options.PasswordSmsTemplate, model.PhoneNumber, Tuple.Create("password", password));
                }
                if (!smsResult.IsSuccessSend) {
                    _logger.LogError(string.Format(Resources.SendPasswordFailed, model.PhoneNumber, smsResult.ErrorMessage));
                }

                // sign in
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await RequestTokenAsync(model.PhoneNumber, password);
            }
            else {
                if (!await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.PhoneNumber)) {
                    throw new ArgumentException(string.Format(Resources.VerifyCodeFailed, model.Code));
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                var token = await RequestTokenAsync(model.PhoneNumber, model.Code);
                if (token.IsError) {
                    _logger.LogError(token.ToString());

                    throw new ArgumentNullException(string.Format(Resources.LoginFailedWithCodeAndPhone, model.Code, model.PhoneNumber));
                }

                return token;
            }
        }

        /// <summary>
        /// Changes the password for the specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the change operation.</returns>
        public async Task<bool> ChangePasswordAsync(ChangePasswordModel model) {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) {
                throw new ArgumentNullException(string.Format(Resources.NotFoundUserByPhoneNumber, model.UserName));
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded) {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return true;
            }

            HandleErrors(result);

            return false;
        }

        /// <summary>
        /// Resets the password for specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the reset operation.</returns>
        public async Task<IResponse> ResetPasswordAsync(ResetPasswordModel model) {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) {
                throw new ArgumentNullException(string.Format(Resources.NotFoundUserByPhoneNumber, model.PhoneNumber));
            }

            if (!await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.PhoneNumber)) {
                throw new ArgumentException(string.Format(Resources.VerifyCodeFailed, model.Code));
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if (!result.Succeeded) {
                HandleErrors(result, string.Format(Resources.PasswordResetFailed, model.PhoneNumber));
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return await RequestTokenAsync(model.PhoneNumber, model.Password);
        }

        /// <summary>
        /// Updates the specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the reset operation.</returns>
        public async Task<bool> UpdateAsync(OAuthUser model) {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null) {
                throw new ArgumentException(string.Format(Resources.NotFoundUserById, model.Id));
            }

            var result = await _userManager.AddClaimsAsync(user, model.ToClaims());
            if (!result.Succeeded) {
                HandleErrors(result, Resources.UpdateUserFailed);
            }

            return true;
        }

        /// <summary>
        /// Binds the open ID for the specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the bind operation.</returns>
        public async Task<bool> BindAsync(OpenIdBindingModel model) {
            var user = await _userManager.FindByLoginAsync(WChatLoginProvider, model.OpenId);
            if (user != null) {
                _logger.LogWarning($"Phone Number {model.PhoneNumber} had bind open id {model.OpenId}");

                // find by phone number
                var byPhone = await _userManager.FindByNameAsync(model.PhoneNumber);

                var userId1 = await _userManager.GetUserIdAsync(user);
                var userId2 = await _userManager.GetUserIdAsync(byPhone);
                if (userId1 == userId2) {
                    return true;
                }
            }

            user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) {
                throw new ArgumentException(string.Format(Resources.NotFoundUserByPhoneNumber, model.PhoneNumber));
            }
            var info = new UserLoginInfo(WChatLoginProvider, model.OpenId, "weixin");
            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded) {
                _logger.LogError(result.ToString());
                return false;
            }

            // add a claim for this user.
            result = await _userManager.AddClaimAsync(user, new Claim(OAuthClaimTypes.WChat, model.OpenId));
            if (!result.Succeeded) {
                _logger.LogError($"User: {model.PhoneNumber} add claim error: {result.ToString()}");
            }

            return true;
        }

        /// <summary>
        /// Logins with the specified model asynchronous.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the login operation.</returns>
        public async Task<IResponse> LoginAsync(OpenIdLoginModel model) {
            var user = await _userManager.FindByLoginAsync(WChatLoginProvider, model.OpenId);
            if (user == null) {
                throw new ArgumentException("cannot login with weixin open id.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            var response = await RequestTokenAsync(userName, code);
            if (response.IsError) {
                _logger.LogError(response.ToString());

                throw new ArgumentNullException(response.Error);
            }

            return response;
        }

        private async Task<IResponse> RequestTokenAsync(string userName, string password) {
            var endpoint = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host.Value}/connect/token";

            _logger.LogInformation($"token_endpoint: {endpoint}");

            string clientId = _httpContext.Request.Headers[ApiConstants.ClientId];
            if (string.IsNullOrWhiteSpace(clientId)) {
                clientId = _options.DefaultClientId;
            }
            string clientSecret = _httpContext.Request.Headers[ApiConstants.ClientSecret];
            if (string.IsNullOrWhiteSpace(clientSecret)) {
                clientSecret = _options.DefaultClientSecret;
            }
            string scope = _httpContext.Request.Headers[ApiConstants.Scope];
            if (string.IsNullOrWhiteSpace(scope)) {
                scope = _options.DefaultScope;
            }

            var client = new TokenClient(endpoint, clientId, clientSecret);
            var response = await client.RequestResourceOwnerPasswordAsync(userName, password, scope);

            return ApiResponse.FromTokenResponse(response);
        }

        private string GenerateCode(string phoneNumber) {
            var securityStamp = Encoding.Unicode.GetBytes(DefaultSecurityStamp);
            return Rfc6238Service.GenerateCode(securityStamp, phoneNumber).ToString(CultureInfo.InvariantCulture);
        }

        private bool ValidateCode(string token, string phoneNumber) {
            var securityStamp = Encoding.Unicode.GetBytes(DefaultSecurityStamp);
            int code;
            if (securityStamp != null && int.TryParse(token, out code)) {
                if (Rfc6238Service.ValidateCode(securityStamp, code, phoneNumber)) {
                    return true;
                }
            }
            _logger.LogWarning(8, $"ValidateCode() failed for phone {phoneNumber}.");
            return false;
        }

        private void HandleErrors(IdentityResult result, string throwMsg = null) {
            foreach (var error in result.Errors) {
                _logger.LogError($"code: {error.Code} description: {error.Description}");
            }

            if (!string.IsNullOrEmpty(throwMsg)) {
                throw new Exception(throwMsg);
            }
        }
    }
}
